using UnityEngine;

namespace _Project.Scripts.AudioSystem.AudioChannel
{
    public class MusicAudioChannel : AudioChannel
    {
        private AudioTweenReleaser _prevReleaser;
    
        protected override AudioSource OnCreateObject()
        {
            GameObject obj = new (mixerType.ToString());
            obj.transform.SetParent(parent);
            AudioSource source = obj.AddComponent<AudioSource>();
            AudioTweenReleaser releaser = obj.AddComponent<AudioTweenReleaser>();
            releaser.Initialize(source, this);
            source.playOnAwake = false;
            source.loop = true;
            source.outputAudioMixerGroup = mixerGroup;
            return source;
        }

        protected override void OnGetObject(AudioSource source)
        {
            _prevReleaser?.FadeOutAndRelease(.5f);
            _prevReleaser = source.GetComponent<AudioTweenReleaser>();
            source.gameObject.SetActive(true);
        }

        protected override void OnReleaseObject(AudioSource source)
        {
            source.Stop();
            source.gameObject.SetActive(false);
        }

        protected override void OnDestroyObject(AudioSource source)
        {
            Object.Destroy(source.gameObject);
        }
    }
}
