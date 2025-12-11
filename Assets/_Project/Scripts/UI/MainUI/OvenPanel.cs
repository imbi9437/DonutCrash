using _Project.Scripts.UI.MainUI;
using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class OvenPanel : MainPanel
{
    public override int PanelType =>(int)MainPanelType.Oven;
    
    [SerializeField] private Button CloseButton;

    public void OnEnable()
    {
        CloseButton.onClick.AddListener(Hide);
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_03, .5f));
    }

    public void OnDisable()
    {
        CloseButton.onClick.RemoveListener(Hide);
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
    }
}
