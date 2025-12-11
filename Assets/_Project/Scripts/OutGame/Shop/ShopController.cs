using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OGS = _Project.Scripts.EventStructs.OutGameStructs;
using DS = _Project.Scripts.EventStructs.DataStructs;

public class ShopController : MonoBehaviour
{
    //========== Table Cache ==========//
    private Dictionary<string, MerchandiseData> merchandiseTable;
    private Dictionary<string, ProductData> productDict;

    //========== new Value ==========//
    private int _newGold;
    private int _newCash;
    private int _newRecipePieces;
    private int _newPerfectRecipes;

    private List<BakerInstanceData> userBakers;
    private List<DonutInstanceData> userDonuts;
    private Dictionary<string, int> userIngredients;
    private Dictionary<string, int> userPurchaseInfo;

    private TrayData userTray;

    //========== Dirty Flag ==========//
    private bool _goldDirty;
    private bool _cashDirty;
    private bool _recipePiecesDirty;
    private bool _perfectRecipesDirty;

    private bool _donutDirty;
    private bool _bakerDirty;
    private bool _ingredientDirty;
    private bool _purchaseDirty;

    private bool _trayDirty;
    
    //========== End Field ==========//
    
    private void Start()
    {
        merchandiseTable = DataManager.Instance.GetMerchandiseTable().ToDictionary(m => m.uid, m => m);
        productDict = DataManager.Instance.GetProductTable().ToDictionary(p => p.uid, p => p);
        
        _newGold = DataManager.Instance.UserGold;
        _newCash = DataManager.Instance.UserCash;
        _newRecipePieces = DataManager.Instance.RecipePieces;
        _newPerfectRecipes = DataManager.Instance.PerfectRecipe;
        
        userBakers = DataManager.Instance.Bakers;
        userDonuts = DataManager.Instance.Donuts;
        userIngredients = DataManager.Instance.Ingredients;
        userPurchaseInfo = DataManager.Instance.PurchaseInfo;

        userTray = DataManager.Instance.TrayData;
        
        EventHub.Instance.RegisterEvent<OGS.RequestTryBuyEvent>(OnRequestTryBuyEvent);
        
        //========== Broad Cast ==========//
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserGoldEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserCashEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserRecipePiecesEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserPerfectRecipeEvent>(OnBroadcast);
        
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserBakerEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserIngredientEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DS.BroadcastSetUserPurchaseInfoEvent>(OnBroadcast);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<OGS.RequestTryBuyEvent>(OnRequestTryBuyEvent);
        
        //========== Broad Cast ==========//
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserGoldEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserCashEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserRecipePiecesEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserPerfectRecipeEvent>(OnBroadcast);
        
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserBakerEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserIngredientEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DS.BroadcastSetUserPurchaseInfoEvent>(OnBroadcast);
    }

    #region Event Rapper Methods

    private void OnRequestTryBuyEvent(OGS.RequestTryBuyEvent evt) => TryBuy(evt.merchandiseUid); 
    
    private void OnBroadcast(DS.BroadcastSetUserGoldEvent evt) => _newGold = DataManager.Instance.UserGold;
    private void OnBroadcast(DS.BroadcastSetUserCashEvent evt) => _newCash = DataManager.Instance.UserCash;
    private void OnBroadcast(DS.BroadcastSetUserRecipePiecesEvent evt) => _newRecipePieces = DataManager.Instance.RecipePieces;
    private void OnBroadcast(DS.BroadcastSetUserPerfectRecipeEvent evt) => _newPerfectRecipes = DataManager.Instance.PerfectRecipe;
    
    private void OnBroadcast(DS.BroadcastSetUserBakerEvent evt) => userBakers = DataManager.Instance.Bakers;
    private void OnBroadcast(DS.BroadcastSetUserDonutEvent evt) => userDonuts = DataManager.Instance.Donuts;
    private void OnBroadcast(DS.BroadcastSetUserIngredientEvent evt) => userIngredients = DataManager.Instance.Ingredients;
    private void OnBroadcast(DS.BroadcastSetUserPurchaseInfoEvent evt) => userPurchaseInfo = DataManager.Instance.PurchaseInfo;
    

    #endregion
    

    private void TryBuy(string uid)
    {
        if (merchandiseTable.TryGetValue(uid, out MerchandiseData merchandise) == false) return;
        if (merchandise == null) return;
        if (CanBuy(merchandise) == false) return;

        if (merchandise.priceType == PriceType.Money)
        {
            TryBuyIAP(uid);
            return;
        }
        
        if (userPurchaseInfo.TryGetValue(uid, out int count) == false) userPurchaseInfo[uid] = 1;
        else userPurchaseInfo[uid] = count + 1;
        _purchaseDirty = true;
        
        PayPrice(merchandise.priceType, merchandise.price);

        foreach (var id in merchandise.productIds)
        {
            GainProduct(id);
        }
        
        UpdateData();
        EventHub.Instance.RaiseEvent(new OGS.BuySuccessEvent(uid));
        var param = new OneButtonParam("구매 성공", "해당 상품을 구매하였습니다.", "돌아가기");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
    }

    private void TryBuyIAP(string uid)
    {
        if (merchandiseTable.TryGetValue(uid, out MerchandiseData merchandise) == false) return;
        if (merchandise == null) return;
        if (CanBuy(merchandise) == false) return;
        
        EventHub.Instance.RegisterEvent<IAPStructs.OnPurchasePending>(BuyAfter);
        EventHub.Instance.RaiseEvent(new IAPStructs.RequestPurchaseProduct(uid));

        void BuyAfter(IAPStructs.OnPurchasePending evt)
        {
            EventHub.Instance.UnRegisterEvent<IAPStructs.OnPurchasePending>(BuyAfter);
            if (userPurchaseInfo.TryGetValue(uid, out int count) == false) userPurchaseInfo[uid] = 1;
            else userPurchaseInfo[uid] = count + 1;
            _purchaseDirty = true;

            foreach (var id in merchandise.productIds)
            {
                GainProduct(id);
            }
        
            UpdateData();
            EventHub.Instance.RaiseEvent(new OGS.BuySuccessEvent(uid));
            var param = new OneButtonParam("구매 성공", "해당 상품을 구매하였습니다.", "돌아가기");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
        }
    }
    
    private bool CanBuy(MerchandiseData item)
    {
        bool hasHistory = userPurchaseInfo.TryGetValue(item.uid, out int count);

        if (hasHistory && count >= item.stockCount && item.stockCount > 0)
        {
            var param = new OneButtonParam("구매 실패", "구매 가능한 개수를 모두 구매하셨습니다.", "돌아가기");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
            
            return false;
        }

        bool requirePrice = item.priceType switch
        {
            PriceType.Gold => DataManager.Instance.UserGold >= item.price,
            PriceType.Cash => DataManager.Instance.UserCash >= item.price,
            PriceType.RecipePieces => DataManager.Instance.RecipePieces >= item.price,
            _ => true
        };

        if (requirePrice == false)
        {
            var param = new OneButtonParam("구매 실패", "금액이 부족합니다.", "돌아가기");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
        }
        
        return requirePrice;
    }
    
    private void PayPrice(PriceType type, int price)
    {
        switch (type)
        {
            case PriceType.Gold:
                _newGold = DataManager.Instance.UserGold - price;
                _goldDirty = true;
                break;
            case PriceType.Cash:
                _newCash = DataManager.Instance.UserCash - price;
                _cashDirty = true;
                break;
            case PriceType.RecipePieces:
                _newRecipePieces = DataManager.Instance.RecipePieces - price;
                _recipePiecesDirty = true;
                break;
        }
    }
    private void GainProduct(string productId)
    {
        if (string.IsNullOrEmpty(productId)) return;
        if (productDict.TryGetValue(productId, out var data) == false) return;
        if (string.IsNullOrEmpty(data.uid)) return;
        if (string.IsNullOrEmpty(data.productId)) return;
        
        switch (data.productType)
        {
            case ProductType.Gold:
                _newGold += data.productValue;
                _goldDirty = true;
                break;
            case ProductType.Cash:
                _newCash += data.productValue;
                _cashDirty = true;
                break;
            case ProductType.Ingredient:
                if (userIngredients.TryGetValue(data.productId, out int _) == false) userIngredients[data.productId] = data.productValue;
                else userIngredients[data.productId] += data.productValue;
                _ingredientDirty = true;
                break;
            case ProductType.Donut:
                DataManager.Instance.TryGetDonutData(data.productId, out DonutData donutData);
                for (int i = 0; i < data.productValue; i++)
                {
                    userDonuts.Add(new DonutInstanceData(donutData));
                }
                _donutDirty = true;
                break;
            case ProductType.Baker:
                DataManager.Instance.TryGetBakerData(data.productId, out BakerData bakerData);
                for (int i = 0; i < data.productValue; i++)
                {
                    userBakers.Add(new BakerInstanceData(bakerData));
                }
                _bakerDirty = true;
                break;
            case ProductType.TrayExpand:
                userTray.grade += data.productValue;
                _trayDirty = true;
                break;
            case ProductType.Recipe:
                _newPerfectRecipes += data.productValue;
                _perfectRecipesDirty = true;
                break;
            case ProductType.RecipePiece:
                _newRecipePieces += data.productValue;
                _recipePiecesDirty = true;
                break;
        }
    }
    
    private void UpdateData()
    {
        if (_goldDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetGoldEvent(_newGold));
        if (_cashDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetCashEvent(_newCash));
        if (_recipePiecesDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetRecipePiecesEvent(_newRecipePieces));
        if (_perfectRecipesDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetPerfectRecipeEvent(_newPerfectRecipes));
        if (_donutDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetDonutListEvent(userDonuts));
        if (_bakerDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetBakerListEvent(userBakers));
        if (_ingredientDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetIngredientEvent(userIngredients));
        if (_purchaseDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetPurchaseInfoEvent(userPurchaseInfo));
        if (_trayDirty) EventHub.Instance.RaiseEvent(new DS.RequestSetTrayEvent(userTray));
        
        ResetDirty();
    }
    private void ResetDirty()
    {
        _goldDirty = false;
        _cashDirty = false;
        _recipePiecesDirty = false;
        _perfectRecipesDirty = false;

        _bakerDirty = false;
        _donutDirty = false;
        _ingredientDirty = false;
        _purchaseDirty = false;

        _trayDirty = false;
    }
}
