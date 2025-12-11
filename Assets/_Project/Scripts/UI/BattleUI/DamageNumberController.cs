using UnityEngine;

using DNC = _Project.Scripts.EventStructs.DamageNumberStructs;

public class DamageNumberController : MonoBehaviour
{
    [SerializeField] private DamageIndicator prefab;

    private void Start()
    {
        EventHub.Instance.RegisterEvent<DNC.RequestDamageNumber>(RequestDamageNumber);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DNC.RequestDamageNumber>(RequestDamageNumber);
    }
    
    private void RequestDamageNumber(DNC.RequestDamageNumber evt) => DamageNumber(evt.damage, evt.isCrit, evt.pos);

    private void DamageNumber(int damage, bool isCrit, Vector3 position)
    {
        DamageIndicator di = Instantiate(prefab, position, Quaternion.identity);
        di.Initialize(damage, isCrit);
    }
}
