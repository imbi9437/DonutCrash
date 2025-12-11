using UnityEngine;

[CreateAssetMenu(fileName = "ComponentChangerSettings", menuName = "Tools/ComponentChangerSettings")]
public class ComponentChangerSettings : ScriptableObject
{
    public string[] filePath;
    public Sprite oldOne;
    public Sprite newOne;
}
