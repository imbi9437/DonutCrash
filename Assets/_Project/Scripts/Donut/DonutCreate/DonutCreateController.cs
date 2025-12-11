using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;
public class DonutCreateController : MonoBehaviour
{
    private TrayData _tray;
    private List<DonutInstanceData> _donuts;
    private Dictionary<string, int> _ingredients = new();

    private bool _isBaking;
    private DateTime _startTime;
    private DateTime _endTime;

    private int _requiredGold;
    private int _newGold;

    private bool _goldDirty;
    private bool _donutDirty;
    private bool _ingredientsDirty;
    private bool _trayDirty;

    void Start()
    {
        _tray = DataManager.Instance.TrayData;
        _ingredients = DataManager.Instance.Ingredients;
        _donuts = DataManager.Instance.Donuts;
        _newGold = DataManager.Instance.UserGold;
        Debug.Log("트레이 세팅");
        InitTray(_tray);


        EventHub.Instance.RegisterEvent<DCS.TrayPanelOpenControlEvent>(OnTrayPanelRapper);
        EventHub.Instance.RegisterEvent<DCS.CheckRequiredIngredientsControlEvent>(CanPlaceableOnTrayRapper);
        EventHub.Instance.RegisterEvent<DCS.SuccessOnTrayControlEvnet>(SuccessOnTrayDonutRapper);
        EventHub.Instance.RegisterEvent<DCS.StartBakingControlEvent>(OnBakingRapper);
        EventHub.Instance.RegisterEvent<DCS.CreateDonutControlEvent>(FinishDonutBakeRapper);
        EventHub.Instance.RegisterEvent<DCS.TrayClearControlEvnet>(ClearAllTrayRapper);
        EventHub.Instance.RegisterEvent<DCS.OnTrayDonutCancelControlEvent>(ReturnIngredientsRapper);
        EventHub.Instance.RegisterEvent<DCS.SelectedDonutControlEvent>(OpenDonutSelectPanelRapper);

        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserIngredientEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserGoldEvent>(OnBroadcast);
        EventHub.Instance?.RegisterEvent<DataStructs.BroadcastSetUserTrayEvent>(OnBroadcast);


    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DCS.TrayPanelOpenControlEvent>(OnTrayPanelRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.CheckRequiredIngredientsControlEvent>(CanPlaceableOnTrayRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.SuccessOnTrayControlEvnet>(SuccessOnTrayDonutRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.StartBakingControlEvent>(OnBakingRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.CreateDonutControlEvent>(FinishDonutBakeRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.TrayClearControlEvnet>(ClearAllTrayRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.OnTrayDonutCancelControlEvent>(ReturnIngredientsRapper);
        EventHub.Instance?.UnRegisterEvent<DCS.SelectedDonutControlEvent>(OpenDonutSelectPanelRapper);

        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserIngredientEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserGoldEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserTrayEvent>(OnBroadcast);
    }

    private void OnTrayPanelRapper(DCS.TrayPanelOpenControlEvent evt) => TrayPanelOpen();
    private void CanPlaceableOnTrayRapper(DCS.CheckRequiredIngredientsControlEvent evt) => CanPlaceableOnTray(evt.recipeId, evt.requiredCount, evt.trayNum);
    private void SuccessOnTrayDonutRapper(DCS.SuccessOnTrayControlEvnet evt) => SuccessOnTrayDonut(evt.recipeUid, evt.requiredCount, evt.slotNumber);
    private void OnBakingRapper(DCS.StartBakingControlEvent evt) => OnBaking();
    private void FinishDonutBakeRapper(DCS.CreateDonutControlEvent evt) => FinishDonutBake();
    private void ClearAllTrayRapper(DCS.TrayClearControlEvnet evt) => ClearAllTray();
    private void ReturnIngredientsRapper(DCS.OnTrayDonutCancelControlEvent evt) => ReturnIngredients(evt.slotNumber);
    private void OpenDonutSelectPanelRapper(DCS.SelectedDonutControlEvent evt) => OpenDonutSelectPanel(evt.slotNum);

    private void OnBroadcast(DataStructs.BroadcastSetUserDonutEvent evt) => _donuts = DataManager.Instance.Donuts;
    private void OnBroadcast(DataStructs.BroadcastSetUserIngredientEvent evt) => _ingredients = DataManager.Instance.Ingredients;
    private void OnBroadcast(DataStructs.BroadcastSetUserGoldEvent evt) => _newGold = DataManager.Instance.UserGold;
    private void OnBroadcast(DataStructs.BroadcastSetUserTrayEvent evt) => _tray = DataManager.Instance.TrayData;

    private void TrayPanelOpen()
    {
        EventHub.Instance.RaiseEvent(new RES.OnTrayPanleViewEvent());
    }

    private void OpenDonutSelectPanel(int slotNumber)
    {
        EventHub.Instance.RaiseEvent(new RES.OnDonutSelectPanelViewEvent());

        EventHub.Instance.RaiseEvent(new RES.OnSelectedDonutViewEvent(slotNumber));

    }
    //트레이에 올라가기 전 도넛레시피를 통해 제작 가능한지, 제작할 도넛의 개수 설정하는 부분
    public void CanPlaceableOnTray(string recipeId, int requiredCount, int trayNum)
    {
        //조건이 충족되었다면 버튼을 활성화 시킴
        bool canCreate = CanOnTrayCheck(recipeId, requiredCount);
        EventHub.Instance.RaiseEvent(new RES.OnTrayButtonViewEvent(canCreate));
    }



    /// <summary> 생성할 도넛을 트레이에 올림 </summary>
    public void SuccessOnTrayDonut(string recipeUid, int requiredCount, int slotNumber)
    {
        DataManager.Instance.TryGetRecipeData(recipeUid, out RecipeData data);
        DataManager.Instance.TryGetDonutData(data.result.itemId, out DonutData donut);
        Debug.Log($"추가 :{requiredCount}");
        if (_tray.slots[slotNumber].count != 0)
        {
            ReturnIngredients(slotNumber);
        }
        _tray.slots[slotNumber].resultId = donut.uid;
        _tray.slots[slotNumber].count = requiredCount;
        _trayDirty = true;
        foreach (var r in data.ingredients)
        {
            if (_ingredients.TryGetValue(r.itemId, out int value))
            {
                _ingredients[r.itemId] = value - (r.count * requiredCount);
                _ingredientsDirty = true;
            }
        }
        UpdateData();
        _requiredGold += data.requireGold * requiredCount;

        bool canCreate = CanCreateDonut();

        //트레이 패널에 UI적 요소를 보내 정보를 보여줄 이벤트
        EventHub.Instance.RaiseEvent(new RES.AddToTrayViewEvent(donut, requiredCount, slotNumber, _requiredGold, _tray.grade));
        EventHub.Instance.RaiseEvent(new RES.CanCreateDonutViewEvnet(canCreate));
        EventHub.Instance.RaiseEvent(new RES.CreatePanelCloseViewEvent());
    }

    public void ReturnIngredients(int slotNumber)
    {
        if (_tray.slots[slotNumber].resultId == null || _tray.slots == null) return;
        DataManager.Instance.TryGetResultRecipeData(_tray.slots[slotNumber].resultId, out RecipeData data);
        foreach (var r in data.ingredients)
        {
            if (_ingredients.TryGetValue(r.itemId, out int value))
            {
                _ingredients[r.itemId] = value + (r.count * _tray.slots[slotNumber].count);
                _ingredientsDirty = true;
            }
        }
        _requiredGold -= data.requireGold * _tray.slots[slotNumber].count;
        _tray.slots[slotNumber].resultId = null;
        _tray.slots[slotNumber].count = 0;
        _trayDirty = true;

        bool canCreate = CanCreateDonut();
        EventHub.Instance.RaiseEvent(new RES.CanCreateDonutViewEvnet(canCreate));
        EventHub.Instance.RaiseEvent(new RES.RefreshGoldViewEvent(_requiredGold));

        UpdateData();
    }

    //트레이에 올릴 수 있는지 조건검사
    public bool CanOnTrayCheck(string recipeUid, int required)
    {
        DataManager.Instance.TryGetRecipeData(recipeUid, out RecipeData data);
        foreach (var r in data.ingredients)
        {
            if (!_ingredients.ContainsKey(r.itemId))
            {
                _ingredients.Add(r.itemId, 0);
                _ingredientsDirty = true;
                UpdateData();
                return false;
            }
            if (_ingredients[r.itemId] < r.count * required)
            {
                return false;
            }
            if (_tray.grade < required) return false;
        }

        return true;
    }
    private bool CanCreateDonut()
    {
        int result = _newGold - _requiredGold;
        bool isOn = false;

        foreach (var r in _tray.slots)
        {
            if (r.count <= 0)
                continue;
            isOn = true;
        }
        if (result >= 0 && isOn)
        {
            return true;
        }

        return false;
    }
    public void ClearAllTray()
    {
        if (_tray.slots.Count == 0) return;
        foreach (var r in _tray.slots)
        {
            if (r == null || r.resultId == null) continue;
            DataManager.Instance.TryGetResultRecipeData(r.resultId, out RecipeData data);
            foreach (var item in data.ingredients)
            {
                _ingredients[item.itemId] += item.count * r.count;
            }
            _ingredientsDirty = true;
            r.resultId = null;
            r.count = 0;
            _newGold += _requiredGold;
            _requiredGold = 0;
        }
        _goldDirty = true;
        _trayDirty = true;
        UpdateData();
    }
    //도넛 생성버튼을 눌러 제작에 들어가는 부분.
    public void OnBaking()
    {
        _isBaking = true;
        _goldDirty = true;
        _newGold -= _requiredGold;

        _startTime = DateTime.Now;
        //_endTime = _startTime.AddMinutes(1.5);
        _endTime = _startTime.AddSeconds(20); //테스트용 20초
        _tray.startTime = _startTime.ToString("o");
        _tray.endTime = _endTime.ToString("o");

        _trayDirty = true;
        UpdateData();

        EventHub.Instance.RaiseEvent(new RES.StartBakingViewEvent(_startTime, _endTime));

    }

    //시간이 지나면 FinishDonutBake 메서드를 통해 도넛데이터에서 도넛인스턴스 데이터에 생성 될 것
    public void FinishDonutBake()
    {
        foreach (var r in _tray.slots)
        {
            for (int i = 0; i < r.count; i++)
            {
                DataManager.Instance.TryGetDonutData(r.resultId, out DonutData donutData);
                _donuts.Add(new DonutInstanceData(donutData));
                Debug.Log($"도넛 생성 완료{r.resultId}");
            }
        }
        _isBaking = false;
        _requiredGold = 0;
        foreach (var r in _tray.slots)
        {
            r.resultId = null;
            r.count = 0;
        }
        _tray.startTime = null;
        _tray.endTime = null;

        _donutDirty = true;
        _trayDirty = true;
        UpdateData();

        OneButtonParam parm = new OneButtonParam("도넛생성 완료 !", $"시간이 되어 도넛이 생성 되었습니다.", "확인");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, parm));
    }

    private void InitTray(TrayData tray)
    {
        if (tray.startTime != null || tray.endTime != null)
        {
            DateTime now = DateTime.Now;
            var remaining = DateTime.Parse(tray.endTime) - now;
            if (remaining.TotalSeconds <= 0)
            {
                FinishDonutBake();

            }
            _isBaking = true;
            return;
        }
        if (tray == null) return;
        if (tray.slots == null) return;
        ClearAllTray();
    }

    private void UpdateData()
    {
        if (_donutDirty) { EventHub.Instance.RaiseEvent(new DataStructs.RequestSetDonutListEvent(_donuts)); }
        if (_goldDirty) { EventHub.Instance.RaiseEvent(new DataStructs.RequestSetGoldEvent(_newGold)); }
        if (_ingredientsDirty) { EventHub.Instance.RaiseEvent(new DataStructs.RequestSetIngredientEvent(_ingredients)); }
        if (_trayDirty) { EventHub.Instance.RaiseEvent(new DataStructs.RequestSetTrayEvent(_tray)); }

        ResetDirty();
    }
    private void ResetDirty()
    {
        _goldDirty = false;
        _ingredientsDirty = false;
        _donutDirty = false;
        _trayDirty = false;
    }

}