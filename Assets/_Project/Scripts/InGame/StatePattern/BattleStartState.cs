using _Project.Scripts.EventStructs;
using _Project.Scripts.SkillSystem;
using System.Collections.Generic;
using UnityEngine;
using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;

namespace _Project.Scripts.InGame.StatePattern
{
    public class BattleStartState : MonoState
    {
        public override int index => (int)BattleState.Start;
        
        private BattleController _battleController;

        public override void Initialize(MonoStateMachine machine)
        {
            base.Initialize(machine);
            _battleController = machine as BattleController;
        }

        protected override void OnEnable()
        {
            EventHub.Instance?.RaiseEvent(new PS.RequestSetUserProfile());
            EventHub.Instance?.RaiseEvent(new IGS.BattleStartEvent());
            EventHub.Instance?.RaiseEvent(new IGS.TryRandomObstacle());
            
            if (_battleController.IsDeck())
                InstantiateDonuts();
            else
                Debug.LogError($"덱 데이터가 준비되지 않은 상태로 게임을 시작하려 합니다.");
            
            Debug.Log("<color=green> BattleStartState 진입 : 도넛 생성 시작<color>");
            
            EventHub.Instance?.RaiseEvent(new AudioStruct.PlayBackAudioEvent(AudioType.BGM_06, .5f));
        }
        
        protected override void Update()
        {
        }

        protected override void OnDisable()
        {
            Debug.Log($"<color=green>알까기 시작 상태 종료</color>");
        }

        private void InstantiateDonuts()
        {
            DeckData leftDeck = _battleController.GetDeckData(InGameOwner.Left);
            DeckData rightDeck = _battleController.GetDeckData(InGameOwner.Right);
            
            // 마녀 준비
            BakerObject leftBaker = new BakerObject();
            leftBaker.Initialize(leftDeck.baker, InGameOwner.Left);
            _battleController.AddBaker(leftBaker);
            
            BakerObject rightBaker = new BakerObject();
            rightBaker.Initialize(rightDeck.baker, InGameOwner.Right);
            _battleController.AddBaker(rightBaker);
            
            // 도넛 준비
            DonutObject prefab = _battleController.donutObjectPrefab;
            
            var leftSpawnZones = _battleController.leftPlayerSpawnZones;
            var rightSpawnZones = _battleController.rightPlayerSpawnZones;
            
            int i = 0;
            leftDeck.waitingDonuts.GetRange(0, 3).ForEach(x =>
            {
                Vector3 spawnPosition = leftSpawnZones[i].transform.position;
                DonutObject left = PhotonManager.PunInstantiate(prefab.gameObject, spawnPosition, Quaternion.identity).GetComponent<DonutObject>();
                left.Initialize(x, InGameOwner.Left);
                _battleController.AddDonut(left);
                i++;
            });
            
            Debug.Log($"호스트 대기도넛 : {leftDeck.waitingDonuts.Count}");
            Debug.Log($"호스트 출전도넛 : {leftDeck.fieldDonuts.Count}");

            i = 0;
            rightDeck.waitingDonuts.GetRange(0, 3).ForEach(x =>
            {
                Vector3 spawnPosition = rightSpawnZones[i].transform.position;
                DonutObject right = PhotonManager.PunInstantiate(prefab.gameObject, spawnPosition, Quaternion.identity).GetComponent<DonutObject>();
                right.Initialize(x, InGameOwner.Right);
                _battleController.AddDonut(right);
                i++;
            });
            
            Debug.Log($"게스트 대기도넛 : {rightDeck.waitingDonuts.Count}");
            Debug.Log($"게스트 출전도넛 : {rightDeck.fieldDonuts.Count}");
            
            // UI 초기 셋팅
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDeathCount(InGameOwner.Left, leftDeck.deathCount));
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDeathCount(InGameOwner.Right, rightDeck.deathCount));
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDonutList(InGameOwner.Left, leftDeck.fieldDonuts, leftDeck.waitingDonuts));
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDonutList(InGameOwner.Right, rightDeck.fieldDonuts, rightDeck.waitingDonuts));
            
            Debug.Log($"도넛 생성 완료. 미니 게임 시작");
            
            InGameOwner owner = Random.value > .5f ? InGameOwner.Left : InGameOwner.Right;
            _battleController.SetTurnOwner(owner);
            
            Debug.Log($"미니게임 완료 가정. {_battleController.GetTurnOwner().ToString()} 턴으로 시작");
            
            _battleController.ChangeState(BattleState.TurnStart);
        }
    }
}
