using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using AS = _Project.Scripts.EventStructs.AudioStruct;

public class MasterPanel : MonoBehaviour
{
    public Toggle createToggle;
    public Toggle mergeToggle;
    public Toggle ulockToggle;

    public GameObject createPanel;
    public GameObject mergePanel;
    public GameObject ulockPanel;

    void Start()
    {
        createToggle.onValueChanged.RemoveAllListeners();
        mergeToggle.onValueChanged.RemoveAllListeners();
        ulockToggle.onValueChanged.RemoveAllListeners();

        createToggle.onValueChanged.AddListener(ison => createPanel.SetActive(ison));
        createToggle.onValueChanged.AddListener(OnClick);
        mergeToggle.onValueChanged.AddListener(ison => mergePanel.SetActive(ison));
        mergeToggle.onValueChanged.AddListener(OnClick);
        ulockToggle.onValueChanged.AddListener(ison => ulockPanel.SetActive(ison));
        ulockToggle.onValueChanged.AddListener(OnClick);

    }
    private void OnClick(bool ison)
    {
        if (ison)
            EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.SFX_UI_03, 1f));
    }

    private void OnEnable()
    {
        mergeToggle.isOn = false;
        ulockToggle.isOn = false;
        createToggle.isOn = true;
    }
}
