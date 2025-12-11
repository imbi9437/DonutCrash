using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;

namespace _Project.Scripts.InGame.StatePattern
{
    public class TurnEndState : MonoState
    {
        private const float DonutSelectTimeout = 10f;
        public override int index => (int)BattleState.TurnEnd;

        private BattleController _battleController;
        
        private CancellationTokenSource _cts;


        public override void Initialize(MonoStateMachine machine)
        {
            base.Initialize(machine);
            _battleController = machine as BattleController;
        }

        protected override void OnEnable()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            EventHub.Instance?.RegisterEvent<IGS.RequestSpawnDonut>(OnRequestSpawnDonut);

            EventHub.Instance?.RaiseEvent(new IGS.TurnEndEvent(_battleController.GetTurnOwner()));

            
            Debug.Log("[EndTurnState] 활성화됨. 파괴 및 승리 조건 확인 시작.");

            if (GameEndOrContinueCheck())
            {
                return;
            }

            TurnEndSequence();
        }

        // 팝업이 띄워진 동안은 Update가 계속 실행됩니다.
        protected override void Update()
        {
            // _waitingForDonutSelection이 true일 때만 UI 입력 처리 등을 할 수 있습니다.
        }

        protected override void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.RequestSpawnDonut>(OnRequestSpawnDonut);
            Debug.Log($"<color=red>데이터 수정 완료 및 턴 변경</color>");

            _cts?.Cancel();
        }

        private void OnRequestSpawnDonut(IGS.RequestSpawnDonut evt) => InstantiateDonuts(evt.selector, evt.donut);

        // bool을 반환하도록 수정: 팝업이 필요한 경우 true 반환
        private bool CountDeathAndRespawn()
        {
            DeckData deck = _battleController.GetDeckData(_battleController.GetOpponentOwner());

            if (deck.fieldDonuts.Count > 2)
            {
                Debug.Log($"도넛 생성할 필요 없음");
                return false;
            }

            if (deck.waitingDonuts.Count < 1)
            {
                Debug.Log($"생성 가능한 도넛이 없음");
                return false;
            }

            List<DonutInstanceData> spawnableDonuts = new();
            for (int i = 0; i < Mathf.Clamp(deck.waitingDonuts.Count, 0, 2); i++)
            {
                spawnableDonuts.Add(deck.waitingDonuts[i]);
            }

            EventHub.Instance?.RaiseEvent(new PS.RequestSelectDonutSpawned(_battleController.GetOpponentOwner(), spawnableDonuts));
            return true;
        }

        private void TurnEndSequence()
        {
            if (CountDeathAndRespawn())
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                DonutSelectionTimeoutAsync(DonutSelectTimeout, _cts.Token).Forget();
                
                return;
            }

            CompleteTurnEndAndStartNext();
        }

        private async UniTask DonutSelectionTimeoutAsync(float time, CancellationToken ct)
        {
            float timeRemaining = time;
            DeckData opponentDeck = _battleController.GetDeckData(_battleController.GetOpponentOwner());

            while (timeRemaining > 0f && !ct.IsCancellationRequested)
            {
                timeRemaining -= 1;
                EventHub.Instance?.RaiseEvent(new PS.RequestChangeTurnTimer((int)time, (int)timeRemaining));
                await UniTask.Delay(1000, cancellationToken: ct);
            }

            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Debug.Log($"도넛 선택 제한시간 {time}초 초과. 팝업을 자동으로 닫습니다.");

            if (opponentDeck.waitingDonuts.Count > 0)
            {
                DonutInstanceData donutToSpawn = opponentDeck.waitingDonuts[0];

                EventHub.Instance?.RaiseEvent(new PS.RequestSpawnDonut(_battleController.GetOpponentOwner(), donutToSpawn));

                Debug.Log($"[TIMEOUT] 대체 도넛 ({donutToSpawn.uid}) 스폰 요청 완료");
            }
            else
            {
                Debug.Log($"[TIMEOUT] 대기 도넛 목록이 비어있어 대체 스폰 불가.");

               CompleteTurnEndAndStartNext();
            }
        }

        // 턴 종료 후 다음 턴 시작 로직을 별도 메서드로 분리 (팝업 완료 후 호출)
        private void CompleteTurnEndAndStartNext()
        {


            _battleController.ToggleTurnOwner(); // 턴 주인 변경 (다음 턴 주인을 설정)

            Debug.Log($"턴 종료 검사 종료. 턴 시작으로 전이. 다음 턴 주인: {_battleController.GetTurnOwner().ToString()}");
            _battleController.ChangeState(BattleState.TurnStart);
        }

        // bool을 반환하도록 수정: 게임이 끝났으면 true 반환
        private bool GameEndOrContinueCheck()
        {
            if (_battleController.CheckIfOwnerHasDied(InGameOwner.Left))
            {
                Debug.Log("왼쪽 플레이어 게임 오버. 오른쪽 플레이어 승리");
                EventHub.Instance.RaiseEvent(new IGS.CalledGameOver(InGameOwner.Left, true));
                return true;
            }
            else if (_battleController.CheckIfOwnerHasDied(InGameOwner.Right))
            {
                Debug.Log("오른쪽 플레이어 게임 오버. 왼쪽 플레이어 승리");
                EventHub.Instance.RaiseEvent(new IGS.CalledGameOver(InGameOwner.Right, true));
                return true;
            }
            else
            {
                Debug.Log("게임이 끝나지 않았습니다.");
                return false;
            }
        }

        private void InstantiateDonuts(InGameOwner owner, DonutInstanceData instanceData)
        {
            DeckData deck = _battleController.GetDeckData(owner);
            if (deck.waitingDonuts == null || deck.waitingDonuts.Count < 1)
            {
                Debug.Log($"{owner}의 도넛이 더이상 없습니다.");
                return;
            }
            
            List<GameObject> spawnZones = owner == InGameOwner.Left? _battleController.leftPlayerSpawnZones : _battleController.rightPlayerSpawnZones;

            GameObject targetZone = null;

            foreach (var zone in spawnZones)
            {
                Spawner spawner = zone.GetComponent<Spawner>();

                if(spawner != null && !spawner.IsOccupied)
                {
                    targetZone = zone;
                    break;
                }
            }

            if(targetZone == null)
            {
                Debug.LogWarning($"[{owner}] 스폰 가능한 빈 스포너가 없습니다. 스폰 실패.");

                CompleteTurnEndAndStartNext();
                return;
            }

            if (deck.waitingDonuts.Count < 1)
                return;
            if (deck.fieldDonuts.Count > 3)
                return;

            Vector3 spawnPos = targetZone.transform.position;

            DonutObject donut = PhotonManager
                .PunInstantiate(_battleController.donutObjectPrefab.gameObject, spawnPos, Quaternion.identity)
                .GetComponent<DonutObject>();

            donut.Initialize(instanceData, owner);
            _battleController.AddDonut(donut);

            TurnEndSequence();
        }
    }
}