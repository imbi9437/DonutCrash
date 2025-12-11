using _Project.Scripts.BuffSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

public class AtkObstacleBuff : BuffBase
{
    public AtkObstacleBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

    public override DonutInGameData ModifyAdd(DonutInGameData donutInGameData)
    {
        donutInGameData.atk += skillData.value1;
        return donutInGameData;
    }

    public override void OnTurnEnded(InGameOwner turnOwner)
    {
        BuffContainer.SendSkillText($"{skillData.skillName} 버프 제거", false);
        RemoveBuff();
    }
}
