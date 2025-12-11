using _Project.Scripts.InGame;

namespace _Project.Scripts.SkillSystem
{
    public abstract class SkillBase
    {
        public abstract SkillObject CreateObject(DonutObject parent, SkillData skillData);
    }
}
