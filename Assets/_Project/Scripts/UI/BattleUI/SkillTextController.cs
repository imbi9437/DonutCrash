using UnityEngine;

using STS = _Project.Scripts.EventStructs.SkillTextStructs;

public class SkillTextController : MonoBehaviour
{
    [SerializeField] private SkillIndicator prefab;

    private void Start()
    {
        EventHub.Instance.RegisterEvent<STS.RequestSkillText>(RequestSkillText);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<STS.RequestSkillText>(RequestSkillText);
    }

    private void RequestSkillText(STS.RequestSkillText evt) => SkillText(evt.str, evt.isAdd, evt.pos);

    private void SkillText(string str, bool isAdd, Vector3 position)
    {
        SkillIndicator si = Instantiate(prefab, position, Quaternion.identity);
        si.Initialize(str, isAdd);
    }
}
