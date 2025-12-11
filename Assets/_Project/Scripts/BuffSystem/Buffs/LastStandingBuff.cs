using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastStandingBuff : BuffBase
{
    public LastStandingBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

    public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
    {
        donutInGameData.atk = (int)(donutInGameData.atk * (1 + skillData.fValue2));
        return donutInGameData;
    }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        if (turnOwner == BuffContainer.Owner)
            RemoveBuff();
    }
}
