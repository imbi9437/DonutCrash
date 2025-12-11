using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class ShootingStarBuff : BuffBase
{
    private int remainTurn;

    public ShootingStarBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack)
    {
        remainTurn = skillData.cooldown; 
    }

    public override bool IsSameBuff(BuffBase buff) => false;

    public override void StackedUp(int value) { }

    public override DonutInGameData ModifyAdd(DonutInGameData donutInGameData)
    {
        donutInGameData.atk += skillData.value1;
        return donutInGameData;
    }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        remainTurn--;
        
        if (remainTurn <= 0)
        {
            BuffContainer.SendSkillText($"{skillData.skillName}의 버프 제거", false);
            RemoveBuff();
        }
    }
}
