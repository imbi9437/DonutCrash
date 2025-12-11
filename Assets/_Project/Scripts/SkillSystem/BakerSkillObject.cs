using _Project.Scripts.InGame;

namespace _Project.Scripts.SkillSystem
{
    public abstract class BakerSkillObject
    {
        protected InGameOwner owner;
        protected SkillData skillData;
    
        protected BakerSkillObject(SkillData skillData, InGameOwner owner)
        {
            this.skillData = skillData;
            this.owner = owner;
        }
    
        public virtual void OnDonutSpawned(DonutObject donut) { }
        public virtual void OnTurnStarted(InGameOwner turnOwner) { }
        public virtual void OnTurnEnded(InGameOwner turnOwner) { }
    }
}
