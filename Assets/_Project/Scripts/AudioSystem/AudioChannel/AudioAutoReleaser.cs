using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace _Project.Scripts.AudioSystem.AudioChannel
{
    public class AudioAutoReleaser : MonoBehaviour
    {
        private AudioSource _source;
        private AudioChannel _channel;
        private CancellationTokenSource _cts;
    
        public void Initialize(AudioSource source, AudioChannel channel)
        {
            _source = source;
            _channel = channel;
        }

        private void OnEnable()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            AutoRelease(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
        }

        private async UniTaskVoid AutoRelease(CancellationToken ct)
        {
            await UniTask.WaitWhile(IsPlaying, cancellationToken: ct);
            _channel.ReleaseAudioSource(_source);
        }
    
        private bool IsPlaying() => _source.isPlaying;
    }
}
