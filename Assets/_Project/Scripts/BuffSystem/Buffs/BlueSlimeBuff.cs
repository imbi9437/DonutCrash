using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class BlueSlimeBuff : BuffBase
{
    // Start is called before the first frame update
    public BlueSlimeBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, skillData.cooldown) { }

    public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
    {
        donutInGameData.slingShotPower *= 1 - skillData.fValue1;
        return donutInGameData;
    }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        if (turnOwner == BuffContainer.Owner)
            stack =- 1;

        if (stack > 0)
            return;
        
        BuffContainer.SendSkillText($"{skillData.skillName} 버프 제거", false);
        RemoveBuff();
    }
}
