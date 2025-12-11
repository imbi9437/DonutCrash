using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessUpgradPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private Ease animationEase;

    [SerializeField] private GameObject donutIcon;
    [SerializeField] private GameObject currentAtk;
    [SerializeField] private GameObject currentDef;
    [SerializeField] private GameObject currentHp;
    [SerializeField] private Button confirmButton;
    private Sequence _sequence;

    void Awake()
    {
        EventHub.Instance?.RegisterEvent<RecipeEventStructs.RequestOpenSuccessPopup>(SuccessMergeRapper);
    }
    void Start()
    {
        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.RequestOpenSuccessPopup>(SuccessMergeRapper);
    }
    void OnDisable()
    {
        gameObject.SetActive(false);
    }
    private void SuccessMergeRapper(RecipeEventStructs.RequestOpenSuccessPopup evt) => Initialize(evt.donut);
    public void Initialize(DonutInstanceData donut)
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;

        DonutSet(donut);

        confirmButton.onClick.AddListener(OnClickConfirmButton);
        Show();
    }
    private void DonutSet(DonutInstanceData donut)
    {
        donut.CalcDonutStatus();
        DonutInstanceData previous = DonutInstanceData.CopyTo(donut);
        previous.level--;
        previous.CalcDonutStatus();
        AddressableLoader.AssetLoadByPath<Sprite>(donut.origin, x => { donutIcon.transform.Find("Frame out/FrameIN/IconImage").GetComponent<Image>().sprite = x; }).Forget();

        donutIcon.transform.Find("LV").GetComponent<TextMeshProUGUI>().text = $"Lv.{donut.level}";

        currentAtk.transform.Find("CurrentState").GetComponent<TextMeshProUGUI>().text = $"{previous.atk}";
        currentDef.transform.Find("CurrentState").GetComponent<TextMeshProUGUI>().text = $"{previous.def}";
        currentHp.transform.Find("CurrentState").GetComponent<TextMeshProUGUI>().text = $"{previous.hp}";

        currentAtk.transform.Find("ResultState").GetComponent<TextMeshProUGUI>().text = $"{donut.atk}";
        currentDef.transform.Find("ResultState").GetComponent<TextMeshProUGUI>().text = $"{donut.def}";
        currentHp.transform.Find("ResultState").GetComponent<TextMeshProUGUI>().text = $"{donut.hp}";
    }

    private void Show()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence();

        gameObject.SetActive(true);

        var fadeTween = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(1, animationTime).SetEase(animationEase);

        _sequence.Join(fadeTween);
        _sequence.Join(scaleTween);

        _sequence.onComplete += () => canvasGroup.interactable = true;
    }

    public void Hide()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence();

        canvasGroup.interactable = false;

        var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(0, animationTime).SetEase(animationEase);

        _sequence.Join(fadeTween);
        _sequence.Join(scaleTween);

        _sequence.onComplete += () => gameObject.SetActive(false);
    }

    private void OnClickConfirmButton()
    {
        gameObject.SetActive(false);
        Hide();
    }
}
