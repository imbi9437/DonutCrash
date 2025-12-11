using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

namespace _Project.Scripts.SkillSystem
{
    public abstract class SkillObject
    {
        protected DonutObject parent;
        protected SkillData skillData;

        public SkillObject(SkillData skillData, DonutObject parent)
        {
            this.skillData = new SkillData(skillData);
            this.parent = parent;
        }

        public virtual void OnSpawn() { }
        public virtual void OnTurnStarted(InGameOwner turnOwner) { }
        public virtual void OnAttack(IDamageable other) { }
        public virtual void OnDefense(IDamageable other) { }
        public virtual void OnAllyCollision(IDamageable other) { }
        public virtual void OnEmptyCollision() { }
        public virtual void OnTurnEnded(InGameOwner turnOwner) { }
    }
}
