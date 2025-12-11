using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DonutNode))]
public class RecipeHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private DonutNode _node;
    
    private CancellationTokenSource _cts;

    private void Start()
    {
        _node = GetComponent<DonutNode>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        CheckHold(1f, _cts.Token).Forget();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cts?.Cancel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cts?.Cancel();
    }
    
    private async UniTask CheckHold(float time, CancellationToken ct)
    {
        await UniTask.WaitForSeconds(time, cancellationToken: ct);
        EventHub.Instance.RaiseEvent(new RecipeEventStructs.RequestRecipeDetailPopup(_node.nodeData.recipeId));
    }
}
