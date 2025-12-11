using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

using PS = _Project.Scripts.EventStructs.PhotonStructs;

public partial class PhotonManager
{
    private void ChangeBattleState(BattleState state, InGameOwner turnOwner)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC("ChangeBattleStateRPC", RpcTarget.Others, (int)state, (int)turnOwner);
    }

    private void ShotDonut(string uid, Vector2 force)
    {
        photonView.RPC("ShotDonutRPC", RpcTarget.MasterClient, uid, force);
    }
    
    private void DonutCollisionEvent(Vector3 point, float damage, string colliderId, string collideeId, CollisionType type, float spdMod, bool isCrit)
    {
        photonView.RPC("DonutCollisionEventRPC", RpcTarget.All, point, damage, colliderId, collideeId, (int)type, spdMod, isCrit);
    }

    private void SelectDonutSpawned(InGameOwner selector, List<DonutInstanceData> donuts)
    {
        string json = donuts.SerializeObject();
        photonView.RPC("RequestSelectDonutRPC", selector == InGameOwner.Left ? RpcTarget.MasterClient : RpcTarget.Others, (int)selector, json);
    }

    private void SpawnDonut(InGameOwner selector, DonutInstanceData donut)
    {
        EventHub.Instance?.RaiseEvent(new PS.CompleteSelectDonut());
        string json = donut.SerializeObject();
        photonView.RPC("RequestSpawnDonutRPC", RpcTarget.MasterClient, (int)selector, json);
    }
    
    private void CompleteSelectDonut()
    {
        photonView.RPC("CompleteSelectDonutRPC", RpcTarget.All);
    }

    private void ChangeDeathCount(InGameOwner changer, int deathCount)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC("SetDeathCountRPC", RpcTarget.All, (int)changer, deathCount);
    }

    private void ChangeDonutList(InGameOwner owner, List<DonutInstanceData> stagedDonuts, List<DonutInstanceData> unstagedDonuts)
    {
        string json00 = stagedDonuts.SerializeObject();
        string json01 = unstagedDonuts.SerializeObject();
        photonView.RPC("SetDonutListRPC", RpcTarget.All, (int)owner, json00, json01);
    }

    private void ChangeTurnTimer(int max, int current)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC("SetTurnTimerRPC", RpcTarget.All, max, current);
    }

    private void SendReward(InGameOwner winner, int deltaGold)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        UnRegisterRoomEvent();
        photonView.RPC("SendRewardRPC", RpcTarget.Others, (int)winner, deltaGold);
    }
}
