using _Project.Scripts.BuffSystem.Buffs;
using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;

namespace _Project.Scripts.SkillSystem.Skills
{
    public class HellsDoorSkill : SkillBase
    {
        public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
        {
            return new HellsDoorSkillObject(skillData, parent);
        }

        public class HellsDoorSkillObject : SkillObject
        {
            public HellsDoorSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

            public override void OnTurnEnded(InGameOwner turnOwner)
            {
                parent.AddBuff(new HellsDoorBuff(skillData, parent, parent, 1));
                parent.SendSkillEffect(ParticleType.HellsDoorBuff);
            }
        }
    }
}
