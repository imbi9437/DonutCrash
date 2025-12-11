using _Project.Scripts.BuffSystem.Buffs;
using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.SkillSystem.Skills
{
    public class SugarBombSkill : SkillBase
    {
        public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
        {
            return new SugarBombSkillObject(skillData, parent);
        }
        
        public class SugarBombSkillObject : SkillObject
        {
            public SugarBombSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

            public override void OnAllyCollision(IDamageable other)
            {
                if (other is IBuffable buffable)
                {
                    buffable.AddBuff(new SugarBombBuff(skillData, parent, buffable));
                    buffable.SendSkillEffect(ParticleType.SugarBombBuff);
                }
            }
        }
    }
}
