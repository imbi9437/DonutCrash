using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Scriptable Object", menuName = "Audio System/Scriptable Object/Audio")]
public class AudioScriptableObject : ScriptableObject
{
    [SerializeField] public AudioType type;
    [SerializeField] public AudioClip clip;
}
