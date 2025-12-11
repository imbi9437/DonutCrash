using _Project.Scripts.InGame;
using _Project.Scripts.SkillSystem.BakerSkills;
using System.Collections.Generic;

namespace _Project.Scripts.SkillSystem
{
    public static class BakerSkillFactory
    {
        private static readonly Dictionary<string, BakerSkillBase> BakerSkillTable = new Dictionary<string, BakerSkillBase>()
        {
            {"32000001", new PassiveIncAtkBakerSkill()},
            {"32000002", new PassiveIncHpBakerSkill()},
            {"32000003", new PassiveIncDefBakerSkill()},
            {"32000004", new PassiveIncShotBakerSkill()},
            {"32000005", new PassiveHpHealBakerSkill()},
        };

        public static bool TryGetBakerSkillObject(SkillData skillData, InGameOwner owner, out BakerSkillObject bakerSkillObject)
        {
            if (!BakerSkillTable.TryGetValue(skillData.logicUid, out BakerSkillBase skill))
            {
                bakerSkillObject = null;
                return false;
            }
            
            bakerSkillObject = skill.CreateObject(owner, skillData);
            return true;
        }
    }
}
