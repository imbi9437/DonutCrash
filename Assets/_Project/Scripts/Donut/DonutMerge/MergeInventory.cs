using _Project.Scripts.EventStructs;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeInventory : MonoBehaviour
{
    public GameObject donutPrefab;
    public RectTransform donutArea;

    public GameObject upgradeInfoPanel;
    public Image currentDonutIcon;
    public TextMeshProUGUI donutNameText;
    [SerializeField] private TextMeshProUGUI donutLvText;

    [SerializeField] private GameObject donutInfoPanel;
    [SerializeField] private Toggle detailInfoToggle;

    public Button selectBtn;
    public Button LevelOderBtn;
    public Button tierOderBtn;

    private bool isOrderLevel;
    private bool isOrderTier;
    void Awake()
    {
        EventHub.Instance.RegisterEvent<RecipeEventStructs.DonutInvenViewEvent>(OnDonutInventoryRapper);
    }
    void Start()
    {
        LevelOderBtn.onClick.RemoveAllListeners();
        tierOderBtn.onClick.RemoveAllListeners();

        LevelOderBtn.onClick.AddListener(OnClickLevelOderButton);
        tierOderBtn.onClick.AddListener(OnClickTierOderButton);
        isOrderLevel = false;
        isOrderTier = false;
        detailInfoToggle.isOn = false;
        detailInfoToggle.gameObject.SetActive(false);

        donutInfoPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    void OnDisable()
    {
        detailInfoToggle.onValueChanged.RemoveAllListeners();
        detailInfoToggle.isOn = false;
        isOrderLevel = false;
        isOrderTier = false;
        ClearInventory();
        gameObject.SetActive(false);
    }
    public void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.DonutInvenViewEvent>(OnDonutInventoryRapper);
    }


    private void OnDonutInventoryRapper(RecipeEventStructs.DonutInvenViewEvent evt) => OnDonutInventory();
    private void OnDetailInfo(bool ison, DonutInstanceData donut)
    {
        donutInfoPanel.SetActive(true);
        EventHub.Instance.RaiseEvent(new RecipeEventStructs.RequestDonutInstanceData(donut));
    }

    private void OnDonutInventory()
    {
        if (DataManager.Instance.Donuts == null) return;

        gameObject.SetActive(true);

        ClearInventory();
        UpdateInventory(DataManager.Instance.Donuts);
    }

    private void ClearInventory()
    {
        foreach (Transform r in donutArea.transform)
        {
            r.gameObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            Destroy(r.gameObject);
        }
        currentDonutIcon.gameObject.SetActive(false);
        donutNameText.gameObject.SetActive(false);
        donutLvText.gameObject.SetActive(false);
        selectBtn.interactable = false;
        detailInfoToggle.isOn = false;
        detailInfoToggle.gameObject.SetActive(false);
        donutInfoPanel.gameObject.SetActive(false);
    }
    private void UpdateInventory(List<DonutInstanceData> donuts)
    {

        var sorted = donuts;

        sorted = isOrderTier ? sorted = sorted.OrderBy(x => x.origin).ToList() : sorted.OrderByDescending(x => x.origin).ToList();

        sorted = sorted.GroupBy(x => x.origin).SelectMany
        (g => isOrderLevel ? g.OrderBy(x => x.level) : g.OrderByDescending(x => x.level))
        .ToList();

        foreach (DonutInstanceData donut in sorted)
        {
            if (donut.isLock) continue;
            InventorySlotUI obj = Instantiate(donutPrefab, donutArea.transform).GetComponent<InventorySlotUI>();
            obj.DonutInitialize(donut);
            obj.SetData(donut);
            obj.GetComponent<Button>().onClick.RemoveAllListeners();
            obj.GetComponent<Button>().onClick.AddListener(() => DonutChoice(donut));
        }
    }




    //강화 할 도넛 선택했을 때 실행
    private void DonutChoice(DonutInstanceData donut)
    {
        currentDonutIcon.gameObject.SetActive(true);
        donutNameText.gameObject.SetActive(true);
        donutLvText.gameObject.SetActive(true);
        DataManager.Instance.TryGetDonutData(donut.origin, out var donutData);
        donutNameText.text = donutData.donutName;
        donutLvText.text = $"LV.{donut.level}";
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
                {
                    currentDonutIcon.sprite = x;
                }).Forget();


        selectBtn.interactable = true;
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(() => SelectButton(donut.uid, donut.origin));
        detailInfoToggle.gameObject.SetActive(true);
        detailInfoToggle.onValueChanged.RemoveAllListeners();
        detailInfoToggle.isOn = false;
        detailInfoToggle.onValueChanged.AddListener(ison => OnDetailInfo(ison, donut));
        donutInfoPanel.SetActive(false);
    }


    private void SelectButton(string donutInstanceID, string origin)
    {
        gameObject.SetActive(false);
        EventHub.Instance.RaiseEvent(new DonutControlStructs.GetMergeableControlEvent(donutInstanceID, origin));
    }
    private void OnClickTierOderButton()
    {
        isOrderTier = !isOrderTier;
        ClearInventory();
        UpdateInventory(DataManager.Instance.Donuts);
    }
    private void OnClickLevelOderButton()
    {
        isOrderLevel = !isOrderLevel;
        ClearInventory();
        UpdateInventory(DataManager.Instance.Donuts);
    }
}
