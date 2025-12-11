using _Project.Scripts.EventStructs;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum ShopCategoryType
{
    Package = 0,
    DiamondExchange = 1,
    GoldExchange = 2,
    Normal = 3,
    Premium = 4,
}

public class ShopCategoryUIController : UIController
{
    [SerializeField] private Button packageButton;
    [SerializeField] private Button diamondExchangeButton;
    [SerializeField] private Button goldExchangeButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button premiumButton;

    protected override void Awake()
    {
        base.Awake();
        
        packageButton.onClick.AddListener(() => ChangePanel(ShopCategoryType.Package));
        diamondExchangeButton.onClick.AddListener(() => ChangePanel(ShopCategoryType.DiamondExchange));
        goldExchangeButton.onClick.AddListener(() => ChangePanel(ShopCategoryType.GoldExchange));
        normalButton.onClick.AddListener(() => ChangePanel(ShopCategoryType.Normal));
        premiumButton.onClick.AddListener(() => ChangePanel(ShopCategoryType.Premium));
        
        EventHub.Instance?.RegisterEvent<UIStructs.RequestOpenShopCategoryEvent>(OnRequestOpenCategory);
    }
    
    private void OnDestroy()
    {
        packageButton.onClick.RemoveListener(() => ChangePanel(ShopCategoryType.Package));
        diamondExchangeButton.onClick.RemoveListener(() => ChangePanel(ShopCategoryType.DiamondExchange));
        goldExchangeButton.onClick.RemoveListener(() => ChangePanel(ShopCategoryType.GoldExchange));
        normalButton.onClick.RemoveListener(() => ChangePanel(ShopCategoryType.Normal));
        premiumButton.onClick.RemoveListener(() => ChangePanel(ShopCategoryType.Premium));
        
        EventHub.Instance?.UnRegisterEvent<UIStructs.RequestOpenShopCategoryEvent>(OnRequestOpenCategory);
    }

    private void OnRequestOpenCategory(UIStructs.RequestOpenShopCategoryEvent evt) => ChangePanel(evt.category);
    
    private void OpenPanel(ShopCategoryType type) => OpenPanel((int)type);
    private void HidePanel(ShopCategoryType type) => ClosePanel((int)type);

    private void ChangePanel(ShopCategoryType type)
    {
        if (currentPanel?.PanelType == (int)type) return;
        
        if (currentPanel != null) ClosePanel(currentPanel.PanelType);
        OpenPanel(type);
    }
    
    protected override IEnumerable GetChildPanel()
    {
        return GetComponentsInChildren<ShopCategoryPanel>(true);
    }
}
