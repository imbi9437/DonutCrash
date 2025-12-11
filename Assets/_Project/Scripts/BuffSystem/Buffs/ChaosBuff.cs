using _Project.Scripts.BuffSystem;
using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBuff : BuffBase
{
    public ChaosBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, -1) { }

    public override void StackedUp(int value) { }
    public override void StackedDown(int value) { }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        BuffContainer.TakeDamage(BuffContainer.GetHp() * skillData.fValue1, false, BuffOwner);
        BuffContainer.SendSkillEffect(ParticleType.ChaosAttack);
    }
}
