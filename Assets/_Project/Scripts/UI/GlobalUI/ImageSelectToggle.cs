using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageSelectToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image image;
    
    public void Initialize(string spritePath, ToggleGroup group, UnityAction<bool> onValueChanged, bool isOn)
    {
        toggle.group = group;
        toggle.isOn = isOn;
        toggle.onValueChanged.AddListener(onValueChanged);

        AddressableLoader.AssetLoadByPath<Sprite>(spritePath, x => image.sprite = x).Forget();
    }
}
