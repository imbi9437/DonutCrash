using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using _Project.Scripts.InGame.Remote;
using DonutClash.UI.GlobalUI;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using DS = _Project.Scripts.EventStructs.DataStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;

public class DonutMergeController : MonoBehaviour
{
    private List<DonutInstanceData> _userDonuts;
    private int _newGold;

    private bool _donutDirty;
    private bool _goldDirty;

    //  각 레벨별 머지 포인트를 저장할 Dictionary 필드 선언
    private readonly Dictionary<int, int> _mergePoint = new();
    //  머지에 소모될 도넛을 저장할 필드 선언
    private List<DonutInstanceData> _ingredientDonuts = new();


    //강화를 위해 선택 된 도넛
    private DonutInstanceData _currentDonut;
    private DonutInstanceData _ingredientDonut;


    private void Start()
    {
        _userDonuts = DataManager.Instance.Donuts;
        _newGold = DataManager.Instance.UserGold;

        CalculateMergePoint(20);

        EventHub.Instance.RegisterEvent<DCS.GetMergeableControlEvent>(GetMergeableListRapper);
        EventHub.Instance.RegisterEvent<DCS.OpenInvenControlEvent>(OpenInvenRapper);
        EventHub.Instance.RegisterEvent<DCS.OnClickMergeControlEvnet>(MergeStartRapper);
        EventHub.Instance.RegisterEvent<DCS.AutoMergeControlEvent>(AutoMergeSystemRapper);

        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserGoldEvent>(OnBroadcast);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DCS.GetMergeableControlEvent>(GetMergeableListRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.OpenInvenControlEvent>(OpenInvenRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.OnClickMergeControlEvnet>(MergeStartRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.AutoMergeControlEvent>(AutoMergeSystemRapper);

        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserGoldEvent>(OnBroadcast);
    }


    private void GetMergeableListRapper(DCS.GetMergeableControlEvent evt) => GetMergeableCheck(evt.instanceId, evt.origin);
    private void OpenInvenRapper(DCS.OpenInvenControlEvent evt) => OpenInventory();
    private void MergeStartRapper(DCS.OnClickMergeControlEvnet evt) => MergeStart();
    private void AutoMergeSystemRapper(DCS.AutoMergeControlEvent evt) => AutoMerge();

    private void OnBroadcast(DS.BroadcastSetUserDonutEvent evt) => _userDonuts = DataManager.Instance.Donuts;
    private void OnBroadcast(DS.BroadcastSetUserGoldEvent evt) => _newGold = DataManager.Instance.UserGold;

    /// <summary>
    /// 유저가 가지고있는 도넛에서 강화재료 도넛리스트를 가져오는 메서드
    /// </summary>
    /// <param name="evt">재료에 맞는 도넛인턴스의 Uid</param>
    private void GetMergeableCheck(string donutId, string origin)
    {
        //강화를 위해 선택 된 도넛 정보
        _currentDonut = _userDonuts.Find(x => x.uid == donutId);

        DataManager.Instance.TryGetModifierData(_currentDonut.origin, out DonutModifierData data);

        EventHub.Instance.RaiseEvent(new RES.OpenMergeViewlEvent(_currentDonut));

        //머지에 대한 조건 검사
        CheckMerge(_currentDonut);
    }

    /// <summary>
    /// 머지를 하기위해 인벤을 열었을 때 실행 될 메서드
    /// </summary>
    private void OpenInventory()
    {
        EventHub.Instance.RaiseEvent(new RES.DonutInvenViewEvent());
    }
    ///<summary>머지버튼이 눌려 머지 하는 함수<summary>
    private void MergeStart()
    {
        DataManager.Instance.TryGetMergeData(_currentDonut.level + 1, out DonutMergeData mergeData);
        bool isSuccess = false;
        isSuccess = Random.value < mergeData.successRate;

        TwoButtonParam param = new TwoButtonParam("도넛 머지 강화", "강화를 시도하시겠습니까?", "예", "아니요");
        param.confirm = () =>
        {
            Merge(_currentDonut, isSuccess);
            GetMergeableCheck(_currentDonut.uid, _currentDonut.origin);
        };
        EventHub.Instance?.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
    }

    private void AutoMerge()
    {
        TwoButtonParam param = new TwoButtonParam("재료 도넛 자동 조합", "보유 도넛에서 재료도넛이 자동 합성 됩니다.", "예", "아니요");
        param.confirm = () => AutoMergeSystem();
        EventHub.Instance?.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
    }

    ///<summary>자동머지에 대한 함수 <summary>
    private void AutoMergeSystem()
    {
        bool isEnd = false;
        DataManager.Instance.TryGetMergeData(_currentDonut.level, out DonutMergeData mergeData);
        while (!isEnd)
        {
            Modifier(_ingredientDonuts[0]);
            if (_ingredientDonuts[0].level == mergeData.requireLevel) isEnd = true;
        }
        Debug.Log("자동머지 끝");
        for (int i = 1; i < _ingredientDonuts.Count; i++)
        {
            _userDonuts.Remove(_ingredientDonuts[i]);
        }

        _donutDirty = true;

        UpdateData();

        _ingredientDonut = _ingredientDonuts[0];
        GetMergeableCheck(_currentDonut.uid, _currentDonut.origin);
    }

    /// <summary> 도넛 머지 확인</summary>
    private void CheckMerge(DonutInstanceData target)
    {
        if (target == null && !DataManager.Instance.Donuts.Contains(target))
        {
            Debug.Log("타겟 없음");
            return;
        }
        if (!CanMerge(target))
        {
            Debug.Log($"머지 불가능 {CanMerge(target)}");
            return;
        }
        _ingredientDonut = null;
        FillSacrificeList(target, out bool isOverPoint);
        if (!isOverPoint)
        {
            Debug.Log($"재료 없음 {isOverPoint}");
            return;
        }
        //  EventHub를 통해 관련 이벤트 발행
        Debug.Log("재료 불러오기 성공");

        if (isOverPoint && _ingredientDonut != null)
        {
            EventHub.Instance.RaiseEvent(new RES.CanMergeViewEvnet(isOverPoint));
        }
        else if (isOverPoint)
        {
            EventHub.Instance.RaiseEvent(new RES.CanAutoMergeViewEvent(isOverPoint));
        }
    }

    /// <summary> 도넛 머지 </summary>
    private void Merge(DonutInstanceData target, bool isSuccess)
    {
        DataManager.Instance.TryGetMergeData(target.level, out DonutMergeData mergeData);
        _userDonuts.Remove(_ingredientDonut);
        _donutDirty = true;
        _newGold -= mergeData.requireGold;
        _goldDirty = true;

        if (!isSuccess)
        {
            OneButtonParam parm = new OneButtonParam("강화 실패", $"강화에 실패했습니다.", "확인");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, parm));
        }
        else
        {
            Modifier(target);
            EventHub.Instance.RaiseEvent(new RES.RequestOpenSuccessPopup(target));
        }

        //  데이터 저장 이벤트 발행
        UpdateData();
    }

    /// <summary> 머지 조건 검사 </summary>
    private bool CanMerge(DonutInstanceData target)
    {
        //  target이 null이거나 uid가 없는 경우 Return false
        if (target == null || target.uid == null) return false;
        //  UserData의 골드가 모자랄 경우 Return false
        DataManager.Instance.TryGetMergeData(target.level, out DonutMergeData mergeData);
        if (_newGold < mergeData.requireGold)
        {
            Debug.Log("보유중인 골드가 모자람");
            return false;
        }
        //  보유중인 도넛의 점수가 모자랄 경우 Return false
        if (_mergePoint[mergeData.requireLevel] > CalculateCurMergePoint(target))
        {
            Debug.Log("보유중인 도넛의 점수가 모자람");
            return false;
        }

        return true;
    }

    /// <summary> 레벨업 및 스탯 보정 </summary>
    /// <param name="target">레벨업 할 도넛 객체</param>
    private void Modifier(DonutInstanceData target)
    {
        //  target이 null 이거나 UID가 없는 경우 Return
        if (target == null && target.uid == null) return;
        //  target의 origin uid에 맞는 ModifierData가 없는경우 Return
        if (!DataManager.Instance.TryGetModifierData(target.origin, out DonutModifierData data)) return;
        //  target의 레벨을 1 올린 후 각종 스탯 보정하기
        target.level++;
        target.CalcDonutStatus();
    }

    /// <summary> 다음 레벨에 필요한 도넛의 점수 계산 </summary>
    private int CalculateMergePoint(int currentLevel)
    {
        int point = 0;
        currentLevel--;
        if (currentLevel <= 0)
        {
            _mergePoint[1] = 1;
            return 1;
        }
        point = 2 * CalculateMergePoint(currentLevel);
        _mergePoint[currentLevel + 1] = point;
        return point;
    }

    /// <summary> 보유중인 도넛의 점수 계산 </summary>
    private int CalculateCurMergePoint(DonutInstanceData target)
    {
        int point = 0;
        _ingredientDonuts.Clear();
        //  탐색 중 도넛의 originId 가 target의 originId와 다를경우 Continue
        //  탐색 중 도넛의 Level이 target의 Level보다 높을 경우 Continue
        //  탐색 중 도넛의 uid가 target의 uid와 같을 경우 Continue
        //  탐색 중 도넛의 isLock이 true일 경우 Continue
        DataManager.Instance.TryGetMergeData(target.level, out DonutMergeData mergeData);
        int i = 0;
        foreach (DonutInstanceData ingredientDonut in _userDonuts)
        {
            if (ingredientDonut.origin != target.origin) continue;
            if (ingredientDonut.level > target.level || ingredientDonut.level > mergeData.requireLevel) continue;
            if (ingredientDonut.isLock) continue;
            if (ingredientDonut.uid == target.uid) continue;
            point += _mergePoint[ingredientDonut.level];
            _ingredientDonuts.Add(ingredientDonut);
        }
        return point;
    }

    /// <summary> 선택될 재료 설정 </summary>
    private void FillSacrificeList(DonutInstanceData target, out bool isOverPoint)
    {
        int point = 0;
        int currentPoint = 0;
        isOverPoint = false;

        //  선택 재료 리스트 초기화
        _ingredientDonuts.Clear();
        List<DonutInstanceData> finIngredients = new();

        DataManager.Instance.TryGetMergeData(target.level, out DonutMergeData mergeData);

        point = _mergePoint[mergeData.requireLevel];
        CalculateCurMergePoint(target);
        _ingredientDonuts = _ingredientDonuts.OrderByDescending(x => x.level).ToList();
        //  필터링 된 도넛을 탐색해 point가 점수를 넘어가기 전까지 선택 리스트에 추가
        //  만약 점수가 초과될 경우 계속 탐색해 마지막에 넣은 도넛보다 점수가 낮고 목표에 정확하게 일치할 경우 마지막 도넛 변경

        for (int i = 0; i < _ingredientDonuts.Count; i++)
        {
            currentPoint += _mergePoint[_ingredientDonuts[i].level];
            if (point <= currentPoint && !isOverPoint)
            {
                finIngredients.Add(_ingredientDonuts[i]);
                isOverPoint = true;
            }
            else if (!isOverPoint)
            {
                finIngredients.Add(_ingredientDonuts[i]);
            }
        }
        if (point > currentPoint)
        {
            isOverPoint = false;
            return;
        }
        isOverPoint = true;
        _ingredientDonuts.Clear();
        _ingredientDonuts = finIngredients;

        if (_ingredientDonuts.Count == 1) _ingredientDonut = _ingredientDonuts[0];
    }

    private void UpdateData()
    {
        if (_donutDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetDonutListEvent(_userDonuts));
        if (_goldDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetGoldEvent(_newGold));

        ResetDirty();
    }

    private void ResetDirty()
    {
        _donutDirty = false;
        _goldDirty = false;
    }
}