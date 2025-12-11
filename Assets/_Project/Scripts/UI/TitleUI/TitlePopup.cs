using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class TitlePopup : UIPanel
{
    protected TitleUIController titleUIController;
    
    public override void Initialize(UIController controller)
    {
        titleUIController = controller as TitleUIController;
        canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
    }
    
    public override void Show()
    {
        if (sequence.IsActive()) sequence.Kill();
        sequence = DOTween.Sequence();
        
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        
        var fadeTween = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        var popTween = transform.DOScale(1, animationTime).SetEase(animationEase);
        sequence.Join(fadeTween);
        sequence.Join(popTween);
    }

    public override void Hide()
    {
        if (sequence.IsActive()) sequence.Kill();
        sequence = DOTween.Sequence();
        
        canvasGroup.interactable = false;
        
        var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
        var popTween = transform.DOScale(0, animationTime).SetEase(animationEase);
        sequence.Join(fadeTween);
        sequence.Join(popTween);
        
        sequence.onComplete += () => gameObject.SetActive(false);
    }
}
