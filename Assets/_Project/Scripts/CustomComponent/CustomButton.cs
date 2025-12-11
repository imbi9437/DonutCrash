using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class CustomButton : Button
{
    [SerializeField] private AudioType type = AudioType.UISample;
    [SerializeField, Range(0, 1)] private float volume = 1f;

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        DefaultOnClickEvent();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        DefaultOnClickEvent();
    }
    
    private void DefaultOnClickEvent()
    {
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(type, volume));
        transform.DOKill(true);
        transform.DOScale(0.9f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
