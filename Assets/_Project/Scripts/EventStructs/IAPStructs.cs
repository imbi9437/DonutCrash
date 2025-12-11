using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace _Project.Scripts.EventStructs
{
    public static class IAPStructs
    {
        public struct OnCompleteStoreConnection : IEvent { }

        public struct RequestFetchProductList : IEvent
        {
            public string[] productIds;
            public int[] type;
            
            public RequestFetchProductList(string[] productIds, int[] type)
            {
                this.productIds = productIds;
                this.type = type;
            }
        }
        public struct RequestPurchaseProduct : IEvent
        {
            public string productId;
            
            public RequestPurchaseProduct(string productId)
            {
                this.productId = productId;
            }
        }
        public struct RequestFetchPurchases : IEvent { }
        
        
        public struct OnStoreDisconnected : IEvent { }

        public struct OnFetchedProducts : IEvent
        {
            public List<Product> products;
            
            public OnFetchedProducts(List<Product> products) => this.products = products;
        }

        public struct OnPurchasePending : IEvent
        {
            public List<string> productIds;
            
            public OnPurchasePending(List<string> productIds) => this.productIds = productIds;
        }
    }
}
