using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class QueenCardSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new QueenCardSkillObject(skillData, parent);
    }

    public class QueenCardSkillObject : SkillObject
    {
        public QueenCardSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new QueenCardBuff(skillData, parent, buffable));
                buffable.SendSkillText($"{skillData.skillName}의 버프 추가", true);
                buffable.SendSkillEffect(ParticleType.QueenCardAttack);
            }
        }
    }
}
