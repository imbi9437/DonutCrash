using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class BlueSlimeSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new BlueSlimeSkillObject(skillData, parent);
    }

    public class BlueSlimeSkillObject : SkillObject
    {
        public BlueSlimeSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new BlueSlimeBuff(skillData, parent, buffable, 1));
                buffable.SendSkillEffect(ParticleType.BlueSlimeAttack);
            }
        }
    }
}
