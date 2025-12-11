using System;
using UnityEngine;
using UnityEngine.UI;

using PS = _Project.Scripts.EventStructs.PhotonStructs;

[RequireComponent(typeof(Button))]
public class BattleStartButton : MonoBehaviour
{
    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickMatchMakingButton);
        
        EventHub.Instance?.RegisterEvent<PS.StopMatchMakingEvent>(OnStopMatchMaking);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<PS.StopMatchMakingEvent>(OnStopMatchMaking);
    }

    private void OnClickMatchMakingButton()
    {
        _button.interactable = false;
        EventHub.Instance?.RaiseEvent(new PS.StartMatchMakingEvent());
    }

    private void OnStopMatchMaking(PS.StopMatchMakingEvent evt)
    {
        _button.interactable = true;
    }
}
