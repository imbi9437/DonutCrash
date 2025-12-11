using DG.Tweening;
using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public abstract class BattleSubPannel : UIPanel
{
    protected BattleSubPanelUIController BattleSubPanelUIController;
    
    public override void Initialize(UIController controller)
    {
        BattleSubPanelUIController = controller as BattleSubPanelUIController;
    }

    public override void Show()
    {
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        
        gameObject.SetActive(true);
        
        var dofade = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        sequence.Join(dofade);

        foreach (Transform Pos in transform)
        {
            var doscale = Pos.DOScale(1 , animationTime).SetEase(animationEase);
            sequence.Join(doscale);
        }
        

        sequence.onComplete = () => canvasGroup.interactable = true;
    }

    public override void Hide()
    {
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        canvasGroup.interactable = false;
        var dofade = canvasGroup.DOFade(0, 0).SetEase(animationEase);
        sequence.Join(dofade);

        foreach (Transform Pos in transform)
        {
            var doscale = Pos.DOScale(0 , 0).SetEase(animationEase);
            sequence.Join(doscale);
        }
        
        sequence.onComplete = () => gameObject.SetActive(false);
    }
}
