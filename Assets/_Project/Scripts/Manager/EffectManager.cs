using _Project.Scripts.EffectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CSS = _Project.Scripts.EventStructs.ChangeSceneStructs;
using ES = _Project.Scripts.EventStructs.EffectStructs;

public class EffectManager : MonoBehaviour
{
    [Space]
    [Header("Effect Data")]
    [SerializeField] private List<ParticleScriptableObject> particles;
    
    private static EffectManager _instance;
    private readonly Dictionary<ParticleType, PrefabObjectPool<ParticleSystem>> _pools =  new ();
    private Transform _parent;
    
    #region Unity Message Methods

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
        _parent = new GameObject("Effect").transform;
        
        EventHub.Instance.RegisterEvent<ES.SetParticleScriptableObjects>(OnSetParticleScriptableObjects);
        EventHub.Instance.RegisterEvent<ES.AddParticleScriptableObject>(OnAddParticleScriptableObject);
        EventHub.Instance.RegisterEvent<ES.PlayParticleEvent>(RequestPlayEffect);
        EventHub.Instance.RegisterEvent<CSS.StartLoadSceneEvent>(OnStartLoadScene);
        EventHub.Instance.RegisterEvent<CSS.CompleteLoadSceneEvent>(OnCompleteLoadScene);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<ES.SetParticleScriptableObjects>(OnSetParticleScriptableObjects);
        EventHub.Instance?.UnRegisterEvent<ES.AddParticleScriptableObject>(OnAddParticleScriptableObject);
        EventHub.Instance?.UnRegisterEvent<ES.PlayParticleEvent>(RequestPlayEffect);
        EventHub.Instance?.UnRegisterEvent<CSS.StartLoadSceneEvent>(OnStartLoadScene);
        EventHub.Instance?.UnRegisterEvent<CSS.CompleteLoadSceneEvent>(OnCompleteLoadScene);
    }
    
    #endregion Unity Message Methods
    
    #region Event Wrapper Methods

    private void OnStartLoadScene(CSS.StartLoadSceneEvent evt) => EventHub.Instance.UnRegisterEvent<ES.PlayParticleEvent>(RequestPlayEffect); 
    private void OnCompleteLoadScene(CSS.CompleteLoadSceneEvent evt) => ResetPool();
    private void OnSetParticleScriptableObjects(ES.SetParticleScriptableObjects evt) => SetParticleScriptableObjects(evt.scriptableObjects);
    private void OnAddParticleScriptableObject(ES.AddParticleScriptableObject evt) => AddParticleScriptableObject(evt.scriptableObject);
    private void RequestPlayEffect(ES.PlayParticleEvent evt) => PlayEffect(evt.type, evt.pos, evt.rot, evt.scale);
    
    #endregion Event Wrapper Methods

    #region EffectManager Methods
    
    private void PlayEffect(ParticleType type, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        ParticleSystem ps = particles.Find(x => x.type == type)?.particle;
        if (ps == null)
        {
            Debug.LogError($"존재하지 않는 파티클 타입을 생성하려고 합니다.");
            return;
        }
        
        if (!_pools.TryGetValue(type, out PrefabObjectPool<ParticleSystem> pool))
        {
            _pools.Add(type, new ParticlePool().Initialize(ps, _parent));
        }
        
        _pools[type].Get(out ParticleSystem particle);
        particle.transform.SetPositionAndRotation(pos, rot);
        particle.transform.localScale = scale;
        particle.Play();
    }

    /// <summary>
    /// 풀매니저가 보유한 풀을 초기상태로 되돌리는 메서드입니다.
    /// 씬 전환으로 인해 파괴된 풀링에 필요한 셋팅을 초기화 합니다.
    /// </summary>
    private void ResetPool()
    {
        _parent = new GameObject("Effect").transform;
        
        foreach (var i in _pools.Values)
        {
            i.ClearPool();
        }
        _pools.Clear();
        
        EventHub.Instance.RegisterEvent<ES.PlayParticleEvent>(RequestPlayEffect);
    }

    private void SetParticleScriptableObjects(List<ParticleScriptableObject> scriptableObjects) => particles = scriptableObjects;
    private void AddParticleScriptableObject(ParticleScriptableObject scriptableObject)
    {
        if (particles.Any(x => x.type == scriptableObject.type))
            particles.RemoveAll(x => x.type == scriptableObject.type);

        particles.Add(scriptableObject);
    }
    
    #endregion EffectManager Methods
    
    #region Public Static Methods

    public static ParticleSystem GetParticle(ParticleType type, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (_instance == null)
        {
            Debug.LogError($"접근 가능한 이펙트 매니저가 존재하지 않습니다.");
            return null;
        }
        
        ParticleSystem ps = _instance.particles.Find(x => x.type == type)?.particle;
        if (ps == null)
        {
            Debug.LogError($"존재하지 않는 파티클 타입을 생성하려고 합니다.");
            return null;
        }
        
        if (!_instance._pools.TryGetValue(type, out PrefabObjectPool<ParticleSystem> pool))
        {
            _instance._pools.Add(type, new ParticlePool().Initialize(ps, _instance._parent));
        }
        
        _instance._pools[type].Get(out ParticleSystem particle);
        particle.transform.SetPositionAndRotation(pos, rot);
        particle.transform.localScale = scale;
        return particle;
    }
    
    #endregion Public Static Methods
}
