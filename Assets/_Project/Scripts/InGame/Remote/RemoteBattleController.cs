using Photon.Pun;
using IGS = _Project.Scripts.EventStructs.InGameStructs;
using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts.InGame.Remote
{
    public class RemoteBattleController : MonoStateMachine
    {
        public Dictionary<InGameOwner, string> PlayerNicknames { get; private set; } = new Dictionary<InGameOwner, string>();
        public InGameOwner TurnOwner { get; private set; }

        public GuidelineRender GuidelineRenderer { get; private set; }
        public SplineController SplineController { get; private set; }
        
        public void SetupVisualizers(GuidelineRender guidelineRender, SplineController splineController)
        {
            GuidelineRenderer = guidelineRender;
            SplineController = splineController;
            Debug.Log("[RemoteBattleController] Visualizers set up.");
        }
        public InGameOwner GetTurnOwner()
        {
            return TurnOwner;
        }
        private void OnEnable()
        {
            EventHub.Instance?.RegisterEvent<IGS.BattleStartEvent>(OnChangeState);
            EventHub.Instance?.RegisterEvent<IGS.TurnStartEvent>(OnChangeState);
            EventHub.Instance?.RegisterEvent<IGS.TurnRunningEvent>(OnChangeState);
            EventHub.Instance?.RegisterEvent<IGS.TurnEndEvent>(OnChangeState);
            EventHub.Instance?.RegisterEvent<IGS.BattleEndEvent>(OnChangeState);
            InitializePlayerNicknames();
            
        }

        private void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.BattleStartEvent>(OnChangeState);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnStartEvent>(OnChangeState);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(OnChangeState);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(OnChangeState);
            EventHub.Instance?.UnRegisterEvent<IGS.BattleEndEvent>(OnChangeState);
        }
        
        public string GetNickname(InGameOwner owner)
        {
            if(PlayerNicknames.TryGetValue(owner, out string nickname))
            {
                return nickname;
            }
            return "Unknown Player";
        }

        private void InitializePlayerNicknames()
        {
            if(PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogError("포톤 룸이 없습니다.");
                return;
            }
            PlayerNicknames.Clear();

            foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
            {
                Photon.Realtime.Player player = playerEntry.Value;
                InGameOwner owner = player.IsMasterClient ? InGameOwner.Left : InGameOwner.Right;

                if (string.IsNullOrEmpty(player.NickName))
                {
                    Debug.LogError($"플레이어 {owner}의 NickName이 유효하지 않습니다.");
                    continue;
                }
                PlayerNicknames.Add(owner,player.NickName);
                Debug.Log($"[Nickame Init Success] {owner} : {player.NickName}");
            }
            if(PlayerNicknames.Count != 2)
            {
                Debug.LogError($"닉네임 초기화 실패 : 등록된 플레이어 수가 {PlayerNicknames.Count}명입니다.");
            }
        }

        private void OnChangeState(IGS.BattleStartEvent evt) => ChangeState(BattleState.Start);
        private void OnChangeState(IGS.TurnStartEvent evt)
        {
            TurnOwner = evt.turnOwner;
            ChangeState(BattleState.TurnStart);
        }
        private void OnChangeState(IGS.TurnRunningEvent evt)
        {
            TurnOwner = evt.turnOwner;
            ChangeState(BattleState.TurnRunning);
        }
        private void OnChangeState(IGS.TurnEndEvent evt)
        {
            TurnOwner = evt.turnOwner;
            ChangeState(BattleState.TurnEnd);
        }
        private void OnChangeState(IGS.BattleEndEvent evt) => ChangeState(BattleState.End);
        
        private void ChangeState(BattleState state) => ChangeState((int)state);
    }
}

