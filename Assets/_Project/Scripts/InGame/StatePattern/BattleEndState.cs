using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System.Collections.Generic;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;
using FE = _Project.Scripts.EventStructs.FirebaseEvents;

namespace _Project.Scripts.InGame.StatePattern
{
    public class BattleEndState : MonoState
    {
        public override int index => (int)BattleState.End;

        private BattleController _battleController;

        private InGameOwner _winner;
        private int _deltaGold;
        private int _oppDeltaGold;

        public override void Initialize(MonoStateMachine machine)
        {
            base.Initialize(machine);
            _battleController = machine as BattleController;
        }


        protected override void OnEnable()
        {
            Debug.Log($"<color=green>알까기 게임 종료</color>");
            
            EventHub.Instance?.RegisterEvent<IGS.AckReward>(OnAckReward);
            EventHub.Instance?.RaiseEvent(new IGS.BattleEndEvent());
            
            _winner =  _battleController.CheckIfOwnerHasDied(InGameOwner.Right) ? InGameOwner.Left : InGameOwner.Right;

            CalcReward();
        }
        
        protected override void Update()
        {
            // Debug.Log($"<color=green>보상 산정 및 보상 지급 중</color>");
        }

        protected override void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.AckReward>(OnAckReward);
            Debug.Log($"<color=green>알까기 종료 상태 끝! 다시 메인 씬으로 이동?</color>");
        }

        private void OnAckReward(IGS.AckReward evt) => OpenPopup();

        private void CalcReward()
        {
            int gold = DataManager.Instance.UserGold;
            _deltaGold = _winner == InGameOwner.Left ? 500 : 100;
            _oppDeltaGold = _winner == InGameOwner.Left ? 100 : 500;
            
            List<BakerInstanceData> bakers = DataManager.Instance.Bakers;
            BakerInstanceData fieldBaker = DataManager.Instance.DeckData.baker;
            BakerInstanceData bakerData = bakers.Find(x => x.uid == fieldBaker.uid);
            bakerData.GetExp(_winner == InGameOwner.Left ? 3 : 2);
            
            EventHub.Instance?.RaiseEvent(new DataStructs.RequestSetBakerListEvent(bakers));
            EventHub.Instance?.RaiseEvent(new DataStructs.RequestSetGoldEvent(gold + _deltaGold));
            EventHub.Instance?.RaiseEvent(new FE.RequestLeaderboardUpdate(_winner == InGameOwner.Left ? 50 : 0, DataManager.Instance.UserNickName));
            EventHub.Instance?.RaiseEvent(new PS.RequestReward(_winner, _oppDeltaGold));
        }

        private void OpenPopup()
        {
            OneButtonParam param = new(_winner == InGameOwner.Left ? "승리" : "패배", $"{_deltaGold}를 획득하였습니다.", $"나가기", () => EventHub.Instance.RaiseEvent(new PS.GameEndEvent()));
            EventHub.Instance.RaiseEvent(new AudioStruct.PlayUiAudioEvent(_winner == InGameOwner.Left ? AudioType.JNG_02 : AudioType.JNG_03, 1f));
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
        }
    }
}
