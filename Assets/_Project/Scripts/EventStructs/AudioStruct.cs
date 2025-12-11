using _Project.Scripts.AudioSystem;

namespace _Project.Scripts.EventStructs
{
    public static class AudioStruct
    {
        /// <summary>
        /// 배경음을 재생하기 위한 이벤트 구조체
        /// 해당 구조체로 소리를 재생시킬 경우 소리가 반복해서 재생됩니다.
        /// </summary>
        public struct PlayBackAudioEvent : IEvent
        {
            public AudioType type;
            public float volume;
            
            public PlayBackAudioEvent(AudioType type, float volume)
            {
                this.type = type;
                this.volume = volume;
            }
        }

        /// <summary>
        /// 효과음을 재생시키기 위한 이벤트 구조체
        /// 해당 구조체로 소리를 재생시킬 경우 소리를 1회 재생하고 회수합니다.
        /// </summary>
        public struct PlaySfxAudioEvent : IEvent
        {
            public AudioType type;
            public float volume;
            
            public PlaySfxAudioEvent(AudioType type, float volume)
            {
                this.type = type;
                this.volume = volume;
            }
        }

        /// <summary>
        /// 유저 인터페이스 효과음을 재생시키기 위한 이벤트 구조체
        /// 해당 구조체로 소리를 재생시킬 경우 소리를 1회 재생하고 회수합니다.
        /// </summary>
        public struct PlayUiAudioEvent : IEvent
        {
            public AudioType type;
            public float volume;

            public PlayUiAudioEvent(AudioType type, float volume)
            {
                this.type = type;
                this.volume = volume;
            }
        }
        
        public struct ChangeMixerVolume : IEvent
        {
            public MixerType mixerType;
            public float volume;

            public ChangeMixerVolume(MixerType mixerType, float volume)
            {
                this.mixerType = mixerType;
                this.volume = volume;
            }
        }
    }
}
