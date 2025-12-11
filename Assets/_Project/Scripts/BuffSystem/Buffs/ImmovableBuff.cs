using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem.Buffs
{
    public class ImmovableBuff : BuffBase
    {
        public ImmovableBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.hp = (int)(donutInGameData.hp * (1 + skillData.fValue1));
            return donutInGameData;
        }
    }
}
