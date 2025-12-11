
using DG.Tweening;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettingPopup : GlobalPanel
{
    public override GlobalPanelType GlobalPanelType => GlobalPanelType.SettingPopup;

    [SerializeField] private List<ToggleConnector> toggleList;
    
    private Sequence _sequence;
    
    [SerializeField] private Button closeButton;
    
    public override void Initialize()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
        
        closeButton.onClick.AddListener(Hide);
    }

    public override void Show(GlobalPanelParam param) => Show(param as SettingParam);
    
    private void Show(SettingParam param)
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

    private void SetInteraction(SettingParam param)
    {
        foreach (var connector in toggleList)
        {
            connector.toggle.onValueChanged.AddListener(connector.panel.SetActive);
        }

        if (toggleList.Count > 0)
        {
            toggleList[0].toggle.isOn = true;
            toggleList[0].panel.SetActive(true);
        }
    }
}

[Serializable]
public class ToggleConnector
{
    public Toggle toggle;
    public GameObject panel;
}