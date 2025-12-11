namespace _Project.Scripts.EventStructs
{
    public static class OutGameStructs
    {
        #region Shop Events

        /// <summary> 상품 구매 요청 이벤트 </summary>
        public struct RequestTryBuyEvent : IEvent
        {
            public string merchandiseUid;
            
            public RequestTryBuyEvent(string merchandiseUid) => this.merchandiseUid = merchandiseUid;
        }

        /// <summary> 상품 구매 성공 이벤트 </summary>
        public struct BuySuccessEvent : IEvent
        {
            public string merchandiseUid;
            public BuySuccessEvent(string merchandiseUid) => this.merchandiseUid = merchandiseUid;
        }

        /// <summary> 상품 구매 실패 이벤트 </summary>
        public struct BuyFailedEvent : IEvent
        {
            public string merchandiseUid;
            public string error;

            public BuyFailedEvent(string merchandiseUid, string error)
            {
                this.merchandiseUid = merchandiseUid;
                this.error = error;
            }
        }

        #endregion

        #region Deck Events

        /// <summary> 덱 도넛 변경 요청 이벤트 </summary>
        public struct RequestChangeDeckDonutEvent : IEvent
        {
            public int index;
            public string uid;
            
            public RequestChangeDeckDonutEvent(int index, string uid)
            {
                this.index = index;
                this.uid = uid;
            }
        }

        /// <summary> 덱 제빵사 변경 요청 이벤트 </summary>
        public struct RequestChangeDeckBakerEvent : IEvent
        {
            public string uid;
            
            public RequestChangeDeckBakerEvent(string uid) => this.uid = uid;
        }
        
        /// <summary> 수정한 덱 저장 요청 이벤트 </summary>
        public struct RequestSaveCurrentDeckEvent : IEvent {}

        /// <summary> 덱 도넛 변경 성공 이벤트 </summary>
        public struct ChangeDeckDonutSuccessEvent : IEvent
        {
            public int index;
            public string uid;

            public ChangeDeckDonutSuccessEvent(int index, string uid)
            {
                this.index = index;
                this.uid = uid;
            }
        }
        
        /// <summary>덱에서 저장하지 않았을때 초기화이벤트</summary>
        public struct RequestNotSaveDeckEvent : IEvent { }

        #endregion
    }
}
