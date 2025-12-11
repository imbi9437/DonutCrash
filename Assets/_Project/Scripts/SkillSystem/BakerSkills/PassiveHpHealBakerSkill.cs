using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;
using System.Collections.Generic;

namespace _Project.Scripts.SkillSystem.BakerSkills
{
    public class PassiveHpHealBakerSkill : BakerSkillBase
    {
        public override BakerSkillObject CreateObject(InGameOwner owner, SkillData skillData)
        {
            return new PassiveHpHealBakerSkillObject(skillData, owner);
        }

        public class PassiveHpHealBakerSkillObject : BakerSkillObject
        {
            public PassiveHpHealBakerSkillObject(SkillData skillData, InGameOwner owner) : base(skillData, owner) { }

            public override void OnTurnEnded(InGameOwner turnOwner)
            {
                if (BattleController.TryGetDonutsByOwner(owner, out List<DonutObject> donuts))
                {
                    donuts.ForEach(x => x.TakeHeal(skillData.value1 * skillData.skillLevel));
                }
            }
        }
    }
}
