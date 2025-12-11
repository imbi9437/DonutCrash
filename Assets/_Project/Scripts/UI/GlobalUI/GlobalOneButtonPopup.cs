using DG.Tweening;
using DonutClash.UI.GlobalUI;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalOneButtonPopup : GlobalPanel
{
    public override GlobalPanelType GlobalPanelType => GlobalPanelType.OneButtonPopup;
    
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text confirmButtonText;

    [SerializeField] private Button confirmButton;

    private Action onConfirm;
    
    public override void Initialize()
    {
        titleText.text = "";
        contentText.text = "";
        
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        
        confirmButton.onClick.AddListener(OnClickConfirmButton);
    }

    public override void Show(GlobalPanelParam param) => Show(param as OneButtonParam);

    private void Show(OneButtonParam param)
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence();
        
        gameObject.SetActive(true);
        SetInteraction(param);
        
        var fadeTween = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(1, animationTime).SetEase(animationEase);
        
        _sequence.Join(fadeTween);
        _sequence.Join(scaleTween);
        
        _sequence.onComplete += () => canvasGroup.interactable = true;
    }

    public override void Hide()
    {
        if(_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence();
        
        canvasGroup.interactable = false;
        
        var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(0, animationTime).SetEase(animationEase);
        
        _sequence.Join(fadeTween);
        _sequence.Join(scaleTween);
        
        _sequence.onComplete += () => Destroy(gameObject);
    }

    private void SetInteraction(OneButtonParam param)
    {
        titleText.text = param.titleText;
        contentText.text = param.contentText;
        
        confirmButtonText.text = param.confirmText;
        onConfirm = param.confirm;
    }
    
    private void OnClickConfirmButton()
    {
        onConfirm?.Invoke();
        Hide();
    }
}
