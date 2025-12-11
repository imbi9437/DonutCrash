using _Project.Scripts.InGame;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.EventStructs
{
    public static class InGameStructs
    {
        public struct SetDeckData : IEvent
        {
            public InGameOwner owner;
            public DeckData deck;

            public SetDeckData(InGameOwner owner, DeckData deck)
            {
                this.owner = owner;
                this.deck = deck;
            }
        }
        public struct BattleStartEvent : IEvent { }

        public struct TurnStartEvent : IEvent
        {
            public InGameOwner turnOwner;
            public bool resetInteractable;

            public TurnStartEvent(InGameOwner turnOwner, bool resetInteractable)
            {
                this.turnOwner = turnOwner;
                this.resetInteractable = resetInteractable;
            }
        }

        public struct ShotDonutEvent : IEvent
        {
            public string donutUid;
            public Vector2 shotForce;

            public ShotDonutEvent(string donutUid, Vector2 shotForce)
            {
                this.donutUid = donutUid;
                this.shotForce = shotForce;
            }
        }

        public struct TurnRunningEvent : IEvent
        {
            public InGameOwner turnOwner;
            public TurnRunningEvent(InGameOwner turnOwner) => this.turnOwner = turnOwner;
        }

        public struct TurnEndEvent : IEvent
        {
            public InGameOwner turnOwner;
            public TurnEndEvent(InGameOwner turnOwner) => this.turnOwner = turnOwner;
        }

        public struct BattleEndEvent : IEvent { }

        public struct RemoveDonutEvent : IEvent
        {
            public DonutObject donut;

            public RemoveDonutEvent(DonutObject donut)
            {
                this.donut = donut;
            }
        }

        public struct ConfirmDeckEvent : IEvent
        {
            public InGameOwner deckOwner;
            public string[] uids;

            public ConfirmDeckEvent(InGameOwner deckOwner, string[] uids)
            {
                this.deckOwner = deckOwner;
                this.uids = uids;
            }
        }

        public struct InteractableChange : IEvent
        {
            public string uid;
            public bool isInteractable;

            public InteractableChange(string uid, bool isInteractable)
            {
                this.uid = uid;
                this.isInteractable = isInteractable;
            }
        }

        public struct DraggingStateChange : IEvent
        {
            public string donutUid;
            public bool isDragging;

            public DraggingStateChange(string uid, bool dragging)
            {
                donutUid = uid;
                isDragging = dragging;
            }
        }

        public struct CalledGameOver : IEvent
        {
            public InGameOwner diedOwner;
            public bool isDied;

            public CalledGameOver(InGameOwner owner, bool died)
            {
                diedOwner = owner;

                isDied = died;
            }
        }

        public struct ChangeDeathCount : IEvent
        {
            public InGameOwner deathOwner;
            public int deathCount;

            public ChangeDeathCount(InGameOwner deathOwner, int deathCount)
            {
                this.deathOwner = deathOwner;
                this.deathCount = deathCount;
            }
        }

        public struct ChangeDonutList : IEvent
        {
            public InGameOwner owner;
            public List<DonutInstanceData> stagedDonuts;
            public List<DonutInstanceData> unstagedDonuts;

            public ChangeDonutList(InGameOwner owner, List<DonutInstanceData> stagedDonuts, List<DonutInstanceData> unstagedDonuts)
            {
                this.owner = owner;
                this.stagedDonuts = stagedDonuts;
                this.unstagedDonuts = unstagedDonuts;
            }
        }

        public struct ChangeTurnTimer : IEvent
        {
            public int max;
            public int current;

            public ChangeTurnTimer(int max, int current)
            {
                this.max = max;
                this.current = current;
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

        public struct DonutSpawnedEvent : IEvent
        {
            public DonutObject donut;
            public DonutSpawnedEvent(DonutObject donut) => this.donut = donut;
        }

        public struct CompleteSelectDonut : IEvent { }

        public struct RequestSetUserProfile : IEvent
        {
            public InGameOwner user;
            public string nickName;
            public string bakerUid;

            public RequestSetUserProfile(InGameOwner user, string nickName, string bakerUid)
            {
                this.user = user;
                this.nickName = nickName;
                this.bakerUid = bakerUid;
            }
        }

        public struct AckReward : IEvent { }

        public struct TryRandomObstacle : IEvent { }
        public struct DragCancelledEvent : IEvent
        {
            // 취소된 도넛을 식별하기 위한 UID
            public string donutUid;

            // 하이라이트를 올바르게 설정하기 위해 현재 턴 소유자를 전달합니다.
            public InGameOwner currentTurnOwner;

            public DragCancelledEvent(string uid, InGameOwner owner)
            {
                donutUid = uid;
                currentTurnOwner = owner;
            }

        }
        public struct ShowTurnStartPopup : IEvent
        {
            public InGameOwner owner;
            public string nickname;

            public ShowTurnStartPopup(InGameOwner owner, string nickname)
            {
                this.owner = owner;
                this.nickname = nickname;
            }
        }
    }
}
