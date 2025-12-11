using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : BakeSubPanel
{

    public override int PanelType => (int)BakePanelType.Upgrade;
    // [SerializeField] private GameObject upgradeProprobabilityPanel;
    // [SerializeField] private GameObject upgradeInfoPanel;
    // [SerializeField] private GameObject dountChoicePanel; 
    // [SerializeField] private Button chosenButton;
    // [SerializeField] private Button panelResetButton;
    //
    // private void Awake()
    // {
    //     chosenButton.onClick.AddListener(UpgradeButtonClick);
    //     panelResetButton.onClick.AddListener(ResetPanel);
    // }
    //
    // private void OnDestroy()
    // {
    //     chosenButton.onClick.RemoveListener(UpgradeButtonClick);
    //     panelResetButton.onClick.RemoveListener(ResetPanel);
    // }
    //
    // private void UpgradeButtonClick()
    // {
    //     upgradeProprobabilityPanel.SetActive(false);
    //     upgradeInfoPanel.SetActive(true);
    // }
    //
    // private void ResetPanel()
    // {
    //     upgradeProprobabilityPanel.SetActive(true);
    //     upgradeInfoPanel.SetActive(false);
    // }

}
