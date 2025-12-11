using DG.Tweening;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalImageSelectPopup : GlobalPanel
{
    public override GlobalPanelType GlobalPanelType => GlobalPanelType.ImageSelectPopup;

    [SerializeField] private TMP_Text titleText;
    
    [SerializeField] private TMP_Text confirmButtonText;
    [SerializeField] private Button confirmButton;
    
    [SerializeField] private Button cancelButton;


    [SerializeField] private Transform parent;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private ImageSelectToggle togglePrefab;
    
    private Sequence _sequence;

    private Action<string> onConfirm;
    private string selectPath;
    
    public override void Initialize()
    {
        titleText.text = "";
        confirmButtonText.text = "";
        
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        cancelButton.onClick.AddListener(Hide);
    }

    public override void Show(GlobalPanelParam param) => Show(param as ImageSelectParam);

    private void Show(ImageSelectParam param)
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
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence();

        canvasGroup.interactable = false;
        
        var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(0, animationTime).SetEase(animationEase);
        
        _sequence.Join(fadeTween);
        _sequence.Join(scaleTween);
        
        _sequence.onComplete += () => Destroy(gameObject);
    }

    private void SetInteraction(ImageSelectParam param)
    {
        titleText.text = param.titleText;
        confirmButtonText.text = param.confirmText;

        confirmButtonText.text = param.confirmText;
        onConfirm = param.confirm;

        selectPath = param.currentPath;

        foreach (var path in param.spritePath)
        {
            var toggle = Instantiate(togglePrefab, parent);
            toggle.Initialize(path, toggleGroup, OnValueChanged, path == selectPath);
            continue;
            
            
            void OnValueChanged(bool isOn)
            {
                if (isOn) selectPath = path;
            }
        }

        
    }
    
    private void OnClickConfirmButton()
    {
        onConfirm?.Invoke(selectPath);
        Hide();
    }
}
