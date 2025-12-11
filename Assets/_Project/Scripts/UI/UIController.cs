using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// UI 컨트롤러 최상위 객체, 각종 컨드롤러의 경우 해당 객체를 상속받아서 사용
/// </summary>
public abstract class UIController : MonoBehaviour
{
    protected readonly Dictionary<int, UIPanel> _panels = new();
    protected UIPanel currentPanel;

    protected virtual void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        var panels = GetChildPanel();

        foreach (UIPanel panel in panels)
        {
            _panels.TryAdd(panel.PanelType, panel);
            panel.Initialize(this);
            panel.gameObject.SetActive(false);
        }
    }
    
    protected void OpenPanel(int panelType)
    {
        if (_panels.TryGetValue(panelType, out var panel) == false)
        {
            Debug.Log("<color=red>해당 타입에 맞는 패널이 존재하지 않습니다.</color>");
            return;
        }
        panel.Show();
        currentPanel = panel;
    }

    protected void ClosePanel(int panelType)
    {
        if (_panels.TryGetValue(panelType, out var panel) == false)
        {
            Debug.Log($"<color=red>해당 타입에 맞는 패널이 존재하지 않습니다.</color>");
            return;
        }
        
        panel.Hide();
        currentPanel = null;
    }

    protected void CloseAllPanel()
    {
        foreach (var panel in _panels.Values)
        {
            panel.Hide();
        }

        currentPanel = null;
    }
    
    protected void OpenDefaultPanel()
    {
        if (_panels.Count <= 0)
        {
            Debug.Log($"<color=red>[{GetType()}]현재 해당 컨트롤러의 패널이 존재하지 않습니다.</color>");
            return;
        }
        
        int index = 0;
        if (_panels.ContainsKey(0) == false) index = _panels.Values.First(s => s != null).PanelType;
        
        
        OpenPanel(index);
    }

    protected abstract IEnumerable GetChildPanel();
}
