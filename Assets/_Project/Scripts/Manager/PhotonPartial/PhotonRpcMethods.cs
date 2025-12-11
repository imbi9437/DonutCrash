using _Project.Scripts.EffectSystem;
using _Project.Scripts.EventStructs;
using _Project.Scripts.InGame;
using Cysharp.Threading.Tasks;
using DonutClash.UI.GlobalUI;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

using EVT = _Project.Scripts.EventStructs.ChangeSceneStructs;
using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;
using ES = _Project.Scripts.EventStructs.EffectStructs;
using DNC = _Project.Scripts.EventStructs.DamageNumberStructs;
using FE = _Project.Scripts.EventStructs.FirebaseEvents;

public partial class PhotonManager
{
    [PunRPC]
    private void SceneChangeRPC(int sceneIndex)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        EventHub.Instance?.RaiseEvent(new EVT.RequestChangeSceneEvent(index: sceneIndex));
        
        if (PhotonNetwork.IsMasterClient)
            CheckPlayerLoad(sceneIndex).Forget();
    }

    [PunRPC]
    private void ChangeBattleStateRPC(int battleState, int turnOwner)
    {
        BattleState bs = (BattleState)battleState;
        switch (bs)
        {
            case BattleState.Start:
                EventHub.Instance?.RaiseEvent(new IGS.BattleStartEvent());
                break;
            case BattleState.TurnStart:
                EventHub.Instance?.RaiseEvent(new IGS.TurnStartEvent(){turnOwner = (InGameOwner)turnOwner});
                break;
            case BattleState.TurnRunning:
                EventHub.Instance?.RaiseEvent(new IGS.TurnRunningEvent(){turnOwner = (InGameOwner)turnOwner});
                break;
            case BattleState.TurnEnd:
                EventHub.Instance?.RaiseEvent(new IGS.TurnEndEvent(){turnOwner = (InGameOwner)turnOwner});
                break;
            case BattleState.End:
                EventHub.Instance?.RaiseEvent(new IGS.BattleEndEvent());
                break;
        }
    }

    [PunRPC]
    private void ShotDonutRPC(string uid, Vector2 force)
    {
        Debug.Log($"도넛 발사 RPC 수령");
        EventHub.Instance?.RaiseEvent(new IGS.ShotDonutEvent(uid, force));
    }

    [PunRPC]
    private void SetDeathCountRPC(int changer, int deathCount)
    {
        EventHub.Instance.RaiseEvent(new IGS.ChangeDeathCount((InGameOwner)changer, deathCount));
    }

    // TODO : 구현이 우선이라 중복된 데이터 또한 전송. 리팩토링 사항 확인후 수정
    [PunRPC]
    private void SetDonutListRPC(int owner, string json00, string json01)
    {
        List<DonutInstanceData> stagedDonuts = json00.DeSerializeObject<List<DonutInstanceData>>();
        List<DonutInstanceData> unstagedDonuts = json01.DeSerializeObject<List<DonutInstanceData>>();
        EventHub.Instance?.RaiseEvent(new IGS.ChangeDonutList((InGameOwner)owner, stagedDonuts, unstagedDonuts));
    }

    [PunRPC]
    private void SetTurnTimerRPC(int max, int current)
    {
        EventHub.Instance?.RaiseEvent(new IGS.ChangeTurnTimer(max, current));
    }

    [PunRPC]
    private void CompleteSelectDonutRPC()
    {
        EventHub.Instance?.RaiseEvent(new IGS.CompleteSelectDonut());
    }

    [PunRPC]
    private void RequestSelectDonutRPC(int selector, string json)
    {
        if ((PhotonNetwork.IsMasterClient ? InGameOwner.Left : InGameOwner.Right) != (InGameOwner)selector)
            return;
        
        List<DonutInstanceData> donutsData = json.DeSerializeObject<List<DonutInstanceData>>();
        EventHub.Instance?.RaiseEvent(new IGS.RequestSelectDonutSpawned((InGameOwner)selector, donutsData));
    }

    [PunRPC]
    private void RequestSpawnDonutRPC(int selector, string json)
    {
        DonutInstanceData data = json.DeSerializeObject<DonutInstanceData>();
        EventHub.Instance?.RaiseEvent(new IGS.RequestSpawnDonut((InGameOwner)selector, data));
    }

    [PunRPC]
    private void SendRewardRPC(int winner, int deltaGold)
    {
        if (PhotonNetwork.IsMasterClient)
            return;
        
        GiveBakerExpReward((InGameOwner)winner == InGameOwner.Right ? 3 : 2);
        GiveGoldReward(deltaGold);
        EventHub.Instance.RaiseEvent(new FE.RequestLeaderboardUpdate((InGameOwner)winner == InGameOwner.Right ? 50 : 0, DataManager.Instance.UserNickName));

        UnRegisterRoomEvent();
        
        OneButtonParam param = new((InGameOwner)winner == InGameOwner.Right ? "승리" : "패배", $"{deltaGold}를 획득하였습니다.", $"나가기", () => EventHub.Instance.RaiseEvent(new PS.GameEndEvent()));
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent((InGameOwner)winner == InGameOwner.Right ? AudioType.JNG_02 : AudioType.JNG_03, 1f));
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
        photonView.RPC("AckRewardRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void AckRewardRPC()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        EventHub.Instance?.RaiseEvent(new IGS.AckReward());
    }

    [PunRPC]
    private void DonutCollisionEventRPC(Vector3 point, float damage, string colliderId, string collideeId, int type, float spdMod, bool isCrit)
    {
        switch ((CollisionType)type)
        {
            case CollisionType.Enemy:
                // 도넛끼리 충돌 일 시
                if (isCrit)
                {
                    // 도넛 끼리 충돌이며 치명타일 시
                    point.y = 0;
                    EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_11, 1f));
                    EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact01, point, Quaternion.identity, Vector3.one * 32f));
                }
                else
                {
                    // 도넛 끼리 충돌이며 치명타가 아닐 시
            
                    // 속도 수정자 값에 의해 사운드 결정
                    switch (spdMod)
                    {
                        case < 0.1f:
                            point.y = 0;
                            EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_02, 1f));
                            EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact01, point, Quaternion.LookRotation(Vector3.up), Vector3.one * 2f));
                            break;
                        case < 0.8f:
                            EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_03, 1f));
                            EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact02, point, Quaternion.identity, Vector3.one * 16f));
                            break;
                        default:
                            EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_04, 1f));
                            EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact01, point, Quaternion.identity, Vector3.one * 24f));
                            break;
                    }
                }
                break;
            case CollisionType.Ally:
                switch (spdMod)
                {
                    case < 0.1f:
                        point.y = 0;
                        EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_02, 1f));
                        EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact03, point, Quaternion.LookRotation(Vector3.up), Vector3.one * 3f));
                        break;
                    case < 0.8f:
                        EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_03, 1f));
                        EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact02, point, Quaternion.identity, Vector3.one * 16f));
                        break;
                    default:
                        EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_04, 1f));
                        EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact01, point, Quaternion.identity, Vector3.one * 24f));
                        break;
                }

                break;
            case CollisionType.None:
            case CollisionType.Mutual:
                // 도넛 끼리 충돌이 아닐 시
                EventHub.Instance?.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_05, 1f));
                EventHub.Instance?.RaiseEvent(new ES.PlayParticleEvent(ParticleType.DonutImpact03, point, Quaternion.LookRotation(Vector3.up), Vector3.one * 2f));
                break;
        }
    }
}
