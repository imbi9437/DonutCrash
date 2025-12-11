using UnityEngine;

namespace _Project.Scripts.EventStructs
{
    public static class SkillTextStructs
    {
        public struct RequestSkillText : IEvent
        {
            public string str;
            public bool isAdd;
            public Vector3 pos;

            public RequestSkillText(string str, bool isAdd, Vector3 pos)
            {
                this.str = str;
                this.isAdd = isAdd;
                this.pos = pos;
            }
        }
    }
}
