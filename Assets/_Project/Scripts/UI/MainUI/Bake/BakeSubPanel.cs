using DG.Tweening;
using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BakeSubPanel : UIPanel
{
    protected BakeUIController bakeUIController;

    protected Sequence sequence; 
    public override void Initialize(UIController controller)
    {
        bakeUIController = controller as BakeUIController;
    }

    public override void Show()
    {
        // if(gameObject.activeSelf) return;
        gameObject.SetActive(true);
        
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();

        var dofade = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        sequence.Join(dofade);

        foreach (Transform pos in transform)
        {
            var doscale = pos.DOScale(1, animationTime).SetEase(animationEase);
            sequence.Join(doscale);
        }

        sequence.onComplete += () => canvasGroup.interactable = true;
    }

    public override void Hide()
    {
        // if(!gameObject.activeSelf) return;
        canvasGroup.interactable = false;
        
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        
        var dofade = canvasGroup.DOFade(0, 0).SetEase(animationEase);
        sequence.Join(dofade);

        foreach (Transform pos in transform)
        {
            var doscale = pos.DOScale(0, 0).SetEase(animationEase);
            sequence.Join(doscale);
        }
        
        sequence.onComplete += () => gameObject.SetActive(false);
    }
}
