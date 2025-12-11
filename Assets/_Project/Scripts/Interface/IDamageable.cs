using _Project.Scripts.EffectSystem;
using _Project.Scripts.InGame;
using UnityEngine;

namespace _Project.Scripts.Interface
{
    public interface IDamageable
    {
        public InGameOwner Owner { get; }
        public void TakeDamage(float damage, bool isCrit, IDamageable source);
        public void TakeHeal(float heal);

        public string GetUid();
        public Vector3 GetPosition();
        
        public void SendSkillText(string str, bool isAdd);
        public void SendSkillEffect(ParticleType type);
    }
}
