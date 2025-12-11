using _Project.Scripts.EventStructs;
using _Project.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


using FE = _Project.Scripts.EventStructs.FirebaseEvents;

public partial class FirebaseManager
{
    private const string UserCollection = "users";
    private const string UserDonutCollection = "donuts";
    private const string UserBakerCollection = "bakers";

    private const string TableCollection = "table";

    private FirebaseFirestore Firestore { get; set; }

    private DocumentReference _userDocs;
    private DocumentReference _userDonutDocs;
    private DocumentReference _userBakerDocs;
    private DocumentReference _userPurchaseDocs;
    private DocumentReference _userTrayDocs;
    
    #region Event Rapper Methods
    
    private void OnRequestSaveUserData(FE.RequestSaveUserData evt) => SaveUserData(evt.userData).Forget();
    private void OnRequestLoadUserData(FE.RequestLoadUserData evt) => LoadUserData().Forget();

    private void OnRequestSaveDeckData(FE.RequestSaveDeckData evt) => SaveDeckData(evt.deckData).Forget();
    private void OnRequestLoadDeckData(FE.RequestLoadDeckData evt) => LoadDeckData().Forget();

    private void OnRequestSaveMatchmakingData(FE.RequestSaveMatchmakingData evt) => SaveMatchMakingData(evt.matchmakingEntry).Forget();
    private void OnRequestLoadMatchmakingData(FE.RequestLoadMatchmakingData evt) => LoadMatchMakingData().Forget();
    
    
    private void OnRequestLoadTableData(FE.RequestLoadTableData evt) => LoadTable().Forget();
    
    #endregion

    private void RegisterFirestoreEvent()
    {
        //========== Firestore Save UserData ==========//
        EventHub.Instance.RegisterEvent<FE.RequestSaveUserData>(OnRequestSaveUserData);
        EventHub.Instance.RegisterEvent<FE.RequestSaveDeckData>(OnRequestSaveDeckData);
        EventHub.Instance.RegisterEvent<FE.RequestSaveMatchmakingData>(OnRequestSaveMatchmakingData);
        
        //========== Firestore Load UserData ==========//
        EventHub.Instance.RegisterEvent<FE.RequestLoadUserData>(OnRequestLoadUserData);
        EventHub.Instance.RegisterEvent<FE.RequestLoadDeckData>(OnRequestLoadDeckData);
        EventHub.Instance.RegisterEvent<FE.RequestLoadMatchmakingData>(OnRequestLoadMatchmakingData);
        
        //========== Firestore Load Table Data ==========//
        EventHub.Instance.RegisterEvent<FE.RequestLoadTableData>(OnRequestLoadTableData);
    }
    private void UnRegisterFirestoreEvent()
    {
        //========== Firestore Save UserData ==========//
        EventHub.Instance?.UnRegisterEvent<FE.RequestSaveUserData>(OnRequestSaveUserData);
        EventHub.Instance?.UnRegisterEvent<FE.RequestSaveDeckData>(OnRequestSaveDeckData);
        EventHub.Instance?.UnRegisterEvent<FE.RequestSaveMatchmakingData>(OnRequestSaveMatchmakingData);
        
        //========== Firestore Load UserData ==========//
        EventHub.Instance?.UnRegisterEvent<FE.RequestLoadUserData>(OnRequestLoadUserData);
        EventHub.Instance?.UnRegisterEvent<FE.RequestLoadDeckData>(OnRequestLoadDeckData);
        EventHub.Instance?.UnRegisterEvent<FE.RequestLoadMatchmakingData>(OnRequestLoadMatchmakingData);
        
        //========== Firestore Load Table Data ==========//
        EventHub.Instance?.UnRegisterEvent<FE.RequestLoadTableData>(OnRequestLoadTableData);
    }
    
    
    private void CacheFirestoreField(string uid)
    {
        _userDocs = Firestore.Collection(UserCollection).Document(uid);
        _userDonutDocs = _userDocs.Collection(UserDonutCollection).Document(uid);
        _userBakerDocs = _userDocs.Collection(UserBakerCollection).Document(uid);
    }

    #region Save User Data Methods
    
    private async UniTask SaveUserData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserData Start");
        
        try
        {
            string uid = Auth.CurrentUser.UserId;
            if (uid != userData.uid) throw new Exception("로그인 정보와 데이터의 고유 아이디가 다릅니다.");

            List<UniTask> tasks = new()
            {
                SaveUserDefaultData(userData),
                SaveUserDonutsData(userData),
                SaveUserBakersData(userData),
                SaveUserIngredientData(userData),
                SaveUserTrayData(userData),
                SaveUserPurchaseData(userData),
            };
            
            await UniTask.WhenAll(tasks);
            
            Debug.Log($"<color=green>[Firestore] SaveUserData Success</color>");
            EventHub.Instance.RaiseEvent(new FE.SaveUserDataSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserData Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.SaveUserDataFailed(e.Message));
        }
    }
    private async UniTask SaveUserDefaultData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserDefaultData Start");

        try
        {
            var dto = UserDataDto.CurrentDto(userData);
            
            await _userDocs.SetAsync(dto).AsUniTask();
            
            Debug.Log($"[Firestore] SaveUserDefaultData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserDefaultData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask SaveUserDonutsData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserDonutsData Start");

        try
        {
            var dto = UserDonutDataDto.CurrentDto(userData);
            
            await _userDonutDocs.SetAsync(dto).AsUniTask();
            
            Debug.Log($"[Firestore] SaveUserDonutsData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserDonutsData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask SaveUserBakersData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserBakersData Start");

        try
        {
            var dto = UserBakerDataDto.CurrentDto(userData);
            
            await _userBakerDocs.SetAsync(dto).AsUniTask();
            
            Debug.Log($"[Firestore] SaveUserBakersData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserBakersData Failed : {e.Message}</color>>");
            throw;
        }
    }
    private async UniTask SaveUserIngredientData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserIngredientData Start");

        try
        {
            var dto = UserIngredientDataDto.CurrentDto(userData);
            var docs = _userDocs.Collection("ingredients").Document(userData.uid);
            
            await docs.SetAsync(dto).AsUniTask();
            
            Debug.Log($"[Firestore] SaveUserIngredientData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserIngredientData Failed : {e.Message}</color>>");
            throw;
        }
    }
    private async UniTask SaveUserTrayData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserTrayData Start");

        try
        {
            var dto = TrayDataDto.CurrentDto(userData.trayData);
            var docs = _userDocs.Collection("tray").Document(userData.uid);
            
            await docs.SetAsync(dto).AsUniTask();
            
            Debug.Log($"[Firestore] SaveUserTrayData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserTrayData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask SaveUserPurchaseData(UserData userData)
    {
        Debug.Log($"[Firestore] SaveUserPurchaseData Start");

        try
        {
            var dto = UserPurchaseDataDto.CurrentDto(userData);
            var docs = _userDocs.Collection("purchase").Document(userData.uid);
            
            await docs.SetAsync(dto).AsUniTask();
            
            Debug.Log("[Firestore] SaveUserPurchaseData Success");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] SaveUserPurchaseData Failed : {e.Message}</color>");
            throw;
        }
    }
    
    #endregion

    #region Save Addtional User Data Methods

    private async UniTask SaveDeckData(DeckData deckData)
    {
        Debug.Log("[Firestore] Save DeckData Start");

        try
        {
            string uid = Auth.CurrentUser.UserId;

            if (deckData.uid != uid) throw new Exception("로그인 정보와 데이터의 UID가 다릅니다.");

            var dto = DeckDataDto.CurrentDto(deckData);
            var docs = _userDocs.Collection("deck").Document(uid);
            
            await docs.SetAsync(dto).AsUniTask();
            
            Debug.Log("[Firestore] SaveDeckData Success");
            EventHub.Instance.RaiseEvent(new FE.SaveDeckDataSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Save Deck Data Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.SaveDeckDataFailed(e.Message));
        }
    }
    private async UniTask SaveMatchMakingData(MatchMakingEntry matchMakingEntry)
    {
        Debug.Log("[Firestore] Save MatchMakingData Start");

        try
        {
            string uid = Auth.CurrentUser.UserId;

            if (matchMakingEntry.uid != uid) throw new Exception("로그인 정보와 데이터의 UID가 다릅니다.");
            
            var dto = MatchMakingEntryDto.CurrentDto(matchMakingEntry);
            var docs = _userDocs.Collection("matchmaking").Document(uid);
            
            await docs.SetAsync(dto).AsUniTask();
            
            Debug.Log("[Firestore] SaveMatchMakingData Success");
            EventHub.Instance.RaiseEvent(new FE.SaveMatchmakingDataSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Save MatchMakingData Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.SaveMatchmakingDataFailed());
        }
    }

    #endregion

    #region Load User Data Methods

    private async UniTask LoadUserData()
    {
        Debug.Log("[Firestore] LoadUserData Start");

        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");
            
            UserData userData = new();

            List<UniTask> tasks = new()
            {
                LoadUserDefaultData(userData),
                LoadUserDonutsData(userData),
                LoadUserBakersData(userData),
                LoadUserIngredientData(userData),
                LoadUserTrayData(userData),
                LoadUserPurchaseData(userData),
            };
            
            await UniTask.WhenAll(tasks);
            
            Debug.Log("<color=green>[Firestore] LoadUserData Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadUserDataSuccess(userData));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserData Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadUserDataFailed(e.Message));
        }
    }
    private async UniTask LoadUserDefaultData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserDefaultData Start");

        try
        {
            var snapshot = await _userDocs.GetSnapshotAsync(Source.Server).AsUniTask();

            if (snapshot.Exists == false) throw new Exception("User Default Data is not exist");
            
            UserDataDto dto = snapshot.ConvertTo<UserDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserDefaultData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserDefaultData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadUserDonutsData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserDonutsData Start");

        try
        {
            var snapshot = await _userDonutDocs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("User Donut Data is not exist");
            
            UserDonutDataDto dto = snapshot.ConvertTo<UserDonutDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserDonutsData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserDonutsData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadUserBakersData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserBakersData Start");

        try
        {
            var snapshot = await _userBakerDocs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("User Baker Data is not exist");
            
            UserBakerDataDto dto = snapshot.ConvertTo<UserBakerDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserBakersData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserBakersData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadUserIngredientData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserIngredientData Start");

        try
        {
            var docs = _userDocs.Collection("ingredients").Document(Auth.CurrentUser.UserId);

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("User Ingredient Data is not exist");

            var dto = snapshot.ConvertTo<UserIngredientDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserIngredientData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserIngredientData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadUserTrayData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserTrayData Start");

        try
        {
            var docs = _userDocs.Collection("tray").Document(Auth.CurrentUser.UserId);
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();

            if (snapshot.Exists == false) throw new Exception("User Tray Data is not exist");
            
            var dto = snapshot.ConvertTo<TrayDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserTrayData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError("<color=red>[Firestore] LoadUserTrayData Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadUserPurchaseData(UserData userData)
    {
        Debug.Log("[Firestore] Load UserPurchaseData Start");

        try
        {
            var docs = _userDocs.Collection("purchase").Document(Auth.CurrentUser.UserId);
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();

            if (snapshot.Exists == false) throw new Exception("User Purchase Data is not exist");

            var dto = snapshot.ConvertTo<UserPurchaseDataDto>();
            userData.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] LoadUserPurchaseData Success</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] LoadUserPurchaseData Failed : {e.Message}</color>");
            throw;
        }
    }

    #endregion

    #region Load Addtional User Data Methods

    private async UniTask LoadDeckData()
    {
        Debug.Log("[Firestore] Load DeckData Start");

        try
        {
            var docs = _userDocs.Collection("deck").Document(Auth.CurrentUser.UserId);
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Deck Data is not exist");
            
            
            DeckDataDto dto = snapshot.ConvertTo<DeckDataDto>();
            DeckData deck = new();
            deck.SetData(dto);

            Debug.Log("<color=green>[Firestore] Load DeckData Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDeckDataSuccess(deck));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load DeckData Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDeckDataFailed(e.Message));
        }
    }
    
    private async UniTask LoadMatchMakingData()
    {
        Debug.Log("[Firestore] Load MatchMakingData Start");

        try
        {
            var docs = _userDocs.Collection("matchmaking").Document(Auth.CurrentUser.UserId);

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("MatchMaking Data is not exist");

            MatchMakingEntryDto dto = snapshot.ConvertTo<MatchMakingEntryDto>();
            MatchMakingEntry matchMakingEntry = new();
            matchMakingEntry.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load MatchMakingData Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadMatchmakingDataSuccess(matchMakingEntry));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load MatchMakingData Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadMatchmakingDataFailed(e.Message));
        }
    }

    #endregion
    
    #region Load Table Data Methods

    private async UniTask LoadTable()
    {
        Debug.Log("[Firestore] Load Table Start");

        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");

            List<UniTask> tasks = new()
            {
                LoadDonutTable(),
                LoadBakerTable(),
                LoadIngredientTable(),
                LoadSkillTable(),
                LoadRecipeTable(),
                LoadRecipeNodeTable(),
                LoadMerchandiseTable(),
                LoadProductTable(),
                LoadDonutMergeTable(),
                LoadDonutModifierTable(),
                LoadBakerLevelUpTable(),
                LoadAchievementTable(),
            };
            
            await UniTask.WhenAll(tasks);
            
            Debug.Log("<color=green>[Firestore] Load Table Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTableSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Table Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTableFailed(e.Message));
        }
    }
    private async UniTask LoadDonutTable()
    {
        Debug.Log("[Firestore] Load Donut Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("donut_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Donut Table Data is not exist");
            
            var dto = snapshot.ConvertTo<Dictionary<string, DonutDataDto>>();
            var table = new Dictionary<string, DonutData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Donut Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDonutTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Donut Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadBakerTable()
    {
        Debug.Log("[Firestore] Load Baker Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("baker_table");
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();

            if (snapshot.Exists == false) throw new Exception("Baker Table Data is not exist");
            
            var dto = snapshot.ConvertTo<Dictionary<string, BakerDataDto>>();
            var table = new Dictionary<string, BakerData>();
            table.SetData(dto);
            
            
            Debug.Log("<color=green>[Firestore] Load Baker Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadBakerTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Baker Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadIngredientTable()
    {
        Debug.Log("[Firestore] Load Ingredient Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("ingredient_table");
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Ingredient Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, IngredientDataDto>>();
            var table = new Dictionary<string, IngredientData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Ingredient Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadIngredientTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Ingredient Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadSkillTable()
    {
        Debug.Log("[Firestore] Load Skill Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("skill_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();

            if (snapshot.Exists == false) throw new Exception("Skill Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, SkillDataDto>>();
            var table = new Dictionary<string, SkillData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Skill Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadSkillTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Skill Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadRecipeTable()
    {
        Debug.Log("[Firestore] Load Recipe Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("recipe_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Recipe Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, RecipeDataDto>>();
            var table = new Dictionary<string, RecipeData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Recipe Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadRecipeTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Recipe Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadRecipeNodeTable()
    {
        Debug.Log("[Firestore] Load Recipe Node Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("recipeNode_table");
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Recipe Node Table Data is not exist");
            
            var dto = snapshot.ConvertTo<Dictionary<string, RecipeNodeDataDto>>();
            var table = new Dictionary<string, RecipeNodeData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Recipe Node Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadRecipeNodeTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Recipe Node Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadMerchandiseTable()
    {
        Debug.Log("[Firestore] Load Merchandise Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("merchandise_table");
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Merchandise Table Data is not exist");
            
            var dto = snapshot.ConvertTo<Dictionary<string, MerchandiseDataDto>>();
            var table = new Dictionary<string, MerchandiseData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Merchandise Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadMerchandiseTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Merchandise Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadProductTable()
    {
        Debug.Log("[Firestore] Load Product Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("product_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Product Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, ProductDataDto>>();
            var table = new Dictionary<string, ProductData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Product Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadProductTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Product Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadDonutMergeTable()
    {
        Debug.Log("[Firestore] Load Donut Merge Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("donutMerge_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Donut Merge Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, DonutMergeDataDto>>();
            var table = new Dictionary<string, DonutMergeData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Donut Merge Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDonutMergeTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Donut Merge Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadDonutModifierTable()
    {
        Debug.Log("[Firestore] Load Donut Modifier Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("donutModifier_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Donut Modifier Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, DonutModifierDataDto>>();
            var table = new Dictionary<string, DonutModifierData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Donut Modifier Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDonutModifierTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Donut Modifier Table Data Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask LoadBakerLevelUpTable()
    {
        Debug.Log("[Firestore] Load Baker Level Up Table Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("bakerLevelUp_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Baker Level Up Table Data is not exist");

            var dto = snapshot.ConvertTo<Dictionary<string, BakerLevelUpDataDto>>();
            var table = new Dictionary<string, BakerLevelUpData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load BakerLevelUp Table Data Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadBakerLevelUpTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Baker Level Up Table Data Failed : {e.Message}</color>");
            throw;
        }
    }

    private async UniTask LoadAchievementTable()
    {
        Debug.Log("[Firestore] Load Achievement Table Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("achievement_table");

            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            
            if (snapshot.Exists == false) throw new Exception("Achievement Table Data is not exist");
            
            var dto = snapshot.ConvertTo<Dictionary<string, AchievementDataDto>>();
            var table = new Dictionary<string, AchievementData>();
            table.SetData(dto);
            
            Debug.Log("<color=green>[Firestore] Load Achievement Table Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadAchievementTableSuccess(table));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Achievement Table Failed : {e.Message}</color>");
            throw;
        }
    }
    
    #endregion

    public async UniTask<DefaultUserData> LoadDefaultData()
    {
        Debug.Log("[Firestore] Load Default User Data Start");

        try
        {
            var docs = Firestore.Collection(TableCollection).Document("defaultUser_table");
            
            var snapshot = await docs.GetSnapshotAsync(Source.Server).AsUniTask();
            if (snapshot.Exists == false) throw new Exception("Default User Data is not exist");
            
            var dto = snapshot.ConvertTo<DefaultUserDataDto>();
            var data = new DefaultUserData();
            data.SetData(dto, Auth.CurrentUser.UserId);
            
            Debug.Log("<color=green>[Firestore] Load Default User Data Success</color>");
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Firestore] Load Default User Data Failed : {e.Message}</color>");
            throw;
        }
    }
}