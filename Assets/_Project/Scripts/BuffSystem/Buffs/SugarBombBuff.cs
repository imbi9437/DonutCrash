using _Project.Scripts.InGame;
using _Project.Scripts.Interface;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.BuffSystem.Buffs
{
    public class SugarBombBuff : BuffBase
    {
        public SugarBombBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer) : base(skillData, buffOwner, buffContainer, skillData.cooldown) { }

        public override void StackedUp(int value)
        {
            stack = skillData.cooldown;
        }

        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.atk = (int)(donutInGameData.atk * (1 + skillData.fValue1));
            return donutInGameData;
        }

        public override void OnTurnEnded(InGameOwner turnOwner)
        {
            if (turnOwner != BuffContainer.Owner)
                return;
            
            stack -= 1;
            if (stack <= 0)
            {
                BuffContainer.SendSkillText($"{skillData.skillName}의 버프 제거", false);
                RemoveBuff();
            }
        }
    }
}
