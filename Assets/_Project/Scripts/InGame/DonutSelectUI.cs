using _Project.Scripts.EventStructs;
using _Project.Scripts.InGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;

public class DonutSelectUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI donutNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI criText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private Button selectButton;
    
    private InGameOwner _owner;

    private DonutInstanceData _selectedDonut;
    
    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.CompleteSelectDonut>(OnSelectDonut);
    }

    private void OnSelectDonut(IGS.CompleteSelectDonut evt) => Destroy(gameObject);
    
    public void Setup(InGameOwner selector, DonutInstanceData data)
    {
        _owner = selector;
        _selectedDonut = data;
        
        DataManager.Instance.TryGetDonutData(data.origin, out DonutData donutData);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
        {
            iconImage.sprite = x;
        }).Forget();
        
        donutNameText.text = donutData.donutName;
        levelText.text = $"Lv. {data.level}";
        hpText.text = $"Health : {data.hp.ToString()}";
        atkText.text = $"Attack : {data.atk.ToString()}";
        defText.text = $"Defense : {data.def.ToString()}";
        criText.text = $"Critical : {data.crit.ToPercent()}%";
        skillDescriptionText.text = donutData.donutDescription;
        
        selectButton.onClick.RemoveAllListeners(); 
        selectButton.onClick.AddListener(OnClickSelectButton);
        
        EventHub.Instance?.RegisterEvent<IGS.CompleteSelectDonut>(OnSelectDonut);
    }
    
    private void OnClickSelectButton()
    {
        EventHub.Instance.RaiseEvent(new PS.RequestSpawnDonut(_owner, _selectedDonut));
    }
}