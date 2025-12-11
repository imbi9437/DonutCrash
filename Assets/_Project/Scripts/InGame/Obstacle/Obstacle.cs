using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class Obstacle : MonoBehaviourPun
{
    [SerializeField] private SkillData skillData;
    
    private readonly List<SkillObject> _skillObjects = new List<SkillObject>();
    private bool _isRunning;
    
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        
        _skillObjects.Clear();
        if (SkillFactory.TryGetSkillObject(skillData, null, out SkillObject skillObject))
            _skillObjects.Add(skillObject);
        else
            Debug.LogError($"Could not get skill object for skill id {skillData.uid}");
        
        EventHub.Instance.RegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
        EventHub.Instance.RegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
        EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        if (_isRunning == false)
            return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            OnAttack(damageable);
        }
    }

    private void OnAttack(IDamageable other)
    {
        _skillObjects.ToList().ForEach(x => x.OnAttack(other));
    }

    private void OnTurnRunning(IGS.TurnRunningEvent evt)
    {
        _isRunning = true;
    }

    private void OnTurnEnd(IGS.TurnEndEvent evt)
    {
        _isRunning = false;
    }
}
