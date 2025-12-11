using DG.Tweening;
using DonutClash.UI.GlobalUI;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalTwoButtonPopup : GlobalPanel
{
    public override GlobalPanelType GlobalPanelType => GlobalPanelType.TwoButtonPopup;
    
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private TMP_Text cancelText;
    
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;
    private Action onCancel;
    
    public override void Initialize() //내용 초기화 
    {
        titleText.text = "";
        contentText.text = "";
        
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    public override void Show(GlobalPanelParam param) => Show(param as TwoButtonParam);

    private void Show(TwoButtonParam param)
    {
        if(_sequence.IsActive()) _sequence.Kill();
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

    private void SetInteraction(TwoButtonParam param)
    {
        titleText.text = param.titleText;
        contentText.text = param.contentText;
        
        confirmText.text = param.confirmText;
        cancelText.text = param.cancelText;
        
        onConfirm = param.confirm;
        onCancel = param.cancel;
    }
    
    private void OnClickConfirmButton()
    {
        onConfirm?.Invoke();
        Hide();
    }
    
    private void OnClickCancelButton()
    {
        onCancel?.Invoke();
        Hide();
    }
}

