using TMPro;
using UnityEngine;
using IGS = _Project.Scripts.EventStructs.InGameStructs;

namespace _Project.Scripts.InGame.Sample
{
    public class SampleBattleStatePrinter : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            EventHub.Instance?.RegisterEvent<IGS.BattleStartEvent>(OnBattleStart);
            EventHub.Instance?.RegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
            EventHub.Instance?.RegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
            EventHub.Instance?.RegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
            EventHub.Instance?.RegisterEvent<IGS.BattleEndEvent>(OnBattleEnd);
        }

        private void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.BattleStartEvent>(OnBattleStart);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
            EventHub.Instance?.UnRegisterEvent<IGS.BattleEndEvent>(OnBattleEnd);
        }

        private void OnBattleStart(IGS.BattleStartEvent evt)
        {
            SetText($"Battle Start");
        }

        private void OnTurnStart(IGS.TurnStartEvent evt)
        {
            SetText($"Turn Start : {evt.turnOwner.ToString()}");
        }

        private void OnTurnRunning(IGS.TurnRunningEvent evt)
        {
            SetText($"Turn Running : {evt.turnOwner.ToString()}");
        }

        private void OnTurnEnd(IGS.TurnEndEvent evt)
        {
            SetText($"Turn End : {evt.turnOwner.ToString()}");
        }

        private void OnBattleEnd(IGS.BattleEndEvent evt)
        {
            SetText($"Battle End");
        }

        private void SetText(string message) => _text?.SetText(message);
    }
}
