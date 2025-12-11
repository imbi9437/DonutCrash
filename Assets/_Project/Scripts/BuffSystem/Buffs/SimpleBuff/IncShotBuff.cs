using _Project.Scripts.Interface;

namespace _Project.Scripts.BuffSystem.Buffs.SimpleBuff
{
    public class IncShotBuff : BuffBase
    {
        public IncShotBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }
        
        public override DonutInGameData ModifyMulti(DonutInGameData donutInGameData)
        {
            donutInGameData.slingShotPower = (int)(donutInGameData.slingShotPower * (1 + (skillData.fValue1 * skillData.skillLevel)));
            return donutInGameData;
        }
    }
}
