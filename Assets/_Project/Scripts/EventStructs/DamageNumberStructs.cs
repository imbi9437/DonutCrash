using UnityEngine;

namespace _Project.Scripts.EventStructs
{
    public static class DamageNumberStructs
    {
        public struct RequestDamageNumber : IEvent
        {
            public int damage;
            public bool isCrit;
            public Vector3 pos;

            public RequestDamageNumber(int damage, bool isCrit, Vector3 pos)
            {
                this.damage = damage;
                this.isCrit = isCrit;
                this.pos = pos;
            }
        }
    }
}
