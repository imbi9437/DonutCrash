using _Project.Scripts.EventStructs;
using _Project.Scripts.UI.MainUI;
using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class ShopPanel : MainPanel
{

    public override int PanelType => (int)MainPanelType.Shop;

    [SerializeField]private ShopCategoryUIController shopCategoryUIController;
    [SerializeField] Button closeButton;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(Hide);
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_02, .5f));
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(Hide);
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
    }

    public override void Show()
    {
        base.Show();
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenShopCategoryEvent(0));
    }

    public override void Initialize(UIController controller)
    {
        base.Initialize(controller);
        
        shopCategoryUIController.Initialize();
    }
}

