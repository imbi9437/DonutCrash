using _Project.Scripts.AudioSystem;
using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingVolumeItem : MonoBehaviour
{
    [SerializeField] private MixerType mixerType;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Toggle muteToggle;
    
    [SerializeField] private Image muteIcon;
    [SerializeField] private Image unmuteIcon;
    
    
    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        muteToggle.onValueChanged.AddListener(MuteToggleChanged);

        
        float volume = AudioManager.Instance.GetVolume(mixerType);
        
        Debug.Log($"========Get Volume : {mixerType} {volume}");
        
        slider.SetValueWithoutNotify(volume);
        volumeText.text = $"{volume:P0}";
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        muteToggle.onValueChanged.RemoveListener(MuteToggleChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        volumeText.text = $"{value:P0}";
        
        MuteToggleChangedWithoutNotify(value <= 0f);
        EventHub.Instance.RaiseEvent(new AudioStruct.ChangeMixerVolume(mixerType, value));
    }
    
    private void OnSliderValueChangedWithoutNotify(float value)
    {
        slider.SetValueWithoutNotify(value);
        
        volumeText.text = $"{value:P0}";
    }

    private void MuteToggleChanged(bool isOn)
    {
        float volume = isOn ? 0f : 0.5f;
        
        muteIcon.enabled = isOn;
        unmuteIcon.enabled = !isOn;
        
        OnSliderValueChangedWithoutNotify(volume);
    }

    private void MuteToggleChangedWithoutNotify(bool isOn)
    {
        muteToggle.SetIsOnWithoutNotify(isOn);
        
        muteIcon.enabled = isOn;
        unmuteIcon.enabled = !isOn;
    }
}