using System;
using System.Collections.Generic;

namespace _Project.Scripts.EventStructs
{
    public static class FirebaseEvents
    {
        #region Firebase Init Events

        /// <summary> Firebase 초기화 성공 이벤트 </summary>
        public struct FirebaseInitSuccess : IEvent
        {
            public bool isLogin;
            
            public FirebaseInitSuccess(bool isLogin) => this.isLogin = isLogin;
        }
        
        /// <summary> Firebase 초기화 실패 이벤트 </summary>
        public struct FirebaseInitFailed : IEvent
        {
            public string error;
            public FirebaseInitFailed(string error) => this.error = error;
        }
        
        /// <summary> Firebase Field 캐싱 완료 이벤트 </summary>
        public struct CompleteFirebaseCache : IEvent { }

        #endregion

        #region Request Auth Events
        
        /// <summary> 익명 로그인 요청 이벤트 </summary>
        public struct RequestCreateGuestEvent : IEvent { }
        
        /// <summary>이메일 회원가입 요청 이벤트</summary>
        public struct RequestCreateEmailEvent : IEvent
        {
            public string email;
            public string password;
            public string confirmPassword;

            public RequestCreateEmailEvent(string email, string password, string confirmPassword)
            {
                this.email = email;
                this.password = password;
                this.confirmPassword = confirmPassword;
            }
        }
        
        
        /// <summary> 이메일 로그인 요청 이벤트 </summary>
        public struct RequestSignInWithEmailEvent : IEvent
        {
            public string email;
            public string password;

            public RequestSignInWithEmailEvent(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }

        /// <summary> Google Play Game 로그인 요청 이벤트 </summary>
        public struct RequestSignInWithGpgsEvent : IEvent { }
        
        /// <summary> 로그아웃 요청 이벤트 </summary>
        public struct RequestLogoutEvent : IEvent { }
        
        
        public struct RequestSwitchCredentialEvent : IEvent
        {
            public string providerId;
            
            public RequestSwitchCredentialEvent(string providerId) => this.providerId = providerId;
        }

        #endregion
        
        #region Auth Callback Events

        /// <summary> 로그인 성공 이벤트 </summary>
        public struct FirebaseLoginSuccess : IEvent
        {
            public string uid;
            
            public FirebaseLoginSuccess(string uid) => this.uid = uid;
        }

        /// <summary> 로그인 실패 이벤트 </summary>
        public struct FirebaseLoginFailed : IEvent
        {
            public string error;
            public FirebaseLoginFailed(string error) => this.error = error;
        }

        /// <summary> 회원가입 성공 이벤트 </summary>
        public struct FirebaseRegisterSuccess : IEvent
        {
            public string uid;
            
            public FirebaseRegisterSuccess(string uid) => this.uid = uid;
        }
        
        /// <summary> 회원가입 실패 이벤트 </summary>
        public struct FirebaseRegisterFailed : IEvent
        {
            public string error;
            public FirebaseRegisterFailed(string error) => this.error = error;
        }

        /// <summary> 로그아웃 이벤트 </summary>
        public struct FirebaseLogoutEvent : IEvent { }
        
        
        public struct FirebaseSwitchCredentialSuccess : IEvent { }
        public struct FirebaseSwitchCredentialFailed : IEvent
        {
            public string error;
            public FirebaseSwitchCredentialFailed(string error) => this.error = error;
        }
        
        #endregion

        
        #region Request Realtime Database Events

        public struct RequestDataUpload : IEvent
        {
            public object data;
            
            public RequestDataUpload(object data) => this.data = data;
        }

        public struct RequestLeaderboardUpdate : IEvent
        {
            public string nickName;
            public long deltaScore;
            
            public RequestLeaderboardUpdate(long deltaScore, string nickName)
            {
                this.nickName = nickName;
                this.deltaScore = deltaScore;
            }
        }

        public struct RequestTotalTopRanking : IEvent
        {
            public int topCount;
            public RequestTotalTopRanking(int topCount) => this.topCount = topCount;
        }

        public struct RequestDailyRanking : IEvent
        {
            public string nickName;
            public int count;
            public RequestDailyRanking(string nickName, int count)
            {
                this.nickName = nickName;
                this.count = count;
            }
        }

        public struct RequestTotalRanking : IEvent
        {
            public string nickName;
            public int count;
            public RequestTotalRanking(string nickName, int count)
            {
                this.nickName = nickName;
                this.count = count;
            }
        }
        
        #endregion

        #region Realtime Database Callback Events

        public struct LeaderboardUpdateSuccess : IEvent { }
        public struct LeaderboardUpdateFailed : IEvent
        {
            public string error;
            public LeaderboardUpdateFailed(string error) => this.error = error;
        }
        
        
        public struct LoadTotalTopRankingSuccess : IEvent
        {
            public List<LeaderboardEntry> ranking;
            
            public LoadTotalTopRankingSuccess(List<LeaderboardEntry> ranking) => this.ranking = ranking;
        }
        public struct LoadTotalTopRankingFailed : IEvent
        {
            public string error;
            public LoadTotalTopRankingFailed(string error) => this.error = error;
        }

        public struct LoadDailyRankingSuccess : IEvent
        {
            public List<LeaderboardEntry> ranking;
            public LoadDailyRankingSuccess(List<LeaderboardEntry> ranking) => this.ranking = ranking;
        }

        public struct LoadDailyRankingFailed : IEvent
        {
            public string error;
            public LoadDailyRankingFailed(string error) => this.error = error;
        }

        public struct LoadTotalRankingSuccess : IEvent
        {
            public List<LeaderboardEntry> ranking;
            public LoadTotalRankingSuccess(List<LeaderboardEntry> ranking) => this.ranking = ranking;
        }

        public struct LoadTotalRankingFailed : IEvent
        {
            public string error;
            public LoadTotalRankingFailed(string error) => this.error = error;
        }

        #endregion


        #region Request Firestore Events

        public struct RequestSaveUserData : IEvent
        {
            public UserData userData;
            public RequestSaveUserData(UserData userData) => this.userData = userData;
        }
        public struct RequestSaveDeckData : IEvent
        {
            public DeckData deckData;
            public RequestSaveDeckData(DeckData deckData) => this.deckData = deckData;
        }
        public struct RequestSaveMatchmakingData : IEvent
        {
            public MatchMakingEntry matchmakingEntry;
            public RequestSaveMatchmakingData(MatchMakingEntry matchmakingEntry) => this.matchmakingEntry = matchmakingEntry;
        }
        
        public struct RequestLoadUserData : IEvent { }
        public struct RequestLoadDeckData : IEvent { }
        public struct RequestLoadMatchmakingData : IEvent { }
        
        public struct RequestLoadTableData : IEvent {}

        #endregion
        
        #region Firestore Callback Events
        
        public struct SaveUserDataSuccess : IEvent { }
        public struct SaveUserDataFailed : IEvent
        {
            public string error;
            public SaveUserDataFailed(string error) => this.error = error;
        }
        
        public struct SaveDeckDataSuccess : IEvent { }
        public struct SaveDeckDataFailed : IEvent
        {
            public string error;
            public SaveDeckDataFailed(string error) => this.error = error;
        }
        
        public struct SaveMatchmakingDataSuccess : IEvent { }
        public struct SaveMatchmakingDataFailed : IEvent
        {
            public string error;
            public SaveMatchmakingDataFailed(string error) => this.error = error;
        }

        public struct LoadUserDataSuccess : IEvent
        {
            public UserData userData;
            public LoadUserDataSuccess(UserData userData) => this.userData = userData;
        }
        public struct LoadUserDataFailed : IEvent
        {
            public string error;
            public LoadUserDataFailed(string error) => this.error = error;
        }
        
        public struct LoadDeckDataSuccess : IEvent
        {
            public DeckData deckData;
            public LoadDeckDataSuccess(DeckData deckData) => this.deckData = deckData;
        }
        public struct LoadDeckDataFailed : IEvent
        {
            public string error;
            public LoadDeckDataFailed(string error) => this.error = error;
        }

        public struct LoadMatchmakingDataSuccess : IEvent
        {
            public MatchMakingEntry matchmakingEntry;
            public LoadMatchmakingDataSuccess(MatchMakingEntry matchmakingEntry) => this.matchmakingEntry = matchmakingEntry;
        }
        public struct LoadMatchmakingDataFailed : IEvent
        {
            public string error;
            public LoadMatchmakingDataFailed(string error) => this.error = error;
        }


        public struct LoadDonutTableSuccess : IEvent
        {
            public Dictionary<string, DonutData> table;
            public LoadDonutTableSuccess(Dictionary<string, DonutData> table) => this.table = table;
        }
        public struct LoadBakerTableSuccess : IEvent
        {
            public Dictionary<string, BakerData> table;
            public LoadBakerTableSuccess(Dictionary<string, BakerData> table) => this.table = table;
        }
        public struct LoadIngredientTableSuccess : IEvent
        {
            public Dictionary<string, IngredientData> table;
            public LoadIngredientTableSuccess(Dictionary<string, IngredientData> table) => this.table = table;
        }
        public struct LoadSkillTableSuccess : IEvent
        {
            public Dictionary<string, SkillData> table;
            public LoadSkillTableSuccess(Dictionary<string, SkillData> table) => this.table = table;
        }
        public struct LoadRecipeTableSuccess : IEvent
        {
            public Dictionary<string, RecipeData> table;
            public LoadRecipeTableSuccess(Dictionary<string, RecipeData> table) => this.table = table;
        }
        public struct LoadRecipeNodeTableSuccess : IEvent
        {
            public Dictionary<string, RecipeNodeData> table;
            public LoadRecipeNodeTableSuccess(Dictionary<string, RecipeNodeData> table) => this.table = table;
        }
        public struct LoadMerchandiseTableSuccess : IEvent
        {
            public Dictionary<string, MerchandiseData> table;
            public LoadMerchandiseTableSuccess(Dictionary<string, MerchandiseData> table) => this.table = table;
        }
        public struct LoadProductTableSuccess : IEvent
        {
            public Dictionary<string, ProductData> table;
            public LoadProductTableSuccess(Dictionary<string, ProductData> table) => this.table = table;
        }
        public struct LoadDonutMergeTableSuccess : IEvent
        {
            public Dictionary<string, DonutMergeData> table;
            public LoadDonutMergeTableSuccess(Dictionary<string, DonutMergeData> table) => this.table = table;
        }
        public struct LoadDonutModifierTableSuccess : IEvent
        {
            public Dictionary<string, DonutModifierData> table;
            public LoadDonutModifierTableSuccess(Dictionary<string, DonutModifierData> table) => this.table = table;
        }
        public struct LoadBakerLevelUpTableSuccess : IEvent
        {
            public Dictionary<string, BakerLevelUpData> table;
            public LoadBakerLevelUpTableSuccess(Dictionary<string, BakerLevelUpData> table) => this.table = table;
        }
        
        public struct LoadAchievementTableSuccess : IEvent
        {
            public Dictionary<string, AchievementData> table;
            public LoadAchievementTableSuccess(Dictionary<string, AchievementData> table) => this.table = table;
        }
        
        public struct LoadTableSuccess : IEvent { }
        public struct LoadTableFailed : IEvent
        {
            public string error;
            public LoadTableFailed(string error) => this.error = error;
        }
        
        #endregion
    }
}
