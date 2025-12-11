using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class ChaosSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new ChaosSkillObject(skillData, parent);
    }
    
    public class ChaosSkillObject : SkillObject
    {
        public ChaosSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new ChaosBuff(skillData, parent, buffable));
            }
        }
    }
}
