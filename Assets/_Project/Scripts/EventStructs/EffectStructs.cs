using _Project.Scripts.EffectSystem;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.EventStructs
    {
        public static class EffectStructs
        {
            /// <summary>
            /// 파티클 시스템으로 된 이펙트를 재생하기 위한 이벤트 입니다.
            /// 해당 이벤트를 통해 생성된 이펙트는 파티클 시스템의 종료와 함께 풀로 다시 회수됩니다.
            /// </summary>
            public struct PlayParticleEvent : IEvent
            {
                public ParticleType type;
                public Vector3 pos;
                public Quaternion rot;
                public Vector3 scale;

                public PlayParticleEvent(ParticleType type, Vector3 pos, Quaternion rot, Vector3 scale)
                {
                    this.type = type;
                    this.pos = pos;
                    this.rot = rot;
                    this.scale = scale;
                }

                public PlayParticleEvent(ParticleType type, Vector3 pos, Quaternion rot)
                {
                    this.type = type;
                    this.pos = pos;
                    this.rot = rot;
                    scale = Vector3.one;
                }
            }

            public struct SetParticleScriptableObjects : IEvent
            {
                public List<ParticleScriptableObject> scriptableObjects;
                
                public SetParticleScriptableObjects(List<ParticleScriptableObject> scriptableObjects) => this.scriptableObjects = scriptableObjects;
            }

            public struct AddParticleScriptableObject : IEvent
            {
                public ParticleScriptableObject scriptableObject;
                public AddParticleScriptableObject(ParticleScriptableObject scriptableObject) => this.scriptableObject = scriptableObject;
            }
        }
    }
    