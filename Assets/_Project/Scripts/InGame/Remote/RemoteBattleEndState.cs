using _Project.Scripts.EventStructs;
using UnityEngine;

namespace _Project.Scripts.InGame.Remote
{
    public class RemoteBattleEndState : MonoState
    {
        public override int index => (int)BattleState.End;
        
        protected override void OnEnable()
        {
            Debug.Log($"<color=green>알까기 게임 종료</color>");
            
            EventHub.Instance.RaiseEvent(new InGameStructs.BattleEndEvent());
        }
        
        protected override void Update()
        {
            
        }

        protected override void OnDisable()
        {
            Debug.Log($"<color=green>알까기 종료 상태 끝! 다시 메인 씬으로 이동?</color>");
        }
    }
}
