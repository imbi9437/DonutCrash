using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PreferencesSubPanelType
{
    ControlPanel,
    AlarmPanel,
    GameQualityPanel,
    GameInfoPanel
}
public class PreferencesUIController : UIController
{
    [SerializeField] private List<Button> buttons;
    
    private void OnEnable()
    {
        OpenDefaultPanel();
    }

    private void OnDisable()
    {
        CloseAllPanel();
    }

    private void Start()
    {
        SubPanelButtonClick();
    }

    private void OnDestroy()
    {
        UnSubPanelButtonClick();
    }

    private void SubPanelButtonClick()
    {
        IEnumerator<Button> buttonEnumerator = buttons.GetEnumerator();

        foreach (var panel in _panels)
        {
            if (buttonEnumerator.MoveNext() == false) break;
            
            buttonEnumerator.Current?.onClick.AddListener(() =>
            {
                if (currentPanel.PanelType != panel.Key)
                {
                    currentPanel?.Hide();
                }
                OpenPanel(panel.Key);
            });
        }
    }

    private void UnSubPanelButtonClick()
    {
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    protected override IEnumerable GetChildPanel()
    {
        return GetComponentsInChildren<PreferencesSubPanel>(true);
    }

   
}
