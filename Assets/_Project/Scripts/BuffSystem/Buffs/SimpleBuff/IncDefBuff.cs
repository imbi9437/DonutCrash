using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem.Buffs.SimpleBuff
{
    public class IncDefBuff : BuffBase
    {
        public IncDefBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.def = (int)(donutInGameData.def * (1 + (skillData.fValue1 * skillData.skillLevel)));
            return donutInGameData;
        }
    }
}
