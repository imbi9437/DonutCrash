using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceArea : MonoBehaviour
{
    [SerializeField] private Button goldButton;
    [SerializeField] private Button cashButton;
    
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text cashText;

    [SerializeField] private Button optionButton;
    private Coroutine _optionCoroutine;


    private void OnEnable()
    {
        SetGold();
        SetCash();
        
        optionButton.onClick.AddListener(OpenSettingPanel);
        optionButton.interactable = true;
        
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserGoldEvent>(OnBroadCast);
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserCashEvent>(OnBroadCast);
    }

    private void OnDisable()
    {
        optionButton.onClick.RemoveListener(OpenSettingPanel);
        if(_optionCoroutine != null)
        {
            StopCoroutine(_optionCoroutine);
            _optionCoroutine = null;
        }
        
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserGoldEvent>(OnBroadCast);
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserCashEvent>(OnBroadCast);
    }

    private void OnBroadCast(DataStructs.BroadcastSetUserGoldEvent evt) => SetGold();
    private void OnBroadCast(DataStructs.BroadcastSetUserCashEvent evt) => SetCash();
    
    
    private void SetGold() => goldText.text = DataManager.Instance.UserGold.ToString();
    private void SetCash() => cashText.text = DataManager.Instance.UserCash.ToString();

    private void OpenSettingPanel()
    {
        optionButton.interactable = false;
        var param = new SettingParam();
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.SettingPopup, param));
        _optionCoroutine = StartCoroutine(InteractiveOptionAfterSecondsCoroutine(1f));
    }
    
    private IEnumerator InteractiveOptionAfterSecondsCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        optionButton.interactable = true;
    }
}
