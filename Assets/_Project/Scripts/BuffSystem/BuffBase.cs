using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem
{
    public abstract class BuffBase
    {
        public IBuffable BuffOwner { get; private set; }
        public IBuffable BuffContainer { get; private set; }

        protected SkillData skillData;
        protected int stack;
        
        public BuffBase(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1)
        {
            this.skillData = skillData;
            BuffOwner = buffOwner;
            BuffContainer = buffContainer;
            this.stack = stack;
        }

        public virtual bool IsSameBuff(BuffBase buff)
        {
            return buff.skillData.uid == skillData.uid;
        }
        
        public virtual void StackedUp(int value)
        {
            stack = 1;
            
            if (stack > 0)
                return;
            
            RemoveBuff();
        }

        public virtual void StackedDown(int value)
        {
            stack -= value;
            
            if (stack > 0)
                return;
            
            RemoveBuff();
        }

        public virtual void RemoveBuff()
        {
            BuffContainer.RemoveBuff(this);
        }

        public virtual DonutInGameData ModifyAdd(DonutInGameData donutInGameData)
        {
            return donutInGameData;
        }

        public virtual DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            return donutInGameData;
        }
        
        public virtual void OnSpawn() { }
        public virtual void OnTurnStarted(InGameOwner turnOwner) { }
        public virtual void OnAttack(IDamageable other) { }
        public virtual void OnDefense(IDamageable other) { }
        public virtual void OnAllyCollision(IDamageable damageable) { }
        public virtual void OnEmptyCollision() { }
        public virtual void OnTurnEnded(InGameOwner turnOwner) { }
    }
}
