using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Threading;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

namespace _Project.Scripts.InGame.StatePattern
{
    public class TurnRunningState : MonoState
    {
        public override int index => (int)BattleState.TurnRunning;

        private BattleController _battleController;
        
        private CancellationTokenSource _cts;

        public override void Initialize(MonoStateMachine machine)
        {
            base.Initialize(machine);
            _battleController = machine as BattleController;
        }

        protected override void OnEnable()
        {
            Debug.Log($"턴 진행 중 상태 돌입. 도넛 정지상태 감지 시작. 턴 주인 : {_battleController.GetTurnOwner()}");

            if (!PhotonNetwork.IsMasterClient)
                return;
            
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            CheckTurnEndCondition(_cts.Token).Forget();
            
            EventHub.Instance.RaiseEvent(new IGS.TurnRunningEvent(_battleController.GetTurnOwner()));
        }

        protected override void Update() { }

        protected override void OnDisable()
        {
            _cts?.Cancel();
            Debug.Log($"<color=red>알 움직임 멈춤, 따라서 상태 종료</color>");
        }

        private async UniTaskVoid CheckTurnEndCondition(CancellationToken ct)
        {
            await UniTask.Delay(600, cancellationToken: ct);
            
            int waiting = 0;
            while (!ct.IsCancellationRequested && waiting < 9)
            {
                await UniTask.Delay(300, cancellationToken: ct);
                bool isStop = true;
                foreach (DonutObject i in _battleController.GetDonutsByOwner(InGameOwner.Left))
                {
                    isStop &= i.IsStop();
                    if (!isStop)
                        break;
                }

                foreach (DonutObject i in _battleController.GetDonutsByOwner(InGameOwner.Right))
                {
                    isStop &= i.IsStop();
                    if (!isStop)
                        break;
                }

                if (isStop)
                    waiting++;
                else
                    waiting = 0;
            }

            Debug.Log($"모든 도넛 정지 감지 상태 전이 조건 충족");
            _battleController.ChangeState(BattleState.TurnEnd);
        }
    }
}
