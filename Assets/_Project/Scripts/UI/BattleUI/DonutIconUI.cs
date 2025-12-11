using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DonutIconUI : MonoBehaviour
{
    [SerializeField] private Image image;
    
    private DonutInstanceData _target;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickIconButton);
    }

    public void Initialize(DonutInstanceData donut)
    {
        _target = donut;
        if (donut == null)
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
            return;
        }
        DataManager.Instance.TryGetDonutData(donut.origin, out DonutData donutData);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
        {
            image.sprite = x;
            image.color = new Color(1, 1, 1, 1);
        }).Forget();
    }

    private void OnClickIconButton()
    {
        if (_target != null)
            EventHub.Instance.RaiseEvent(new UIStructs.PrintDonutStateEvent(_target));
    }
}
