using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class CustomToggle : Toggle
{
    [SerializeField] private AudioType type = AudioType.UISample;
    [SerializeField, Range(0, 1)] private float volume = 1f;

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        DefaultOnClickEvent();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        transform.DOScale(1.1f, 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        transform.DOScale(1f, 0.1f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        DefaultOnClickEvent();
    }

    private void DefaultOnClickEvent()
    {
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(type, volume));
        transform.DOScale(0.9f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
