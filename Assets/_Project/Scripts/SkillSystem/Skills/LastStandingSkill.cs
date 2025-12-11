using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.SkillSystem;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class LastStandingSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new LastStandingSkillObject(skillData, parent);
    }
    
    public class LastStandingSkillObject : SkillObject
    {
        public LastStandingSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnTurnStarted(InGameOwner turnOwner)
        {
            if (turnOwner != parent.Owner)
                return;

            if (parent.GetCurrentHp() > skillData.fValue1 * parent.GetHp())
                return;
            
            parent.AddBuff(new LastStandingBuff(skillData, parent, parent));
            parent.SendSkillText($"{skillData.skillName}의 버프 추가", true);
            parent.SendSkillEffect(ParticleType.LastStandingBuff);
        }
    }
}
