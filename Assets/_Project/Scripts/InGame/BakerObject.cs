using _Project.Scripts.SkillSystem;
using System.Collections.Generic;
using UnityEngine;
using IGS = _Project.Scripts.EventStructs.InGameStructs;

namespace _Project.Scripts.InGame
{
    public class BakerObject
    {
        private readonly List<BakerSkillObject> _bakerSkillObjects = new List<BakerSkillObject>();
        
        private BakerInstanceData _bakerInstanceData;
        public InGameOwner Owner { get; private set; }

        public void Initialize(BakerInstanceData bakerInstanceData, InGameOwner owner, List<BakerSkillObject> skillObjects = null)
        {
            if (DataManager.Instance.TryGetBakerData(bakerInstanceData?.origin, out BakerData bakerData) == false)
            {
                Debug.LogError($"베이커 데이터가 비어있습니다.");
                return;
            }
            

            _bakerInstanceData = bakerInstanceData;
            
            Owner = owner;
            
            _bakerSkillObjects.Clear();
            bakerData?.skills?.ForEach(x =>
            {
                if (DataManager.Instance.TryGetSkillData(x, out SkillData skillData) == false)
                {
                    Debug.LogError($"{bakerData.uid}의 스킬 데이터 {x} 탐색 실패");
                    return;
                }
                skillData = new SkillData(skillData, _bakerInstanceData?.level ?? 1);
                if (BakerSkillFactory.TryGetBakerSkillObject(skillData, Owner, out BakerSkillObject bakerSkillObject))
                    _bakerSkillObjects.Add(bakerSkillObject);
                else
                    Debug.LogError($"{bakerData.uid}의 스킬{skillData.uid} 생성 실패");
            });
            if (skillObjects != null)
            {
                _bakerSkillObjects.AddRange(skillObjects);
            }
            
            EventHub.Instance.RegisterEvent<IGS.DonutSpawnedEvent>(OnDonutSpawned);
            EventHub.Instance.RegisterEvent<IGS.TurnEndEvent>(OnTurnEnded);
        }

        public void DeActive()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.DonutSpawnedEvent>(OnDonutSpawned);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(OnTurnEnded);
        }

        private void OnDonutSpawned(IGS.DonutSpawnedEvent evt) => DonutSpawned(evt.donut);
        private void OnTurnEnded(IGS.TurnEndEvent evt) => TurnEnded(evt.turnOwner);

        private void DonutSpawned(DonutObject donut)
        {
            _bakerSkillObjects?.ForEach(x => x?.OnDonutSpawned(donut));
        }

        private void TurnEnded(InGameOwner owner)
        {
            _bakerSkillObjects?.ForEach(x => x?.OnTurnEnded(owner));
        }
    }
}
