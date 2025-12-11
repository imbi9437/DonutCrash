using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingButton : MonoBehaviour
{
    private Button _button;
    private Coroutine _coroutine;
    
    private void Start()
    {
        if (TryGetComponent(out _button) == false)
        {
            Debug.LogWarning($"버튼을 탐색할 수 없습니다.");
            return;
        }
        
        _button.onClick.AddListener(OnClickSettingButton);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClickSettingButton);
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void OnClickSettingButton()
    {
        _button.interactable = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(InteractiveBtnAfterSecondsCoroutine(1f));
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.SettingPopup, new SettingParam()));
    }

    private IEnumerator InteractiveBtnAfterSecondsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        _button.interactable = true;
    }
}
