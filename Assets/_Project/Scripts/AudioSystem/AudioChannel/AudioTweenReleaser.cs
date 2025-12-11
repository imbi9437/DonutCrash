using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.AudioSystem.AudioChannel
{
    public class AudioTweenReleaser : MonoBehaviour
    {
        private AudioSource _source;
        private AudioChannel _channel;
        
        public void Initialize(AudioSource source, AudioChannel channel)
        {
            _source = source;
            _channel = channel;
        }

        public void FadeOutAndRelease(float duration)
        {
            _source
                .DOFade(0f, duration)
                .OnComplete(() =>
                {
                    _channel.ReleaseAudioSource(_source);
                });
        }
    }
}
