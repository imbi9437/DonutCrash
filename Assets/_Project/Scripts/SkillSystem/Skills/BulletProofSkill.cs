using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class BulletProofSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new BulletProofSkillObject(skillData, parent);
    }

    public class BulletProofSkillObject : SkillObject
    {
        public BulletProofSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAllyCollision(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new BulletProofBuff(skillData, parent, buffable));
                buffable.SendSkillEffect(ParticleType.BulletProofBuff);
            }
        }

        public override void OnDefense(IDamageable other)
        {
            if (other is DonutObject donut)
                donut.SetSpeed(0f);
        }
    }
}
