using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;

using PS = _Project.Scripts.EventStructs.PhotonStructs;

public class AttackObstacleSkill : SkillBase
{
    public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
    {
        return new AttackObstacleSkillObject(skillData, parent);
    }
    
    public class AttackObstacleSkillObject : SkillObject
    {
        public AttackObstacleSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

        public override void OnAttack(IDamageable other)
        {
            other.TakeDamage(skillData.value1, false, null);
            EventHub.Instance.RaiseEvent(new PS.RequestDonutCollisionEvent(other.GetPosition(), skillData.value1, null, other.GetUid(), CollisionType.Mutual, 0f, false));
        }
    }
}
