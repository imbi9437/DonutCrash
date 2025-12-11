using _Project.Scripts.EventStructs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

public class DonutInfoPopup : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI donutName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI critical;
    [SerializeField] private TextMeshProUGUI description;
    
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickCloseButton);
        gameObject.SetActive(false);
        
        EventHub.Instance?.RegisterEvent<UIStructs.PrintDonutStateEvent>(RequestPrintDonutState);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<UIStructs.PrintDonutStateEvent>(RequestPrintDonutState);
    }

    private void RequestPrintDonutState(UIStructs.PrintDonutStateEvent evt) => Initialize(evt.data);

    private void Initialize(DonutInstanceData instanceData)
    {
        DataManager.Instance.TryGetDonutData(instanceData?.origin, out DonutData donutData);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
        {
            icon.sprite = x;
        }).Forget();
        
        donutName.text = donutData?.donutName ?? "UnknownDonut";
        level.text = $"level : {instanceData?.level}";
        health.text = $"health : {instanceData?.hp}";
        attack.text = $"attack : {instanceData?.atk}";
        defense.text = $"defense : {instanceData?.def}";
        critical.text = $"critical : {instanceData?.crit.ToPercent()}%";
        description.text = donutData?.donutDescription ?? "UnknownDonutDescription";
        
        gameObject.SetActive(true);
        
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.SFX_UI_08, 1f));
    }

    private void OnClickCloseButton()
    {
        gameObject.SetActive(false);
        
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.SFX_UI_09, 1f));
    }
}
