using UnityEngine;

namespace _Project.Scripts.EffectSystem
{
    [CreateAssetMenu(fileName = "New Particle Scriptable Object", menuName = "Effect System/Scriptable Object/Particle")]
    public class ParticleScriptableObject : ScriptableObject
    {
        public ParticleType type;
        public ParticleSystem particle;
    }
}
