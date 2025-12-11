using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DonutInfoPanel : MonoBehaviour
{
    [SerializeField] private Image donutImage; //도넛 이미지 
    [SerializeField] private TMP_Text donutNameText; //도넛 이름 
    [SerializeField] private TMP_Text donutEffectText; //도넛 특수효과 설명text
    [SerializeField] private TMP_Text donutLvText; //도넛 레벨
    [SerializeField] private TMP_Text donutAtkText; //도넛 공격력 
    [SerializeField] private TMP_Text donutDefText;
    [SerializeField] private TMP_Text donutHpText;
    [SerializeField] private TMP_Text donutMassText;
    [SerializeField] private TMP_Text donutCritText;

    [SerializeField] private Button cancelButton;

    void Awake()
    {
        EventHub.Instance?.RegisterEvent<RecipeEventStructs.RequestDonutInstanceData>(DonutDataConnectRapper);
    }
    void Start()
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.RequestDonutInstanceData>(DonutDataConnectRapper);
    }
    private void DonutDataConnectRapper(RecipeEventStructs.RequestDonutInstanceData evt) => DonutDataConnect(evt.donut);

    private void DonutDataConnect(DonutInstanceData donut)
    {
        if (donut == null) return;
        gameObject.SetActive(true);
        DonutInfoUIUpdate(donut);
    }
    private void DonutInfoUIUpdate(DonutInstanceData donut)
    {
        DataManager.Instance.TryGetDonutData(donut.origin, out DonutData donutData);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x => donutImage.sprite = x).Forget();
        donut.CalcDonutStatus();
        donutNameText.text = donutData.donutName;
        donutAtkText.text = $"  {donut.atk}";
        donutDefText.text = $"DEF : {donut.def}";
        donutHpText.text = $"  {donut.hp}";
        donutCritText.text = $"CRI : {donut.crit}";
        donutLvText.text = $"LV : {donut.level}";
        donutMassText.text = $"MASS : {donut.mass}";

        donutEffectText.text = donutData.donutDescription;
    }

    void Reset()
    {
        donutImage = transform.Find("BackGround/ImageFrame/DonutIcon").GetComponent<Image>();

        donutNameText = transform.Find("BackGround/TopInfo/DonutNameBackGround/DonutNameText").GetComponent<TMP_Text>();
        donutLvText = transform.Find("BackGround/TopInfo/DonutLv").GetComponent<TMP_Text>();

        donutEffectText = transform.Find("BackGround/TextBoxFrame/DescriptionText").GetComponent<TMP_Text>();

        donutAtkText = transform.Find("BackGround/BottomInfo/AtkIcon/AtkText").GetComponent<TMP_Text>();
        donutDefText = transform.Find("BackGround/BottomInfo/DefText").GetComponent<TMP_Text>();
        donutHpText = transform.Find("BackGround/BottomInfo/HPIcon/HpText").GetComponent<TMP_Text>();
        donutMassText = transform.Find("BackGround/BottomInfo/MassText").GetComponent<TMP_Text>();
        donutCritText = transform.Find("BackGround/BottomInfo/CRIText").GetComponent<TMP_Text>();
    }
}
