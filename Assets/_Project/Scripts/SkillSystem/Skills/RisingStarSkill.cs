using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class RisingStarSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new RisingStarSkillObjectSkill(skillData, parent);
    }
    
    public class RisingStarSkillObjectSkill : SkillObject
    {
        public RisingStarSkillObjectSkill(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAllyCollision(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new RisingStarBuff(skillData, parent, buffable));
                buffable.SendSkillEffect(ParticleType.RisingStarBuff);
            }
        }
    }
}
