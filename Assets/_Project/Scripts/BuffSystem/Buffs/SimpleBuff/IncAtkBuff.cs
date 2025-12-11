using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem.Buffs.SimpleBuff
{
    public class IncAtkBuff : BuffBase
    {
        public IncAtkBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.atk = (int)(donutInGameData.atk * (1 + (skillData.fValue1 * skillData.skillLevel)));
            return donutInGameData;
        }
    }
}
