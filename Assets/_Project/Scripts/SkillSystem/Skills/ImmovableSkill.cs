using _Project.Scripts.BuffSystem.Buffs;
using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.SkillSystem.Skills
{
    public class ImmovableSkill : SkillBase
    {
        public override SkillObject CreateObject(DonutObject parent, SkillData skillData)
        {
            return new ImmovableSkillObject(skillData, parent);
        }

        public class ImmovableSkillObject : SkillObject
        {
            public ImmovableSkillObject(SkillData skillData, DonutObject parent) : base(skillData, parent) { }

            public override void OnSpawn()
            {
                BattleController.TryGetDeathCountByOwner(parent.Owner, out int? deathCount);
                if (deathCount <= skillData.value1)
                {
                    parent.AddBuff(new ImmovableBuff(skillData, parent, parent, 1));
                    parent.SendSkillText($"{skillData.skillName}의 버프 추가", true);
                    parent.SendSkillEffect(ParticleType.ImmovableBuff);
                }
            }
        }
    }
}
