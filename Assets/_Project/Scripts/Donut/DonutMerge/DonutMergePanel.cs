using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;

public class DonutMergePanel : MonoBehaviour
{
    [Header("강화 슬롯")]
    [SerializeField]
    private TextMeshProUGUI upgradeDonutLevText;
    [SerializeField]
    private GameObject upgradeDonut;
    [SerializeField]
    private Button selectMergeDonutBtn;


    [Header("강화 재료 부분")]
    [SerializeField]
    private TextMeshProUGUI ingredientDonutLevText;
    [SerializeField]
    private Image ingredientDonutIcon;
    [SerializeField]
    private Button autoMergeBtn;
    [SerializeField]
    private Image ingredientNodeSlot;


    [Header("강화 결과 부분")]
    [SerializeField]
    private GameObject resultDonut;
    [SerializeField]
    private RectTransform resultStateInfoArea;
    [SerializeField]
    private TextMeshProUGUI resultDonutLevText;
    [SerializeField]
    private Image resultDonutIcon;
    [SerializeField]
    private TextMeshProUGUI currentAtkText;
    [SerializeField]
    private TextMeshProUGUI currentDefText;
    [SerializeField]
    private TextMeshProUGUI currentHpText;
    [SerializeField]
    private TextMeshProUGUI resultAtkText;
    [SerializeField]
    private TextMeshProUGUI resultDefText;
    [SerializeField]
    private TextMeshProUGUI resultHpText;
    [SerializeField]
    private TextMeshProUGUI successRate;
    [SerializeField]
    private Button mergeBtn;
    [SerializeField]
    private GameObject requireGoldBar;


    [Header("강화 도넛 선택 인벤토리")]
    [SerializeField]
    private GameObject donutInvenPanel;

    void Start()
    {
        EventHub.Instance.RegisterEvent<RES.OpenMergeViewlEvent>(OpenMergePanelRapper);
        EventHub.Instance.RegisterEvent<RES.CanAutoMergeViewEvent>(CanAutoMergeRapper);
        EventHub.Instance.RegisterEvent<RES.CanMergeViewEvnet>(CanMergeRapper);
        //EventHub.Instance.RegisterEvent<RES.RequestOpenSuccessPopup>(SuccessMergeRapper);
        //EventHub.Instance?.UnRegisterEvent<RES.RequestOpenSuccessPopup>(SuccessMergeRapper);
        //private void SuccessMergeRapper(RES.RequestOpenSuccessPopup evt) => OpenSuccessPopup();

        InitializedPanel();
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RES.OpenMergeViewlEvent>(OpenMergePanelRapper);
        EventHub.Instance?.UnRegisterEvent<RES.CanAutoMergeViewEvent>(CanAutoMergeRapper);
        EventHub.Instance?.UnRegisterEvent<RES.CanMergeViewEvnet>(CanMergeRapper);
    }

    private void OpenMergePanelRapper(RES.OpenMergeViewlEvent evt) => OnDonutSelected(evt.donutData);
    private void CanAutoMergeRapper(RES.CanAutoMergeViewEvent evt) => CanAutoMerge(evt.isOn);
    private void CanMergeRapper(RES.CanMergeViewEvnet evt) => CanMerge(evt.isOn);
    void OnDisable()
    {
        InitializedPanel();
    }

    /// <summary> 패널 초기화</summary>
    private void InitializedPanel()
    {
        selectMergeDonutBtn.onClick.RemoveAllListeners();
        selectMergeDonutBtn.onClick.AddListener(OnClickSelectButton);

        upgradeDonut.gameObject.SetActive(false);
        upgradeDonutLevText.gameObject.SetActive(false);
        DisableOtherDonut();
        resultStateInfoArea.gameObject.SetActive(false);



        mergeBtn.interactable = false;
        autoMergeBtn.interactable = false;
    }
    /// <summary> 강화 할 도넛 외 도넛들 비활성화 </summary>
    private void DisableOtherDonut()
    {
        ingredientDonutIcon.gameObject.SetActive(false);
        ingredientDonutLevText.gameObject.SetActive(false);
        resultDonutIcon.gameObject.SetActive(false);
        resultDonutLevText.gameObject.SetActive(false);
        resultStateInfoArea.gameObject.SetActive(false);
        ingredientNodeSlot.gameObject.SetActive(true);
        resultDonut.gameObject.SetActive(false);


        requireGoldBar.SetActive(false);
    }
    /// <summary> 강화 할 도넛 외 도넛들 활성화 </summary>
    private void EnableOtherDonut()
    {
        ingredientDonutIcon.gameObject.SetActive(true);
        ingredientDonutLevText.gameObject.SetActive(true);
        resultDonutIcon.gameObject.SetActive(true);
        resultDonutLevText.gameObject.SetActive(true);
        ingredientNodeSlot.gameObject.SetActive(false);
        resultDonut.gameObject.SetActive(true);
    }
    /// <summary> 머지에 관한 정보UI 표시 함수 <summary>
    private void OnDonutSelected(DonutInstanceData donutData)
    {
        UpdateDonutsInfo(donutData);
        UpdateResultStateText(donutData);

        mergeBtn.interactable = false;
        autoMergeBtn.interactable = false;


        DataManager.Instance.TryGetMergeData(donutData.level + 1, out DonutMergeData mergeData);
        requireGoldBar.SetActive(true);
        requireGoldBar.GetComponentInChildren<TextMeshProUGUI>().text = $"{mergeData.requireGold}";
    }
    /// <summary> 선택 된 도넛의 UI </summary>
    private void UpdateDonutsInfo(DonutInstanceData donutData)
    {
        if (donutData == null) return;
        upgradeDonut.gameObject.SetActive(true);

        DataManager.Instance.TryGetDonutData(donutData.origin, out var donut);
        UpdateIcon(donut);
        UpdateDonutLevel(donutData);
        DisableOtherDonut();
        upgradeDonutLevText.gameObject.SetActive(true);
    }
    /// <summary> 머지 후 다음 도넛의 스탯 UI 표시 함수 <summary>
    private void UpdateResultStateText(DonutInstanceData donutData)
    {
        DataManager.Instance.TryGetModifierData(donutData.origin, out var modifierData);
        donutData.CalcDonutStatus();
        currentAtkText.text = $"{donutData.atk}";
        currentDefText.text = $"{donutData.def}";
        currentHpText.text = $"{donutData.hp}";

        DonutInstanceData result = DonutInstanceData.CopyTo(donutData);
        result.level++;
        result.CalcDonutStatus();
        resultAtkText.text = $"{result.atk}";
        resultDefText.text = $"{result.def}";
        resultHpText.text = $"{result.hp}";


        DataManager.Instance.TryGetMergeData(donutData.level + 1, out DonutMergeData mergeData);
        successRate.text = $"성공확률 : {mergeData.successRate * 100}%";
    }

    /// <summary> 도넛 아이콘 변경</summary>
    private void UpdateIcon(DonutData donut)
    {
        AddressableLoader.AssetLoadByPath<Sprite>(donut.resourcePath, x =>
        {
            upgradeDonut.transform.Find("Frame out/FrameIN/IconImage").GetComponent<Image>().sprite = x;
            ingredientDonutIcon.sprite = x;
            resultDonut.transform.Find("Frame out/FrameIN/IconImage").GetComponent<Image>().sprite = x;
        }).Forget();
    }
    /// <summary> 도넛 레벨텍스트 변경</summary>
    private void UpdateDonutLevel(DonutInstanceData donutData)
    {
        DataManager.Instance.TryGetMergeData(donutData.level, out DonutMergeData mergeData);
        upgradeDonutLevText.text = $"Lv.{donutData.level}";
        ingredientDonutLevText.text = $"Lv.{mergeData.requireLevel}";
        resultDonutLevText.text = $"Lv.{donutData.level + 1}";
    }

    #region 도넛선택, 머지, 오토머지 버튼관련
    ///<summary>오토머지버튼 활성화 <summary>
    private void CanAutoMerge(bool ison)
    {
        autoMergeBtn.onClick.RemoveAllListeners();
        autoMergeBtn.onClick.AddListener(OnClickAutoMergeButton);
        autoMergeBtn.interactable = ison;
    }
    ///<summary> 머지버튼 활성화 <summary>
    private void CanMerge(bool ison)
    {
        if (!ison) return;
        mergeBtn.onClick.RemoveAllListeners();
        mergeBtn.onClick.AddListener(OnClickMergeButton);
        mergeBtn.interactable = ison;

        resultStateInfoArea.gameObject.SetActive(true);
        EnableOtherDonut();
    }
    ///<summary> 머지버튼 리스너 <summary>
    private void OnClickMergeButton()
    {
        EventHub.Instance.RaiseEvent(new DCS.OnClickMergeControlEvnet());
    }
    /// <summary> 오토머지 버튼 리스너 <summary>
    private void OnClickAutoMergeButton()
    {
        EventHub.Instance.RaiseEvent(new DCS.AutoMergeControlEvent());
    }
    ///<>
    ///<summary> 도넛 선택 버튼 리스너 <summary>
    private void OnClickSelectButton()
    {
        donutInvenPanel.SetActive(true);
        //플레이어가 가지고있는 도넛 생성
        EventHub.Instance.RaiseEvent(new DonutControlStructs.OpenInvenControlEvent());
    }
    #endregion
}

