using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;

public enum BattleSubPanelType
{
    Lobby,
    Deck,
}
public class BattleSubPanelUIController : UIController
{
    
    public void OpenPanel(BattleSubPanelType type) => OpenPanel((int)type);
    public void ClosePanel(BattleSubPanelType type) => ClosePanel((int)type);
    
    public void ChangePanel(BattleSubPanelType type)
    {
        if (currentPanel.PanelType == (int)type) return;
        
        ClosePanel(currentPanel.PanelType);
        OpenPanel(type);
    }
    
    protected override IEnumerable GetChildPanel()
    {
        return GetComponentsInChildren<BattleSubPannel>(true);
    }
    
}
