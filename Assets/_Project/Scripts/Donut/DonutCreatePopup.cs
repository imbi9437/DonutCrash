using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using DG.Tweening;
using System.Net;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DonutCreatePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private Ease animationEase;


    [SerializeField] private Image currentDonutIcon;
    [SerializeField] private Image doughIcon;
    [SerializeField] private Image ingredientIcon;

    [SerializeField] private TextMeshProUGUI requiredIngredientCountText;
    [SerializeField] private TextMeshProUGUI requiredhasDoughCountText;
    [SerializeField] private TMP_InputField requiredCountText;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private Button countUpBtn;
    [SerializeField] private Button countDownBtn;
    [SerializeField] private Button maxButton;
    [SerializeField] private Button minButton;

    private Sequence _sequence;

    private bool isEditing;
    private int donutCount = 1;

    private int hasDongh;
    private int hasIngredient;
    private int requireDough;
    private int requireIngredient;
    void Awake()
    {
        EventHub.Instance?.RegisterEvent<RecipeEventStructs.RequestOpenCreateDonutPopUp>(OpenCreatePopup);
        EventHub.Instance.RegisterEvent<RecipeEventStructs.OnTrayButtonViewEvent>(CanPlaceadTrayDonutRapper);
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.RequestOpenCreateDonutPopUp>(OpenCreatePopup);
        EventHub.Instance?.UnRegisterEvent<RecipeEventStructs.OnTrayButtonViewEvent>(CanPlaceadTrayDonutRapper);
    }
    void OnDisable()
    {
        gameObject.SetActive(false);
    }
    private void OpenCreatePopup(RecipeEventStructs.RequestOpenCreateDonutPopUp evt) => Initialize(evt.recipeData, evt.slotNum);
    private void CanPlaceadTrayDonutRapper(RecipeEventStructs.OnTrayButtonViewEvent evt) => CanPlaceadTrayDonut(evt.isOn);




    public void Initialize(RecipeData recipeData, int slotNum)
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        donutCount = 1;

        PopupSet(recipeData);
        SetText(recipeData);

        ClearListeners();
        UpdateListeners(recipeData.uid, slotNum);

        Show();
    }

    private void PopupSet(RecipeData data)
    {
        DataManager.Instance.TryGetDonutData(data.result.itemId, out DonutData donutData);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x => { currentDonutIcon.sprite = x; }).Forget();


        AddressableLoader.AssetLoadByPath<Sprite>(data.ingredients[0].itemId, x =>
        {
            doughIcon.sprite = x;
        }).Forget();
        AddressableLoader.AssetLoadByPath<Sprite>(data.ingredients[1].itemId, x =>
        {
            ingredientIcon.sprite = x;
        }).Forget();
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

    private void OnClickConfirmButton(string recipeId, int donutCount, int slotNum)
    {
        EventHub.Instance.RaiseEvent(new DonutControlStructs.SuccessOnTrayControlEvnet(recipeId, donutCount, slotNum));
        gameObject.SetActive(false);
        Hide();
    }
    private void SetText(RecipeData data)
    {
        hasDongh = DataManager.Instance.Ingredients[data.ingredients[0].itemId];
        hasIngredient = DataManager.Instance.Ingredients[data.ingredients[1].itemId];
        requireDough = data.ingredients[0].count;
        requireIngredient = data.ingredients[1].count;

        UpdateText();
    }
    private void UpdateText()
    {
        requiredhasDoughCountText.text = $"{hasDongh} / {requireDough * donutCount}";
        requiredIngredientCountText.text = $"{hasIngredient} / {requireIngredient * donutCount}";
        requiredCountText.text = $"{donutCount}";
    }

    /// <summary> 버튼 리스너 초기화 </summary>
    #region 버튼 관련
    private void CanPlaceadTrayDonut(bool ison)
    {
        confirmButton.interactable = ison;
    }
    private void ClearListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        countUpBtn.onClick.RemoveAllListeners();
        countDownBtn.onClick.RemoveAllListeners();
        maxButton.onClick.RemoveAllListeners();
        minButton.onClick.RemoveAllListeners();
    }
    private void UpdateListeners(string recipeId, int trayNum)
    {
        countUpBtn.onClick.AddListener(() => OnClickCountUpButton(recipeId, trayNum));
        countDownBtn.onClick.AddListener(() => OnClickCountDownButton(recipeId, trayNum));
        maxButton.onClick.AddListener(() => OnClickMaxButton(recipeId, trayNum));
        minButton.onClick.AddListener(() => OnClickMinButton(recipeId, trayNum));
        requiredCountText.onValueChanged.AddListener((x) => OnInputChanged(recipeId, x, trayNum));
        confirmButton.onClick.AddListener(() => OnClickConfirmButton(recipeId, donutCount, trayNum));
        cancelButton.onClick.AddListener(() => Hide());
    }
    #endregion

    #region 도넛 생상량 조절하는 부분
    /// <summary>트레이에 올릴 도넛의 개수 버튼</summary>
    public void OnClickCountUpButton(string recipeId, int trayNum)
    {
        donutCount++;
        ApplyValue(recipeId, donutCount, trayNum);
    }
    public void OnClickCountDownButton(string recipeId, int trayNum)
    {
        donutCount--;
        ApplyValue(recipeId, donutCount, trayNum);
    }
    public void OnClickMaxButton(string recipeId, int trayNum)
    {
        donutCount = DataManager.Instance.TrayData.grade;
        ApplyValue(recipeId, donutCount, trayNum);
    }
    public void OnClickMinButton(string recipeId, int trayNum)
    {
        donutCount = 1;
        ApplyValue(recipeId, donutCount, trayNum);
    }
    private void OnInputChanged(string recipeId, string requiredCount, int trayNum)
    {
        if (isEditing) return;
        isEditing = true;
        if (int.TryParse(requiredCount, out int number))
        {
            ApplyValue(recipeId, number, trayNum);
        }
        else
        {
            ApplyValue(recipeId, 1, trayNum);
        }
        isEditing = false;
    }
    private void ApplyValue(string recipeId, int count, int trayNum)
    {
        donutCount = Mathf.Clamp(count, 1, DataManager.Instance.TrayData.grade);

        DataManager.Instance.TryGetRecipeData(recipeId, out RecipeData recipeData);

        EventHub.Instance?.RaiseEvent(
            new DonutControlStructs.CheckRequiredIngredientsControlEvent(recipeId, donutCount, trayNum)
        );
        UpdateText();
    }
    #endregion
}
