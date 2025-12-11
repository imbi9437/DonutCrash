using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class RisingStarBuff : BuffBase
{
    public RisingStarBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, skillData.cooldown) { }

    public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
    {
        donutInGameData.slingShotPower = (int)(donutInGameData.slingShotPower * (1 + skillData.fValue1));
        return donutInGameData;
    }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        if (turnOwner == BuffContainer.Owner)
            stack--;
        
        if (stack <= 0)
        {
            BuffContainer.SendSkillText($"{skillData.skillName}의 버프 제거",false);
            RemoveBuff();
        }
    }
}
