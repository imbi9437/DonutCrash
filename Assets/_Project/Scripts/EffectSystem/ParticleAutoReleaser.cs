using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace _Project.Scripts.EffectSystem
{
    public class ParticleAutoReleaser : MonoBehaviour
    {
        private ParticleSystem _particle;
        private PrefabObjectPool<ParticleSystem> _pool;
        private CancellationTokenSource _cts;

        public void Initialize(ParticleSystem particle, PrefabObjectPool<ParticleSystem> pool)
        {
            _particle = particle;
            _pool = pool;
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
            await UniTask.WaitWhile(() => _particle.IsAlive(true), cancellationToken: ct);
            _pool.Release(_particle);
        }
    }
}
