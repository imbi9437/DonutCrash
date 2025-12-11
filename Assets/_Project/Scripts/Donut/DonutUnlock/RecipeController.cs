using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System.Collections.Generic;
using UnityEngine;
using DS = _Project.Scripts.EventStructs.DataStructs;

public class RecipeController : MonoBehaviour
{
    private readonly List<string> _canUnlockRecipes = new(); //해금 가능한 레시피 계산
    private List<string> _unlockedRecipes = new();  //유저 해금 정보 캐싱

    private List<RecipeNodeData> _recipeNodeTable;  //모든 레시피 노드 캐싱
    private int _perfectRecipe;

    private bool _recipesDirty;
    private bool _perfectRecipeDirty;

    private void Start()
    {
        Init();

        EventHub.Instance.RegisterEvent<DonutControlStructs.OnClickUnlockControlEvent>(UnlockRecipeNodeRapper);
        EventHub.Instance.RegisterEvent<DonutControlStructs.InitUnlockRecipeControlEvent>(CanUnlockNodeClickRapper);
        EventHub.Instance.RegisterEvent<DonutControlStructs.InitNodeDonutsControlEvent>(SetDonutNodeRapper);

        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserPerfectRecipeEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserUnlockRecipeEvent>(OnBroadcast);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DonutControlStructs.OnClickUnlockControlEvent>(UnlockRecipeNodeRapper);
        EventHub.Instance?.UnRegisterEvent<DonutControlStructs.InitUnlockRecipeControlEvent>(CanUnlockNodeClickRapper);
        EventHub.Instance?.UnRegisterEvent<DonutControlStructs.InitNodeDonutsControlEvent>(SetDonutNodeRapper);

        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserPerfectRecipeEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserUnlockRecipeEvent>(OnBroadcast);
    }


    private void UnlockRecipeNodeRapper(DonutControlStructs.OnClickUnlockControlEvent evt) => UnlockRecipe(evt.node);
    private void SetDonutNodeRapper(DonutControlStructs.InitNodeDonutsControlEvent evt) => SetDonutNode(evt.rootNodes);
    private void CanUnlockNodeClickRapper(DonutControlStructs.InitUnlockRecipeControlEvent evt) => CanUnlockNodeClick(evt.node);

    private void OnBroadcast(DS.BroadcastSetUserPerfectRecipeEvent evt) => _perfectRecipe = DataManager.Instance.PerfectRecipe;
    private void OnBroadcast(DS.BroadcastSetUserUnlockRecipeEvent evt) => _unlockedRecipes = DataManager.Instance.UnlockRecipe;

    /// <summary> 시작 시 노드들을 세팅하는 기능 <summary>
    private void SetDonutNode(List<DonutNode> rootNodes)
    {
        Debug.Log("노드세팅");
        if (rootNodes == null) return;
        for (int i = 0; i < rootNodes.Count; i++)
        {
            rootNodes[i].nodeData = _recipeNodeTable.Find(x => x.uid == (30200001 + i).ToString());
        }
        foreach (var r in rootNodes)
        {
            SetNode(r.nodeData.uid, r);
        }
    }
    private void SetNode(string nodeID, DonutNode node)
    {
        if (node.isSet) return;
        node.isSet = true;

        DataManager.Instance.TryGetRecipeNodeData(nodeID, out var data);

        node.nodeData = data;
        EventHub.Instance.RaiseEvent(new RecipeEventStructs.SetNodePanelViewEvnet(node));

        CanUnlockRecipe(node);

        if (node.nextNodes.Count > 0 && node.nextNodes != null)
        {
            for (int i = 0; i < node.nextNodes.Count; i++)
            {
                SetNode(node.nodeData.nextNodes[i], node.nextNodes[i]);
            }
        }
        return;
    }

    /// <summary> 레시피 해금 여부 판단 후 해금 하는 함수 <summary>
    private void CanUnlockNodeClick(DonutNode node)
    {
        if (node == null) return;
        DataManager.Instance.TryGetRecipeNodeData(node.nodeData.uid, out RecipeNodeData data);
        if (data == null || !_canUnlockRecipes.Contains(data.uid) || _unlockedRecipes.Contains(data.uid))
            return;
        UnlockRecipe(node);
    }

    /// <summary> Controller 초기화 함수 <summary>
    private void Init()
    {
        _unlockedRecipes.Clear();
        _canUnlockRecipes.Clear();

        //DataManager로 부터 유저의 레시피 정보 받아와 캐싱
        _unlockedRecipes = DataManager.Instance.UnlockRecipe;
        _perfectRecipe = DataManager.Instance.PerfectRecipe;
        //DataManager로 부터 모든 레시피 노드 정보 받아와 캐싱
        _recipeNodeTable = DataManager.Instance.GetRecipeNodeTable();
        //이전 노드가 해금된 레시피 노드를 모두 찾아 List에 추가하는 로직
        foreach (var node in _recipeNodeTable)
        {
            DataManager.Instance.TryGetRecipeNodeData(node.uid, out var data);
            if (!_unlockedRecipes.Contains(data.recipeId)) continue;

            foreach (var recipe in node.nextNodes)
            {
                _canUnlockRecipes.Add(recipe);
            }
        }
        Debug.Log("초기화 끝 ");
    }

    /// <summary> 노드 전체의 레시피 해금이 가능한 지 판단하는 함수 </summary>
    private bool CanUnlockRecipe(DonutNode node)
    {
        //해당 노드의 레시피가 해금되었을 경우 Return false
        if (_unlockedRecipes.Contains(node.nodeData.recipeId))
        {
            Debug.Log("이미 해금되어 있음");
            EventHub.Instance.RaiseEvent(new RecipeEventStructs.UnlockNodeViewEvent(node));
            return false;
        }

        //해당 노드의 이전단계 레시피가 해금되지 않았을 경우 Return false

        if (!_canUnlockRecipes.Contains(node.nodeData.uid))
        {
            Debug.Log("이전 노드가 해금되어있지 않음");
            EventHub.Instance.RaiseEvent(new RecipeEventStructs.LockNodeViewEvent(node));
            return false;
        }

        EventHub.Instance.RaiseEvent(new RecipeEventStructs.CanUnlockViewEvent(node));
        return true;
    }

    /// <summary> 레시피 해금 함수 </summary>
    private void UnlockRecipe(DonutNode node)
    {
        if (!DataManager.Instance.TryGetRecipeNodeData(node.nodeData.uid, out RecipeNodeData data)) return;
        DataManager.Instance.TryGetRecipeData(node.nodeData.recipeId, out RecipeData reicpeData);
        if (_perfectRecipe < 1)
        {
            OneButtonParam param = new OneButtonParam("해금 실패", $"온전한 레시피가 부족하여 해금할 수 없습니다.", "확인");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
            return;
        }
        if (_canUnlockRecipes.Contains(data.uid) && !_unlockedRecipes.Contains(data.uid))
        {
            TwoButtonParam param = new TwoButtonParam("도넛 해금", $"온전한 레시피 1개를 소모하여 <color=red>{reicpeData.recipeName}</color>의 레시피를 해금하시겠습니까 ? <br><br>온전한 레시피 보유 개수 : {_perfectRecipe}", "예", "아니요");
            param.confirm = () => OnClickUnlockConfirm(node, data);
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
        }
    }
    private void OnClickUnlockConfirm(DonutNode node, RecipeNodeData data)
    {
        //레시피 추가가 됐다면 해금가능에서 삭제
        _unlockedRecipes.Add(data.recipeId);
        _perfectRecipe--;
        _canUnlockRecipes.Remove(data.uid);
        foreach (var n in node.nodeData.nextNodes)
        {
            _canUnlockRecipes.Add(n);
        }
        foreach (var r in node.nextNodes)
        {
            EventHub.Instance.RaiseEvent(new RecipeEventStructs.CanUnlockViewEvent(r));
        }
        
        EventHub.Instance.RaiseEvent(new RecipeEventStructs.UnlockNodeViewEvent(node));;
        _recipesDirty = true;
        _perfectRecipeDirty = true;
        UpdateData();
        EventHub.Instance.RaiseEvent(new RecipeEventStructs.RequestPerfectRecipeCount());
    }

    private void UpdateData()
    {
        if (_recipesDirty) { EventHub.Instance.RaiseEvent(new DS.RequestSetUnlockRecipeEvent(_unlockedRecipes)); }
        if (_perfectRecipeDirty) { EventHub.Instance.RaiseEvent(new DS.RequestSetPerfectRecipeEvent(_perfectRecipe)); }

        ResetDirty();
    }
    private void ResetDirty()
    {
        _recipesDirty = false;
        _perfectRecipeDirty = false;
    }
}
