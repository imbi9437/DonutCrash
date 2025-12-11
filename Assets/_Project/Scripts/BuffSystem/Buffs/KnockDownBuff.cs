using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class KnockDownBuff : BuffBase
{
    public KnockDownBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

    public override void OnAttack(IDamageable other)
    {
        int damage = (int)(BuffContainer.GetAtk() * skillData.fValue1);
        other.TakeDamage(damage, false, BuffOwner);
        other.SendSkillText($"{skillData.skillName}의 버프 효과 발동", false);
            
        if (BuffContainer is DonutObject donut)
            donut.SetSpeed(skillData.value1);
        
        RemoveBuff();
    }
}
