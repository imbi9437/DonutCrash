using _Project.Scripts.BuffSystem.Buffs.SimpleBuff;
using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;

namespace _Project.Scripts.SkillSystem.BakerSkills
{
    public class PassiveIncShotBakerSkill : BakerSkillBase
    {
        public override BakerSkillObject CreateObject(InGameOwner owner, SkillData skillData)
        {
            return new PassiveIncShotBakerSkillObject(skillData, owner);
        }

        public class PassiveIncShotBakerSkillObject : BakerSkillObject
        {
            public PassiveIncShotBakerSkillObject(SkillData skillData, InGameOwner owner) : base(skillData, owner) { }
            
            public override void OnDonutSpawned(DonutObject donut)
            {
                if (donut.Owner != owner)
                    return;
                
                BattleController.TryGetDeathCountByOwner(owner, out int? deathCount);
                if (deathCount > skillData.cooldown)
                    return;

                donut.AddBuff(new IncShotBuff(skillData, null, donut));
            }
        }
    }
}
