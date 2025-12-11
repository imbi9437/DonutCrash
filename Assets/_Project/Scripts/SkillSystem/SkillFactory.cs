using _Project.Scripts.InGame;
using _Project.Scripts.SkillSystem.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.SkillSystem
{
    public static class SkillFactory
    {
        /// <summary>
        /// 스킬 데이터를 기반으로 스킬 오브젝트를 생성하기 위한 파싱 딕셔너리
        /// 스킬이 추가될 때마다 초기화 객체를 추가해주어야 합니다.
        /// </summary>
        private static readonly Dictionary<string, SkillBase> SkillTable = new Dictionary<string, SkillBase>
        {
            {"31000001", new HellsDoorSkill()},
            {"31000002", new BlueSlimeSkill()},
            {"31000003", new ImmovableSkill()},
            {"31000004", new SugarBombSkill()},
            {"31000005", new HeavensDoorSkill()},
            {"31000006", new KnockDownSkill()},
            {"31000007", new QueenCardSkill()},
            {"31000008", new BulletProofSkill()},
            {"31000009", new RisingStarSkill()},
            {"31000010", new ChaosSkill()},
            {"31000011", new ShootingStarSkill()},
            {"31000012", new LastStandingSkill()},
            
            {"31000901", new AtkBuffObstacleSkill()},
            {"31000902", new AttackObstacleSkill()},
        };
    
        public static bool TryGetSkillObject(SkillData skillData, DonutObject owner, out SkillObject skillObject)
        {
            if (!SkillTable.TryGetValue(skillData.logicUid, out SkillBase skill))
            {
                skillObject = null;
                return false;
            }

            skillObject = skill.CreateObject(owner, skillData);
            return true;
        }
    }
}
