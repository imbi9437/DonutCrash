using UnityEngine;

namespace _Project.Scripts.AudioSystem.AudioChannel
{
    public class DefaultAudioChannel : AudioChannel
    {
        protected override AudioSource OnCreateObject()
        {
            GameObject obj = new (mixerType.ToString());
            obj.transform.SetParent(parent);
            AudioSource source = obj.AddComponent<AudioSource>();
            AudioAutoReleaser releaser = obj.AddComponent<AudioAutoReleaser>();
            releaser.Initialize(source, this);
            source.playOnAwake = false;
            source.loop = false;
            source.outputAudioMixerGroup = mixerGroup;
            return source;
        }

        protected override void OnGetObject(AudioSource source)
        {
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
