using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class ShootingStarSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new ShootingStarSkillObject(skillData, parent);
    }
    
    public class ShootingStarSkillObject : SkillObject
    {
        public ShootingStarSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            parent.AddBuff(new ShootingStarBuff(skillData, parent,parent));
            parent.SendSkillEffect(ParticleType.StarLightBuff);
        }

        public override void OnAllyCollision(IDamageable other)
        {
            parent.AddBuff(new ShootingStarBuff(skillData, parent,parent));
            parent.SendSkillEffect(ParticleType.StarLightBuff);
        }

        public override void OnEmptyCollision()
        {
            parent.AddBuff(new ShootingStarBuff(skillData, parent,parent));
            parent.SendSkillEffect(ParticleType.StarLightBuff);
        }
    }
}
