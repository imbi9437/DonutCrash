using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

using IAPEvent = _Project.Scripts.EventStructs.IAPStructs;
using DE = _Project.Scripts.EventStructs.DataStructs;

public class IAPManager : MonoSingleton<IAPManager>
{
    public StoreController store;
    
    private List<Product> products;
    
    private void Start()
    {
        EventHub.Instance.RegisterEvent<DE.CompleteLoadDataEvent>(OnCompleteLoadData);
        EventHub.Instance.RegisterEvent<IAPStructs.RequestPurchaseProduct>(OnRequestPurchaseProduct);
        
        InitializeIAP().Forget();
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DE.CompleteLoadDataEvent>(OnCompleteLoadData);
        EventHub.Instance?.UnRegisterEvent<IAPStructs.RequestPurchaseProduct>(OnRequestPurchaseProduct);
    }
    
    private void OnCompleteLoadData(DE.CompleteLoadDataEvent evt) => CacheAllProducts();
    
    private void OnRequestPurchaseProduct(IAPStructs.RequestPurchaseProduct evt) => Purchase(evt.productId);

    private async UniTask InitializeIAP()
    {
        store = UnityIAPServices.StoreController();

        store.OnStoreDisconnected += OnStoreDisconnected;
        
        store.OnProductsFetched += OnStoreProductsFetched;
        store.OnProductsFetchFailed += OnStoreProductsFetchedFailed;
        store.OnPurchasesFetched += OnPurchasesFetched;
        store.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        
        store.OnPurchasePending += OnPurchasePending;
        store.OnPurchaseFailed += OnPurchaseFailed;
        store.OnPurchaseConfirmed += OnPurchaseConfirmed;
        
        await store.Connect();
        
        EventHub.Instance.RaiseEvent(new IAPEvent.OnCompleteStoreConnection());
    }

    private void CacheAllProducts()
    {
        var table = DataManager.Instance.GetMerchandiseTable();
        
        var list = new List<ProductDefinition>();
        foreach (var data in table)
        {
            if (data.priceType != PriceType.Money) continue;
            
            var definition = new ProductDefinition(data.uid, UnityEngine.Purchasing.ProductType.Consumable);
            list.Add(definition);
        }
        
        store.FetchProducts(list);
    }

    private void Purchase(string uid)
    {
        var target = products.FirstOrDefault(p => p.definition.id == uid);
        
        if (target == null) return;
            
        store.PurchaseProduct(target);
    }
    
    #region IAP Callbacks
    
    /// <summary> 스토어 연결 끊김 콜백 </summary>
    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        Debug.Log($"<color=red>[IAPManager] Store Disconnected : {description.message}</color>");
    }
    
    /// <summary> 스토어 상품 정보 호출 성공 콜백 </summary>
    private void OnStoreProductsFetched(List<Product> products)
    {
        Debug.Log($"<color=green>[IAPManager] Products Fetched</color>");
        
        this.products = products;
    }
    
    /// <summary> 스토어 상품 정보 호출 실패 콜백 </summary>
    private void OnStoreProductsFetchedFailed(ProductFetchFailed failure)
    {
        Debug.Log($"<color=red>[IAPManager] Products Fetch Failed : [{failure.FailureReason}</color>");
    }

    /// <summary> 스토어 상품 구매 정보 호출 성공 콜백 </summary>
    private void OnPurchasesFetched(Orders order)
    {
        Debug.Log($"<color=green>[IAPManager] Purchases Fetched</color>");
    }
    
    /// <summary> 스토어 상품 구매 정보 호출 실패 콜백 </summary>
    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription description)
    {
        Debug.Log($"<color=red>[IAPManager] Purchases Fetch Failed : {description.message}</color>");
    }
    
    /// <summary> 구매 완료 후 처리 대기 콜백 </summary>
    private void OnPurchasePending(PendingOrder order)
    {
        if (order.CartOrdered.Items().Count <= 0) return;
        
        Debug.Log($"<color=green>[IAPManager] Purchase Pending : {order.CartOrdered.Items().First().Product.definition.id}</color>");

        var list = order.CartOrdered.Items().Select(i => i.Product.definition.id).ToList();
        EventHub.Instance.RaiseEvent(new IAPStructs.OnPurchasePending(list));
        
        store.ConfirmPurchase(order);
    }
    
    /// <summary> 구매 실패 처리 콜백 </summary>
    private void OnPurchaseFailed(FailedOrder order)
    {
        Debug.Log($"<color=red>[IAPManager] Purchase Failed : {order.FailureReason.ToString()}</color>");

        var param = new OneButtonParam("구매 실패", "해당 아이템 구매에 실패하셨습니다.","돌아가기");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
    }

    /// <summary> 구매 처리 완료 콜백 </summary>
    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log($"<color=green>[IAPManager] Purchase Confirmed");
    }
    
    #endregion

    public bool TryGetProductPrice(string productId, out string productPrice)
    {
        productPrice = string.Empty;
        if (products == null || products.Count <= 0) return false;
        
        var product = products.FirstOrDefault(p => p.definition.id == productId);
        if (product == null) return false;
        
        productPrice = product.metadata.localizedPriceString;
        return true;
    }
}
