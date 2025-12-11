using _Project.Scripts.BuffSystem.Buffs.SimpleBuff;
using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;

namespace _Project.Scripts.SkillSystem.BakerSkills
{
    public class PassiveIncDefBakerSkill : BakerSkillBase
    {
        public override BakerSkillObject CreateObject(InGameOwner owner, SkillData skillData)
        {
            return new PassiveIncDefBakerSkillObject(skillData, owner);
        }

        public class PassiveIncDefBakerSkillObject : BakerSkillObject
        {
            public PassiveIncDefBakerSkillObject(SkillData skillData, InGameOwner owner) : base(skillData, owner) { }
            
            public override void OnDonutSpawned(DonutObject donut)
            {
                if (donut.Owner != owner)
                    return;
                
                BattleController.TryGetDeathCountByOwner(owner, out int? deathCount);
                if (deathCount > skillData.cooldown)
                    return;

                donut.AddBuff(new IncDefBuff(skillData, null, donut));
            }
        }
    }
}
