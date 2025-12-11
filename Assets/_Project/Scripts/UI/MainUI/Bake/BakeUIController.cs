using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BakePanelType
{
    Oven,
    Upgrade,
    UnLock
}


public class BakeUIController : UIController
{
    [SerializeField] private List<Button>  _buttons = new();

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
        IEnumerator<Button> buttonEnumerator = _buttons.GetEnumerator();

        foreach (var panel in _panels)
        {
            if (buttonEnumerator.MoveNext() == false) break;
            
            buttonEnumerator.Current?.onClick.AddListener(() =>
            {
                if (currentPanel?.PanelType != panel.Key)
                {
                    currentPanel?.Hide();
                }
                OpenPanel(panel.Key);
            });
        }
    }
    
    private void UnSubPanelButtonClick()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }


    protected override IEnumerable GetChildPanel()
    {
        return GetComponentsInChildren<BakeSubPanel>(true);
    }
}
