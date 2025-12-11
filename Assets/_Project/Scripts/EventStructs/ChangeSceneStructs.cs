namespace _Project.Scripts.EventStructs
{
    public static class ChangeSceneStructs
    {
        public struct RequestChangeSceneEvent : IEvent
        {
            public string sceneName;
            public int index;
            public bool isDirect;
            public float delay;
        
            public RequestChangeSceneEvent(string sceneName = null, int index = -1, bool isDirect = false, float delay = 0f)
            {
                this.sceneName = sceneName;
                this.index = index;
                this.isDirect = isDirect;
                this.delay = delay;
            }
        }

        public struct RequestChangeTargetSceneEvent : IEvent
        {
            public float delay;

            public RequestChangeTargetSceneEvent(float delay = 0f)
            {
                this.delay = delay;
            }
        }
        
        
        public struct StartLoadSceneEvent : IEvent { }

        public struct UpdateLoadSceneEvent : IEvent
        {
            public float progress;
            public UpdateLoadSceneEvent(float progress)
            {
                this.progress = progress;
            }
        }

        public struct CompleteLoadSceneEvent : IEvent
        {
            public int sceneIndex;
            
            public CompleteLoadSceneEvent(int sceneIndex) => this.sceneIndex = sceneIndex;
        }

        public struct FailLoadSceneEvent : IEvent
        {
            public string message;
            public FailLoadSceneEvent(string message) => this.message = message;
        }
    }
}
