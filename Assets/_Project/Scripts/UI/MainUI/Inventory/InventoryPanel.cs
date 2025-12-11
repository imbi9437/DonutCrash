using _Project.Scripts.EventStructs;
using _Project.Scripts.UI.MainUI;
using DonutClash.UI.GlobalUI;
using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class InventoryPanel : MainPanel
{
    public override int PanelType => (int)MainPanelType.Inventory;
    
    [SerializeField] private Button closeButton;
    
    private void OnEnable()
    {
        Debug.Log($"OnEnable");
        closeButton.onClick.AddListener(Hide);
        closeButton.onClick.AddListener(()=>EventHub.Instance.RaiseEvent(new UIStructs.CloseInventoryPanelEvent()));
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_04, .5f));
    }

    private void OnDisable()
    {
        Debug.Log($"OnDisable");
        closeButton.onClick.RemoveAllListeners();
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
    }
    
}
