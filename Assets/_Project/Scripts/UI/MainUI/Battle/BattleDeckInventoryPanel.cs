using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDeckInventoryPanel : MonoBehaviour
{
    [SerializeField]private Button CloseButton;
    private void OnEnable()
    {
        CloseButton.onClick.AddListener(()=>
        {
            EventHub.Instance?.RaiseEvent(new UIStructs.CloseInventoryPanelEvent());
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        CloseButton.onClick.RemoveAllListeners();
    }
    
}
