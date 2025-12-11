using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem.Buffs.SimpleBuff
{
    public class IncHpBuff : BuffBase
    {
        public IncHpBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }
    
        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.hp = (int)(donutInGameData.hp * skillData.fValue1 * skillData.skillLevel);
            return donutInGameData;
        }
    }
}
