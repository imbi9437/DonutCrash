using _Project.Scripts.UI.MainUI;
using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class BattlePanel : MainPanel
{
    public override int PanelType => (int)MainPanelType.Battle;
    
    [SerializeField] private BattleSubPanelUIController battleSubPanelUIController;
    
    [SerializeField] private Button closeButton;

    private void OnEnable()
    {
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_05, .5f));
    }

    private void OnDisable()
    {
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
    }

    public override void Initialize(UIController controller)
    {
        base.Initialize(controller);
        
        battleSubPanelUIController.Initialize();
        
        closeButton.onClick.AddListener(Hide);
    }

    public override void Show()
    {
        base.Show();
        
        battleSubPanelUIController.OpenPanel(BattleSubPanelType.Lobby);
    }
}
