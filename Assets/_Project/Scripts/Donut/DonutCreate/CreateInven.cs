using UnityEngine;
using UnityEngine.UI;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using System.Collections.Generic;
using _Project.Scripts.InGame.Remote;
using System.Diagnostics;
using Unity.VisualScripting;
using System.Linq;
using TMPro;

public class CreateInven : MonoBehaviour
{
    //[SerializeField] private GameObject donutCreatePanel;
    [SerializeField] private RectTransform donutRecipeArea;
    [SerializeField] private GameObject donutPrefab;
    [SerializeField] private Material mat;

    [SerializeField] private GameObject currentRecipePanel;
    [SerializeField] private Image currentRecipe;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI donutLevelText;
    [SerializeField] private Button selectBtn;

    void Awake()
    {
        EventHub.Instance.RegisterEvent<RES.OnSelectedDonutViewEvent>(SelectedDonutPanelRapper);
    }
    void Start()
    {
        currentRecipePanel.gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RES.OnSelectedDonutViewEvent>(SelectedDonutPanelRapper);
    }
    void OnDisable()
    {
        foreach (Transform child in donutRecipeArea.transform)
        {
            Destroy(child.gameObject);
        }
        currentRecipePanel.gameObject.SetActive(false);
    }
    private void SelectedDonutPanelRapper(RES.OnSelectedDonutViewEvent evt) => SelectedDonutPanel(evt.slotNumber);


    private void SelectedDonutPanel(int slotNum)
    {
        List<RecipeData> _recipes = DataManager.Instance.GetRecipeTable();
        if (_recipes == null) return;
        var sorted = _recipes.OrderBy(x => !DataManager.Instance.UnlockRecipe.Contains(x.uid)).ThenBy(x => x.uid).ToList();
        foreach (var item in sorted)
        {
            RecipePrefab dnObj = Instantiate(donutPrefab, donutRecipeArea).GetComponent<RecipePrefab>();
            dnObj.GetComponent<Button>().onClick.AddListener(() => OnClickDonutRecipe(item, slotNum));
            dnObj.DonutInitialize(item.result.itemId);
            if (!DataManager.Instance.UnlockRecipe.Contains(item.uid))
            {
                dnObj.transform.Find("Frame out/FrameIN").GetComponent<Image>().material = mat;
                dnObj.transform.Find("Frame out/FrameIN/IconImage").GetComponent<Image>().material = mat;
                dnObj.GetComponent<Button>().interactable = false;
            }
            else if (DataManager.Instance.UnlockRecipe.Contains(item.uid))
            {
                dnObj.transform.Find("Frame out/FrameIN").GetComponent<Image>().material = null;
                dnObj.transform.Find("Frame out/FrameIN/IconImage").GetComponent<Image>().material = null;
                dnObj.GetComponent<Button>().interactable = true;
            }
        }

    }

    //생성 할 도넛에 대한 클릭이벤트 등록
    private void OnClickDonutRecipe(RecipeData recipeData, int slotNum)
    {
        if (DataManager.Instance.TryGetDonutData(recipeData.result.itemId, out DonutData data))
            AddressableLoader.AssetLoadByPath<Sprite>(data.resourcePath, x => currentRecipe.sprite = x).Forget();

        recipeNameText.text = $"{data.donutName}";
        donutLevelText.text = "LV.1";
        currentRecipePanel.gameObject.SetActive(true);
        //클릭 된 레시피에 정보 패널 활성화
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(() => OpenPopup(recipeData, slotNum));

    }
    private void OpenPopup(RecipeData recipeData, int slotNum)
    {
        EventHub.Instance.RaiseEvent(new RES.RequestOpenCreateDonutPopUp(recipeData, slotNum));
    }
    //selectBtn에게 팝업 추가하고 추가하기.ㅣ
    //최초 생성시 정렬할 것.

}
