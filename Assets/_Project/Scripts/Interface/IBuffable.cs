using _Project.Scripts.BuffSystem;
using UnityEngine;

namespace _Project.Scripts.Interface
{
    public interface IBuffable : IDamageable
    {
        public void AddBuff(BuffBase buff);
        public void RemoveBuff(BuffBase buff);
        
        public int GetAtk();
        public int GetDef();
        public int GetHp();
        public float GetSlingShotPower();
        public float GetCurrentHp();
    }
}
