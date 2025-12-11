using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace _Project.Scripts.AudioSystem.AudioChannel
{
    public abstract class AudioChannel
    {
        private ObjectPool<AudioSource> _audioSourcePool;
        protected Transform parent;
        protected MixerType mixerType;
        protected AudioMixerGroup mixerGroup;

        public virtual AudioChannel Initialize(MixerType mixerType, AudioMixerGroup mixerGroup, Transform parent)
        {
            if (_audioSourcePool != null)
            {
                Debug.LogError($"이미 초기화된 오브젝트 풀을 초기화려고 시도합니다\n한번 초기화된 오브젝트 풀은 다시 초기화할 수 없습니다.");
                return this;
            }
        
            this.mixerType = mixerType;
            this.mixerGroup = mixerGroup;
            this.parent = parent;
            _audioSourcePool = new ObjectPool<AudioSource>(OnCreateObject, OnGetObject, OnReleaseObject, OnDestroyObject);
            return this;
        }

        public void Get(out AudioSource source) => _audioSourcePool.Get(out source);
        public void ReleaseAudioSource(AudioSource source) => _audioSourcePool.Release(source);
        public void ClearPool() => _audioSourcePool.Clear();

        protected abstract AudioSource OnCreateObject();
        protected abstract void OnGetObject(AudioSource source);
        protected abstract void OnReleaseObject(AudioSource source);
        protected abstract void OnDestroyObject(AudioSource source);
    }
}
