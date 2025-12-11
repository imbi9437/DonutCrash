using _Project.Scripts.InGame;

namespace _Project.Scripts.SkillSystem
{
    public abstract class BakerSkillBase
    {
        public abstract BakerSkillObject CreateObject(InGameOwner owner, SkillData skillData);
    }
}
