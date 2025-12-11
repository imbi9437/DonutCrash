using _Project.Scripts.EventStructs;

namespace _Project.Scripts.EventStructs
{
    public static class CameraAnimationStructs
    {
        public struct RequestCameraAnimation : IEvent
        {
            public string clip;
            public float weight;

            public RequestCameraAnimation(string animation, float weight)
            {
                this.clip = animation;
                this.weight = weight;
            }
        }
    }
}
