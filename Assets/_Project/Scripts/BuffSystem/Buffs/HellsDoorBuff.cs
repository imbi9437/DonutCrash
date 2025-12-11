using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using _Project.Scripts.InGame.StatePattern;
using _Project.Scripts.Interface;
using System.Collections.Generic;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.BuffSystem.Buffs
{
    public class HellsDoorBuff : BuffBase
    {
        // Start is called before the first frame update
        public HellsDoorBuff(SkillData skillData, IBuffable buffOwner, IBuffable buffContainer, int stack = 1) : base(skillData, buffOwner, buffContainer, stack) { }

        public override bool IsSameBuff(BuffBase buff)
        {
            return buff is HellsDoorBuff && buff.BuffOwner == BuffOwner;
        }

        public override void StackedUp(int value)
        {
            stack += value;
            BuffContainer.SendSkillText($"{skillData.skillName}스킬의 중첩 {stack}", true);
            if (stack < skillData.cooldown)
                return;

            BattleController.TryGetOpponentDonutsByOwner(BuffOwner.Owner, out List<DonutObject> donuts);
            donuts.ForEach(x =>
            {
                x.TakeDamage(BuffOwner.GetAtk() * skillData.fValue1, false, BuffOwner);
                x.SendSkillText($"{skillData.skillName} 폭발 공격 피격", true);
                x.SendSkillEffect(ParticleType.HellsDoorAttack);
            });
            
            BuffContainer.SendSkillText($"{skillData.skillName} 버프 제거", false);
            RemoveBuff();
        }

        public override DonutInGameData ModifyAdd(DonutInGameData donutInGameData)
        {
            donutInGameData.atk += skillData.value1 * stack;
            return donutInGameData;
        }
    }
}
