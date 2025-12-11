using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.EffectSystem
{
    public class ParticlePool : PrefabObjectPool<ParticleSystem>
    {
        protected override ParticleSystem OnCreate()
        {
            ParticleSystem ps = base.OnCreate();
            ParticleAutoReleaser par = ps.AddComponent<ParticleAutoReleaser>();
            par.Initialize(ps, this);
            return ps;
        }

        protected override void OnRelease(ParticleSystem go)
        {
            go.Stop();
            base.OnRelease(go);
        }
    }
}
