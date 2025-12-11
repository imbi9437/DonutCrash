using _Project.Scripts.InGame;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.SkillSystem.Skills
{
    public class KnockDownSkill : SkillBase
    {
        public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
        {
            return new KnockDownSkillObject(skillData, parent);
        }
        
        public class KnockDownSkillObject : SkillObject
        {
            public KnockDownSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

            public override void OnTurnStarted(InGameOwner turnOwner)
            {
                if (turnOwner == parent.Owner)
                {
                    parent.AddBuff(new KnockDownBuff(skillData, parent, parent, 1));
                    parent.SendSkillText($"{skillData.skillName}의 버프 추가", true);
                }
            }
        }
    }
}
