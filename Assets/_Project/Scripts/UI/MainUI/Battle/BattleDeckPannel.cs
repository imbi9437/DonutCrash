using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;

public class BattleDeckPannel : BattleSubPannel
{
    public override int PanelType => (int)BattleSubPanelType.Deck;
    
    [SerializeField] private List<DonutSlotUI> donutSlots;
    [SerializeField] private BakerSlotUI bakerSlot;

    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;

    private void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
        saveButton.onClick.RemoveListener(OnSaveButtonClick);
    }

    public override void Initialize(UIController controller)
    {
        base.Initialize(controller);
        
        for (int i = 0; i < donutSlots.Count; i++)
        {
            int index = i;
            donutSlots[i].Initialize(index);
        }
        
        bakerSlot.Initialize();
    }

    private void OnBackButtonClick()
    {
        BattleSubPanelUIController.ChangePanel(BattleSubPanelType.Lobby);
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestNotSaveDeckEvent());
    }
    private void OnSaveButtonClick()
    {
        var param = new TwoButtonParam("저장", "저장하시겠습니까?", "예", "아니요");
        param.confirm = RequestSaveDeck;
        
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
    }

    private void RequestSaveDeck()
    {
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestSaveCurrentDeckEvent());
        BattleSubPanelUIController.ChangePanel(BattleSubPanelType.Lobby);
    }
}
