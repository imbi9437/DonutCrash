using _Project.Scripts.AudioSystem;
using _Project.Scripts.AudioSystem.AudioChannel;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using CSS = _Project.Scripts.EventStructs.ChangeSceneStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Space]
    [Header("Audio Data")]
    [SerializeField] private List<AudioScriptableObject> audioScriptableObjects;

    private readonly Dictionary<MixerType, AudioChannel> _audioChannels  = new ();
    private readonly Dictionary<MixerType, AudioMixerGroup> _mixerGroups = new ();

    private AudioMixer _mixer;

    private void Start()
    {
        _mixer = Resources.Load<AudioMixer>("AudioMixer");
        
        _mixerGroups.Add(MixerType.Master, _mixer.FindMatchingGroups("Master")[0]);
        _mixerGroups.Add(MixerType.Music, _mixer.FindMatchingGroups("Music")[0]);
        _mixerGroups.Add(MixerType.Sfx, _mixer.FindMatchingGroups("SFX")[0]);
        _mixerGroups.Add(MixerType.UI, _mixer.FindMatchingGroups("UI")[0]);
        
        _audioChannels.Add(MixerType.Music, new MusicAudioChannel().Initialize(MixerType.Music, _mixerGroups[MixerType.Music], transform));
        _audioChannels.Add(MixerType.Sfx, new DefaultAudioChannel().Initialize(MixerType.Sfx, _mixerGroups[MixerType.Sfx] ,transform));
        _audioChannels.Add(MixerType.UI, new DefaultAudioChannel().Initialize(MixerType.UI, _mixerGroups[MixerType.UI], transform));
        
        LoadVolume();
        
        EventHub.Instance.RegisterEvent<CSS.StartLoadSceneEvent>(OnStartLoadScene);
        EventHub.Instance.RegisterEvent<CSS.CompleteLoadSceneEvent>(OnCompleteLoadScene);
        RegisterAudioEvents();
        
        EventHub.Instance?.RaiseEvent(new AS.PlayBackAudioEvent(AudioType.BGM_01, .5f));
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    // 오디오또한 3D 음향효과를 주고싶다면 이펙트와 같이 게임 씬에 배치되어야 합니다.
    // 그럴경우 씬 전환간에 풀과 풀링 오브젝트의 예외처리가 필요합니다.
    // 해당 예외처리가 필요하다면 아래의 두 이벤트 메서드 내, 이벤트 허브에 이벤트 등록 및 해지의 주석을 해지하여 오류를 방지해 주세요.
    private void OnStartLoadScene(CSS.StartLoadSceneEvent evt)
    {
        // UnRegisterEvents();
    }

    private void OnCompleteLoadScene(CSS.CompleteLoadSceneEvent evt)
    {
        if (_audioChannels == null)
        {
            Debug.LogError($"오디오 매니저가 초기화되지 않았습니다.");
            return;
        }

        // 현재의 오디오는 모든 오브젝트가 파괴되지 않는 씬에 올라가 풀에 회수된 오브젝트가 회손되지 않습니다.
        // 따라서 풀에 회수된 오브젝트 스택을 초기화 하는 구문 없이 회수된 오브젝트만 초기화 합니다.
        foreach (var i in _audioChannels.Values)
        {
            i?.ClearPool();
        }
        
        // RegisterEvents();
    }

    private void Play(AS.PlayBackAudioEvent evt)
    {
        if (!TryGetAudioScriptable(evt.type, out AudioScriptableObject aso))
        {
            Debug.LogError("오디오 타입에 해당하는 오디오가 존재하지 않습니다.");
            return;
        }
        
        _audioChannels[MixerType.Music].Get(out AudioSource source);
        source.clip = aso.clip;
        source.volume = 0f;
        source.DOFade(evt.volume, .5f);
        source.Play();
    }

    private void Play(AS.PlaySfxAudioEvent evt)
    {
        if (!TryGetAudioScriptable(evt.type, out AudioScriptableObject aso))
        {
            Debug.LogError("오디오 타입에 해당하는 오디오가 존재하지 않습니다.");
            return;
        }
            
        _audioChannels[MixerType.Sfx].Get(out AudioSource source);
        source.clip = aso.clip;
        source.volume = evt.volume;
        source.outputAudioMixerGroup = _mixerGroups[MixerType.Sfx];
        source.Play();
    }

    private void Play(AS.PlayUiAudioEvent evt)
    {
        if (!TryGetAudioScriptable(evt.type, out AudioScriptableObject aso))
        {
            Debug.LogError("오디오 타입에 해당하는 오디오가 존재하지 않습니다.");
            return;
        }
        
        _audioChannels[MixerType.UI].Get(out AudioSource source);
        source.clip = aso.clip;
        source.volume = evt.volume;
        source.outputAudioMixerGroup = _mixerGroups[MixerType.UI];
        source.Play();
    }

    private bool TryGetAudioScriptable(AudioType type, out AudioScriptableObject aso)
    {
        aso = audioScriptableObjects.Find(x => x.type == type);
        return aso;
    }

    private void RegisterAudioEvents()
    {
        EventHub.Instance.RegisterEvent<AS.PlayBackAudioEvent>(Play);
        EventHub.Instance.RegisterEvent<AS.PlaySfxAudioEvent>(Play);
        EventHub.Instance.RegisterEvent<AS.PlayUiAudioEvent>(Play);
        
        EventHub.Instance.RegisterEvent<AS.ChangeMixerVolume>(ChangeVolume);
    }

    private void UnRegisterEvents()
    {
        EventHub.Instance?.UnRegisterEvent<AS.PlayBackAudioEvent>(Play);
        EventHub.Instance?.UnRegisterEvent<AS.PlaySfxAudioEvent>(Play);
        EventHub.Instance?.UnRegisterEvent<AS.PlayUiAudioEvent>(Play);
        
        EventHub.Instance?.UnRegisterEvent<AS.ChangeMixerVolume>(ChangeVolume);
    }
    
    private void ChangeVolume(AS.ChangeMixerVolume evt)
    {
        var mixer = _mixerGroups[evt.mixerType];
        var db = Mathf.Log10(Mathf.Clamp(evt.volume, 0.0001f,1f)) * 20;

        PlayerPrefs.SetFloat($"volume_{evt.mixerType}", evt.volume);
        mixer.audioMixer.SetFloat(evt.mixerType.ToString(), db);
    }
    public float GetVolume(MixerType mixerType)
    {
        var mixer = _mixerGroups[mixerType];
        
        if (mixer == null) return -1f;
        
        mixer.audioMixer.GetFloat(mixerType.ToString(), out var db);
        
        Debug.Log($"=====Load : volume_{mixerType} {db}");
        return Mathf.Pow(10, db / 20);
    }

    private void LoadVolume()
    {
        foreach (var mixerGroup in _mixerGroups)
        {
            var volume = PlayerPrefs.GetFloat($"volume_{mixerGroup.Key}", 1f);
            var evt = new AS.ChangeMixerVolume(mixerGroup.Key, volume);
            ChangeVolume(evt);
        }
    }
}
