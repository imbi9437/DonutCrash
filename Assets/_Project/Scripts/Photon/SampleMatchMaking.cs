using _Project.Scripts.EventStructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PS = _Project.Scripts.EventStructs.PhotonStructs;

public class SampleMatchMaking : MonoBehaviour
{
    private Button _button;
    
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickMatchMakingButton);
    }

    private void OnClickMatchMakingButton()
    {
        EventHub.Instance?.RaiseEvent(new PS.StartMatchMakingEvent());
    }
}
