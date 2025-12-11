using _Project.Scripts.EventStructs;
using _Project.Scripts.InGame;
using UnityEngine;

namespace _Project.Scripts.EventStructs
{
    public static class SkillStructs
    {
        public struct AttackEvent : IEvent
        {
            public DonutObject user;
            public DonutObject target;
            
            public AttackEvent(DonutObject user, DonutObject target)
            {
                this.user = user;
                this.target = target;
            }
        }
        public struct HealthEvent : IEvent
        {
            public DonutObject user;
            public DonutObject target;
            public float previousHealth;
            public float currentHealth;

            public HealthEvent(DonutObject user, DonutObject target, float previousHealth, float currentHealth)
            {
                this.user = user;
                this.target = target;
                this.previousHealth = previousHealth;
                this.currentHealth = currentHealth;
            }
        }
        
        public struct TurnStartedEvent : IEvent
        {
            public InGameOwner turnOwner;
            
            public TurnStartedEvent(InGameOwner turnOwner)
            {
                this.turnOwner = turnOwner;
            }
        }

        public struct TurnEndedEvent : IEvent
        {
            public DonutObject user;
            public DonutObject target;

            public TurnEndedEvent(DonutObject user, DonutObject target)
            {
                this.user = user;
                this.target = target;
            }
        }
    }
}
