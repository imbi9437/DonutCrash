using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;
using _Project.Scripts.Interface;
using System.Collections.Generic;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.SkillSystem.Skills
{
    public class HeavensDoorSkill : SkillBase
    {
        public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
        {
            return new HeavensDoorSkillObject(skillData, parent);
        }
    
        public class HeavensDoorSkillObject : SkillObject
        {
            public HeavensDoorSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

            public override void OnSpawn()
            {
                BattleController.TryGetDonutsByOwner(parent.Owner, out List<DonutObject> donuts);
                donuts.ForEach(x =>
                {
                    x.TakeHeal(x.GetHp() * skillData.fValue1);
                    x.SendSkillEffect(ParticleType.HeavensDoorHeal);
                });
                parent.SendSkillEffect(ParticleType.HeavensDoorSpawn);
            }

            public override void OnAllyCollision(IDamageable other)
            {
                other.TakeHeal(skillData.value1);
                other.SendSkillEffect(ParticleType.HeavensDoorHeal);
            }
        }
    }
}
