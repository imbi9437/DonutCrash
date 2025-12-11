using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchandiseButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Image merchandiseIcon;
    [SerializeField] private TMP_Text merchandiseName;
    
    [SerializeField] private Image priceIcon;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text stockText;

    private string merchandiseId;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
        EventHub.Instance.RegisterEvent<OutGameStructs.BuySuccessEvent>(OnSuccessBuy);
    }

    public void Initialize(string merchandiseId)
    {
        this.merchandiseId = merchandiseId;
        
        DataManager.Instance.TryGetMerchandiseData(merchandiseId, out var data);
        int purchased = DataManager.Instance.PurchaseInfo.GetValueOrDefault(merchandiseId, 0);
        
        merchandiseName.text = data.merchandiseName;
        
        if (string.IsNullOrWhiteSpace(data.resourcePath) == false)
            AddressableLoader.AssetLoadByPath<Sprite>($"{data.resourcePath}", SetSprite).Forget();
        else SetSprite(null);
        
        AddressableLoader.AssetLoadByPath<Sprite>($"{data.priceType}", x => priceIcon.sprite = x).Forget();

        if (data.priceType == PriceType.Money)
        {
            IAPManager.Instance.TryGetProductPrice(data.uid, out var price);
            priceText.text = price;
        }
        else
            priceText.text = data.price.ToString();

        if (data.stockCount <= 0) stockText.text = "";
        else
        {
            stockText.color = purchased >= data.stockCount ? Color.red : Color.green;
            stockText.text = $"{purchased}/{data.stockCount}";
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
        
        EventHub.Instance?.UnRegisterEvent<OutGameStructs.BuySuccessEvent>(OnSuccessBuy);
    }

    private void OnSuccessBuy(OutGameStructs.BuySuccessEvent evt)
    {
        if (evt.merchandiseUid != merchandiseId) return;
        DataManager.Instance.TryGetMerchandiseData(merchandiseId, out var data);
        int purchased = DataManager.Instance.PurchaseInfo[merchandiseId];
        
        if (data.stockCount <= 0) stockText.text = "";
        else
        {
            stockText.color = purchased >= data.stockCount ? Color.red : Color.green;
            stockText.text = $"{purchased}/{data.stockCount}";
        }
    }
    
    private void OnButtonClick()
    {
        if (string.IsNullOrEmpty(merchandiseId)) return;

        var param = new TwoButtonParam(merchandiseName.text, "구매하시겠습니까?", "예", "아니요");
        param.confirm = () => EventHub.Instance.RaiseEvent(new OutGameStructs.RequestTryBuyEvent(merchandiseId));
        
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup,param));
    }

    private void SetSprite(Sprite sprite)
    {
        merchandiseIcon.color = sprite ? Color.white : Color.clear;
        
        if (sprite) merchandiseIcon.sprite = sprite;
    }
}
