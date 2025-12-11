using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class TurnTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image fill;

    private void Start()
    {
        EventHub.Instance?.RegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
        EventHub.Instance?.RegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
        
        EventHub.Instance?.RegisterEvent<IGS.ChangeTurnTimer>(OnChangeTurnTimer);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
        EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
        
        EventHub.Instance?.UnRegisterEvent<IGS.ChangeTurnTimer>(OnChangeTurnTimer);
    }

    private void OnTurnStart(IGS.TurnStartEvent evt) => Setup();
    private void OnTurnRunning(IGS.TurnRunningEvent evt) => Setup();

    private void OnChangeTurnTimer(IGS.ChangeTurnTimer evt) => Setup(evt.max, evt.current);

    private void Setup(float max, float current)
    {
        float ratio = Mathf.Clamp01((max - current) / max);

        text.text = current.ToString("00");
        fill.DOKill();
        fill.DOFillAmount(ratio, .2f);
    }

    private void Setup()
    {
        text.text = "--";
        fill.DOKill();
        fill.DOFillAmount(0, .2f);
    }
}
