using System.Collections.Generic;

namespace _Project.Scripts.EventStructs
{
    public static class DataStructs
    {
        public struct RequestCreateNewDataEvent : IEvent
        {
            public string uid;

            public RequestCreateNewDataEvent(string uid)
            {
                this.uid = uid;
            }
        }
        public struct CompleteCreateNewDataEvent : IEvent { }
        public struct CompleteLoadUserDataEvent : IEvent { }
        public struct CompleteLoadTableDataEvent : IEvent { }
        public struct CompleteLoadDataEvent : IEvent { }
        
        #region Request Set UserData Event

        public struct RequestSetNickNameEvent : IEvent
        {
            public string newValue;
            public RequestSetNickNameEvent(string newValue) => this.newValue = newValue;
        }
        
        public struct RequestSetProfileImageEvent : IEvent
        {
            public string newValue;
            public RequestSetProfileImageEvent(string newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 에너지 변경 요청 이벤트 </summary>
        public struct RequestSetEnergyEvent : IEvent
        {
            public int newValue;
            public RequestSetEnergyEvent(int newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 인게임 재화 (골드) 변경 요청 이벤트 </summary>
        public struct RequestSetGoldEvent : IEvent
        {
            public int newValue;
            public RequestSetGoldEvent(int newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 유료 재화 (다이아) 변경 요청 이벤트 </summary>
        public struct RequestSetCashEvent : IEvent
        {
            public int newValue;
            public RequestSetCashEvent(int newValue) => this.newValue = newValue;
        }

        /// <summary> 유저데이터 레시피 조각 변경 요청 이벤트 </summary>
        public struct RequestSetRecipePiecesEvent : IEvent
        {
            public int newValue;
            public RequestSetRecipePiecesEvent(int newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 완벽한 레시피 변경 요청 이벤트 </summary>
        public struct RequestSetPerfectRecipeEvent : IEvent
        {
            public int newValue;
            public RequestSetPerfectRecipeEvent(int newValue) => this.newValue = newValue;
        }

        /// <summary> 유저데이터 도넛 리스트 변경 요청 이벤트 </summary>
        public struct RequestSetDonutListEvent : IEvent
        {
            public List<DonutInstanceData> newValue;
            public RequestSetDonutListEvent(List<DonutInstanceData> newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 제빵사 리스트 변경 요청 이벤트 </summary>
        public struct RequestSetBakerListEvent : IEvent
        {
            public List<BakerInstanceData> newValue;
            public RequestSetBakerListEvent(List<BakerInstanceData> newValue) => this.newValue = newValue;
        }

        /// <summary> 유저데이터 재료 데이터 변경 요청 이벤트 </summary>
        public struct RequestSetIngredientEvent : IEvent
        {
            public Dictionary<string, int> newValue;
            public RequestSetIngredientEvent(Dictionary<string, int> newValue) => this.newValue = newValue;
        }

        /// <summary> 유저데이터 해금 레시피 데이터 변경 요청 이벤트 </summary>
        public struct RequestSetUnlockRecipeEvent : IEvent
        {
            public List<string> newValue;
            public RequestSetUnlockRecipeEvent(List<string> newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 튜도리얼 인덱스 변경 요청 이벤트 </summary>
        public struct RequestSetTutorialEvent : IEvent
        {
            public int newValue;
            public RequestSetTutorialEvent(int newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저데이터 상품 구매내역 데이터 변경 요청 이벤트 </summary>
        public struct RequestSetPurchaseInfoEvent : IEvent
        {
            public Dictionary<string, int> newValue;
            public RequestSetPurchaseInfoEvent(Dictionary<string, int> newValue) => this.newValue = newValue;
        }

        /// <summary> [상점] 상품 구매 요청 이벤트 </summary>
        public struct RequestPurchaseEvent : IEvent
        {
            public MerchandiseData data;

            public RequestPurchaseEvent(MerchandiseData data)
            {
                this.data = data;
            }
        }

        /// <summary> 유저데이터 트레이 데이터 변경 요청 이벤트 </summary>
        public struct RequestSetTrayEvent : IEvent
        {
            public TrayData newValue;
            public RequestSetTrayEvent(TrayData newValue) => this.newValue = newValue;
        }
        
        
        #endregion

        #region Request Set DeckData Event

        /// <summary> 유저 덱 데이터 도넛 변경 요청 이벤트 </summary>
        public struct RequestSetDeckDonutEvent : IEvent
        {
            public List<DonutInstanceData> newValue;
            public RequestSetDeckDonutEvent(List<DonutInstanceData> newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저 덱 데이터 제빵사 변경 요청 이벤트 </summary>
        public struct RequestSetDeckBakerEvent : IEvent
        {
            public BakerInstanceData newValue;
            public RequestSetDeckBakerEvent(BakerInstanceData newValue) => this.newValue = newValue;
        }

        #endregion

        #region Request Set MatchmakingData Event

        /// <summary> 유저 매치메이킹 데이터 MMR 변경 요청 이벤트 </summary>
        public struct RequestSetMMREvent : IEvent
        {
            public int newValue;
            public RequestSetMMREvent(int newValue) => this.newValue = newValue;
        }
        
        /// <summary> 유저 매치메이킹 데이터 온라인 상태 변경 요청 이벤트 </summary>
        public struct RequestSetOnlineStateEvent : IEvent
        {
            public bool newValue;
            public RequestSetOnlineStateEvent(bool newValue) => this.newValue = newValue;
        }

        #endregion

        #region Broadcast Set User Data Event

        /// <summary> 유저 닉네임 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserNicknameEvent : IEvent {}
        public struct BroadcastSetUserProfileImageEvent : IEvent {}
        
        /// <summary> 유저 에너지 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserEnergyEvent : IEvent {}
        
        /// <summary> 유저 골드 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserGoldEvent : IEvent {}
        
        /// <summary> 유저 캐쉬 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserCashEvent : IEvent {}
        
        /// <summary> 유저 레시피 조각 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserRecipePiecesEvent : IEvent {}
        
        /// <summary> 유저 완벽한 레시피 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserPerfectRecipeEvent : IEvent {}
        
        /// <summary> 유저 도넛 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserDonutEvent : IEvent {}
        
        /// <summary> 유저 제빵사 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserBakerEvent : IEvent {}
        
        /// <summary> 유저 재료 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserIngredientEvent : IEvent {}
        
        /// <summary> 유저 레시피 해금 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserUnlockRecipeEvent : IEvent {}
        
        /// <summary> 유저 튜토리얼 진행도 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserTutorialEvent : IEvent {}
        
        /// <summary> 유저 마지막 접속 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserLastTimeEvent : IEvent {}
        
        /// <summary> 유저 구매 이력 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserPurchaseInfoEvent : IEvent {}
        
        /// <summary> 유저 트레이 변경 알림 이벤트 </summary>
        public struct BroadcastSetUserTrayEvent : IEvent {}

        #endregion
    }
}
