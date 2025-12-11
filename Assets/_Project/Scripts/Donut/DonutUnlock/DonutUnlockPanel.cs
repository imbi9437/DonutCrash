using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;
public class DonutUnlockPanel : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> nodePanels = new();
    private List<DonutNode> rootNodes = new();

    [SerializeField] private TextMeshProUGUI hasPerfectRecipeCount;
    [SerializeField] private RecipeDetailInfo recipeDetailInfo;
    void Start()
    {
        EventHub.Instance.RegisterEvent<RES.SetNodePanelViewEvnet>(SetNodeInfoRapper);
        EventHub.Instance.RegisterEvent<RES.UnlockNodeViewEvent>(UnlockNodeRapper);
        EventHub.Instance.RegisterEvent<RES.LockNodeViewEvent>(LockNodeRapper);
        EventHub.Instance.RegisterEvent<RES.CanUnlockViewEvent>(CanUnlockNodeRapper);
        EventHub.Instance.RegisterEvent<RES.RequestPerfectRecipeCount>(RefreshTextRapper);
        EventHub.Instance.RegisterEvent<RES.RequestRecipeDetailPopup>(RequestRecipeDetailPopup);

        OnPanel();
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RES.SetNodePanelViewEvnet>(SetNodeInfoRapper);
        EventHub.Instance?.UnRegisterEvent<RES.UnlockNodeViewEvent>(UnlockNodeRapper);
        EventHub.Instance?.UnRegisterEvent<RES.LockNodeViewEvent>(LockNodeRapper);
        EventHub.Instance?.UnRegisterEvent<RES.CanUnlockViewEvent>(CanUnlockNodeRapper);
        EventHub.Instance?.UnRegisterEvent<RES.RequestPerfectRecipeCount>(RefreshTextRapper);
        EventHub.Instance?.UnRegisterEvent<RES.RequestRecipeDetailPopup>(RequestRecipeDetailPopup);
    }

    private void SetNodeInfoRapper(RES.SetNodePanelViewEvnet evt) => SetNodeInfo(evt.node);
    private void UnlockNodeRapper(RES.UnlockNodeViewEvent evt) => UnlockNode(evt.node);
    private void LockNodeRapper(RES.LockNodeViewEvent evt) => LockNode(evt.node);
    private void CanUnlockNodeRapper(RES.CanUnlockViewEvent evt) => CanUnlockNode(evt.node);
    private void RefreshTextRapper(RES.RequestPerfectRecipeCount evt) => RefreshText();
    private void RequestRecipeDetailPopup(RES.RequestRecipeDetailPopup evt) => OpenDetailInfoPopup(evt.recipeUid);

    private void OnPanel()
    {
        Debug.Log("패널 활성화");
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        foreach (var panel in nodePanels)
        {
            rootNodes.Add(panel.transform.Find("RootNode").GetComponent<DonutNode>());
        }

        EventHub.Instance.RaiseEvent(new DCS.InitNodeDonutsControlEvent(rootNodes));
    }
    void OnEnable()
    {
        RefreshText();
    }
    private void IngredinetIconUpdate()
    {
        foreach (var panel in nodePanels)
        {
            DataManager.Instance.TryGetRecipeData(panel.transform.Find("RootNode").GetComponent<DonutNode>().nodeData.recipeId, out RecipeData recipeData);


            AddressableLoader.AssetLoadByPath<Sprite>(recipeData.ingredients[1].itemId, x =>
            {
                panel.transform.Find("Ingredient").GetComponent<Image>().sprite = x;
            }).Forget();
        }
    }
    /// <summary> 노드패널 UI 세팅 </summary>
    private void SetNodeInfo(DonutNode node)
    {
        DataManager.Instance.TryGetRecipeData(node.nodeData.recipeId, out RecipeData data);
        DataManager.Instance.TryGetDonutData(data.result.itemId, out var donutData);

        node.nodeNameText.text = data.recipeName;
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
        {
            node.icon.sprite = x;
        }).Forget();
        node.nodeBtn.onClick.RemoveAllListeners();
        node.nodeBtn.onClick.AddListener(() => OnClickNodeButton(node));
        IngredinetIconUpdate();
    }
    /// <summary> 해금이 된 노드에 대한 UI </summary>
    private void UnlockNode(DonutNode node)
    {
        node.nodeBtn.interactable = true;
        node.transform.Find("Frame Out/Slot").GetComponent<Image>().color = new Color(255f, 190f / 255f, 150f / 255f);
        node.isLockIcon.SetActive(false);
        node.nodeBtn.onClick.RemoveAllListeners();
    }
    /// <summary> 해금이 가능 한 노드에 대한 UI </summary>
    private void CanUnlockNode(DonutNode node)
    {
        node.isLockIcon.SetActive(true);
        node.transform.Find("NodeLock").GetComponent<Image>().color = new Color32(0, 233, 0, 200);
        node.nodeBtn.interactable = true;
    }

    /// <summary> 해금이 불가능한 노드에 대한 UI </summary>
    private void LockNode(DonutNode node)
    {
        node.nodeBtn.interactable = false;
        node.isLockIcon.SetActive(true);
    }

    private void OnClickNodeButton(DonutNode node)
    {
        EventHub.Instance.RaiseEvent(new DCS.InitUnlockRecipeControlEvent(node));
    }
    private void RefreshText()
    {
        hasPerfectRecipeCount.text = $"온전한 레시피 보유 개수 : {DataManager.Instance.PerfectRecipe}";
    }

    private void OpenDetailInfoPopup(string recipeUid)
    {
        recipeDetailInfo.SetRecipeDetail(recipeUid);
        recipeDetailInfo.gameObject.SetActive(true);
    }
}
