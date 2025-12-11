using DG.Tweening;
using DonutClash.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public abstract class PreferencesSubPanel : UIPanel
{
    protected PreferencesUIController UIController;
    
    protected Sequence sequence;

    public override void Initialize(UIController controller)
    {
        UIController = controller as PreferencesUIController;
    }

    public override void Show()
    {
        
        gameObject.SetActive(true);
        
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();

        var dofade = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        sequence.Join(dofade);

        foreach (Transform Pos in transform)
        {
            var doscale = Pos.DOScale(1 , animationTime).SetEase(animationEase);
            sequence.Join(doscale);
        }
        

        sequence.onComplete += () => canvasGroup.interactable = true;
    }

    public override void Hide()
    {
        
        canvasGroup.interactable = false;
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
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
