using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

public class AtkBuffObstacleSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new AtkBuffObstacleSkillObject(skillData, parent);
    }
    
    public class AtkBuffObstacleSkillObject : SkillObject
    {
        public AtkBuffObstacleSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            if (other is IBuffable buffable)
            {
                buffable.AddBuff(new AtkObstacleBuff(skillData, null, buffable));
                buffable.SendSkillEffect(ParticleType.SugarBombBuff);
            }
        }
    }
}
