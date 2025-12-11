using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;

namespace _Project.Scripts.UI.MainUI
{
    public enum MainPanelType
    {
        Shop,
        Inventory,
        Oven,
        Battle,
    }
    
    public class MainUIController : UIController
    {
        [Header("패널버튼 UI")] 
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button ovenButton;
        [SerializeField] private Button battleButton;

        private void Start()
        {
            shopButton.onClick.AddListener(() => OpenPanel((int)MainPanelType.Shop));
            inventoryButton.onClick.AddListener(() =>
            {
                OpenPanel((int)MainPanelType.Inventory);
                EventHub.Instance?.RaiseEvent(new US.OpenInventoryPanelEvent(InventoryButtonType.Combine));
            });
            ovenButton.onClick.AddListener(() => OpenPanel((int)MainPanelType.Oven));
            battleButton.onClick.AddListener(() => OpenPanel((int)MainPanelType.Battle));
            
            EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
        }

        private void OnDestroy()
        {
            shopButton.onClick.RemoveAllListeners();
            inventoryButton.onClick.RemoveAllListeners();
            ovenButton.onClick.RemoveAllListeners();
            battleButton.onClick.RemoveAllListeners();
        }
        
        private void OpenPanel(MainPanelType type) => OpenPanel((int)type);
        private void ClosePanel(MainPanelType type) => ClosePanel((int)type);

        protected override IEnumerable GetChildPanel()
        {
            return GetComponentsInChildren<MainPanel>(true);
        }
    }
}