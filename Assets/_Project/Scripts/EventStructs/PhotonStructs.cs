using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.EventStructs
{
    public static class PhotonStructs
    {
        public struct JoinedLobbyEvent : IEvent { }
        public struct LeaveRoomEvent : IEvent { }
        public struct StartMatchMakingEvent : IEvent { }

        public struct StopMatchMakingEvent : IEvent
        {
            public bool withLeaveRoom;
            public StopMatchMakingEvent(bool withLeaveRoom) => this.withLeaveRoom = withLeaveRoom;
        }
        public struct CompleteWaitingPlayer : IEvent { }

        public struct ChangeBattleState : IEvent
        {
            public BattleState state;
            public InGameOwner turnOwner;

            public ChangeBattleState(BattleState state, InGameOwner turnOwner)
            {
                this.state = state;
                this.turnOwner = turnOwner;
            }
        }

        public struct ShotDonut : IEvent
        {
            public string uid;
            public Vector2 force;

            public ShotDonut(string uid, Vector2 force)
            {
                this.uid = uid;
                this.force = force;
            }
        }

        public struct RequestSelectDonutSpawned : IEvent
        {
            public InGameOwner selector;
            public List<DonutInstanceData> donuts;

            public RequestSelectDonutSpawned(InGameOwner selector, List<DonutInstanceData> donuts)
            {
                this.selector = selector;
                this.donuts = donuts;
            }
        }

        public struct RequestSpawnDonut : IEvent
        {
            public InGameOwner selector;
            public DonutInstanceData donut;

            public RequestSpawnDonut(InGameOwner selector, DonutInstanceData donut)
            {
                this.selector = selector;
                this.donut = donut;
            }
        }

        public struct RequestChangeDeathCount : IEvent
        {
            public InGameOwner owner;
            public int deathCount;

            public RequestChangeDeathCount(InGameOwner owner, int deathCount)
            {
                this.owner = owner;
                this.deathCount = deathCount;
            }
        }

        public struct RequestChangeDonutList : IEvent
        {
            public InGameOwner owner;
            public List<DonutInstanceData> stagedDonuts;
            public List<DonutInstanceData> unstagedDonuts;
            
            public RequestChangeDonutList(InGameOwner owner, List<DonutInstanceData> stagedDonuts, List<DonutInstanceData> unstagedDonuts)
            {
                this.owner = owner;
                this.stagedDonuts = stagedDonuts;
                this.unstagedDonuts = unstagedDonuts;
            }
        }

        public struct RequestChangeTurnTimer : IEvent
        {
            public int max;
            public int current;
            
            public RequestChangeTurnTimer(int max, int current)
            {
                this.max = max;
                this.current = current;
            }
        }

        public struct RequestReward : IEvent
        {
            public InGameOwner winner;
            public int deltaGold;

            public RequestReward(InGameOwner winner, int deltaGold)
            {
                this.winner = winner;
                this.deltaGold = deltaGold;
            }
        }
        
        public struct UnRegisterRoomEvent : IEvent { }
        public struct GameEndEvent : IEvent { }
        public struct CompleteSelectDonut : IEvent { }
        
        public struct OnOpponentDisconnected : IEvent { }
        
        public struct OnUnexpectedDisconnect : IEvent { }
        
        public struct RequestSetUserProfile : IEvent { }

        public struct RequestDonutCollisionEvent : IEvent
        {
            public Vector3 point;
            public float damage;
            public string collider;
            public string collidee;
            public CollisionType type;
            public float spdMod;
            public bool isCrit;

            public RequestDonutCollisionEvent(Vector3 point, float damage, string collider, string collidee, CollisionType type, float spdMod, bool isCrit)
            {
                this.point = point;
                this.damage = damage;
                this.collider = collider;
                this.collidee = collidee;
                this.type = type;
                this.spdMod = spdMod;
                this.isCrit = isCrit;
            }
        }

        public struct OnChangeApplicationFocus : IEvent
        {
            public bool hasFocus;
            public OnChangeApplicationFocus(bool hasFocus) => this.hasFocus = hasFocus;
        }
    }
}
