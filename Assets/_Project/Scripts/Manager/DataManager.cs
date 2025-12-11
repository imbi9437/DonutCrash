using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using DS = _Project.Scripts.EventStructs.DataStructs;

public partial class DataManager : MonoSingleton<DataManager>
{
    [Header("유저 관련 데이터 캐싱")]
    private UserData localUserData;         //플레이어의 유저 데이터
    private DeckData localDeckData;         //플레이어의 덱 데이터
    private MatchMakingEntry localMatchData;//플레이어의 매치메이킹 데이터

    
    [Header("정적 아이템 객체 데이터 캐싱")] 
    private Dictionary<string, DonutData> donutTable;               //도넛 데이터 테이블
    private Dictionary<string, BakerData> bakerTable;               //제빵사 데이터 테이블
    private Dictionary<string, IngredientData> ingredientTable;     //재료 데이터 테이블

    
    [Header("시스템 객체 데이터 캐싱")]
    private Dictionary<string, SkillData> skillTable;               //스킬 데이터 테이블
    private Dictionary<string, RecipeData> recipeTable;             //도넛별 레시피 테이블
    private Dictionary<string, RecipeNodeData> recipeNodeTable;     //레시피별 노드 테이블
    private Dictionary<string, MerchandiseData> merchandiseTable;   //상품 데이터 테이블
    private Dictionary<string, ProductData> productTable;           //제품 데이터 테이블
    private Dictionary<string, AchievementData> achievementTable;
    
    [Header("육성 관련 데이터 캐싱")]
    private Dictionary<int, DonutMergeData> donutMergeTable;           //레벨별 도넛 머지 데이터 테이블
    private Dictionary<string, DonutModifierData> donutModifierTable;  //도넛별 스펙업 테이블
    private Dictionary<int, BakerLevelUpData> bakerLevelUpTable;       //레벨별 제빵사 경험치 테이블


    [Header("기타 필드")] 
    private UniTaskCompletionSource<bool> _userInitTcs;
    private UniTaskCompletionSource<bool> _tableInitTcs;
    
    #region Unity Message Methods

    public void Start()
    {
        // 정적 아이템 객체 데이터
        EventHub.Instance?.RegisterEvent<FE.LoadDonutTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadBakerTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadIngredientTableSuccess>(OnLoadTableData);
        
        // 시스템 객체 데이터
        EventHub.Instance?.RegisterEvent<FE.LoadSkillTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadRecipeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadRecipeNodeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadMerchandiseTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadProductTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadAchievementTableSuccess>(OnLoadTableData);
        
        // 육성 관련 데이터
        EventHub.Instance?.RegisterEvent<FE.LoadDonutMergeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadDonutModifierTableSuccess>(OnLoadTableData);
        EventHub.Instance?.RegisterEvent<FE.LoadBakerLevelUpTableSuccess>(OnLoadTableData);
        
        // 유저 관련 데이터
        EventHub.Instance?.RegisterEvent<FE.LoadUserDataSuccess>(OnSuccessLoadUserData);
        EventHub.Instance?.RegisterEvent<FE.LoadDeckDataSuccess>(OnSuccessLoadDeckData);
        EventHub.Instance?.RegisterEvent<FE.LoadMatchmakingDataSuccess>(OnSuccessLoadMatchData);
        
        // 일반 콜백
        EventHub.Instance?.RegisterEvent<DS.RequestCreateNewDataEvent>(OnCreateNewAccountData);
        
        //DataManagerSetter Partial 이벤트 등록
        RegisterSetterEvent();

        WaitUntilUserDataInitialized().Forget();
        WaitUntilTableDataInitialized().Forget();
        WaitUntilLoadDataComplete().Forget();
    }
    private void OnDestroy()
    {
        // 정적 아이템 객체 데이터
        EventHub.Instance?.UnRegisterEvent<FE.LoadDonutTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadBakerTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadIngredientTableSuccess>(OnLoadTableData);
        
        // 시스템 객체 데이터
        EventHub.Instance?.UnRegisterEvent<FE.LoadSkillTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadRecipeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadRecipeNodeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadMerchandiseTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadProductTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadAchievementTableSuccess>(OnLoadTableData);
        
        // 육성 관련 데이터
        EventHub.Instance?.UnRegisterEvent<FE.LoadDonutMergeTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadDonutModifierTableSuccess>(OnLoadTableData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadBakerLevelUpTableSuccess>(OnLoadTableData);
        
        // 유저 관련 데이터
        EventHub.Instance?.UnRegisterEvent<FE.LoadUserDataSuccess>(OnSuccessLoadUserData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadDeckDataSuccess>(OnSuccessLoadDeckData);
        EventHub.Instance?.UnRegisterEvent<FE.LoadMatchmakingDataSuccess>(OnSuccessLoadMatchData);
        
        // 일반 콜백
        EventHub.Instance?.UnRegisterEvent<DS.RequestCreateNewDataEvent>(OnCreateNewAccountData);
        
        //DataManagerSetter Partial 이벤트 해지
        UnRegisterSetterEvent();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            if (localUserData == null) return;
            if (string.IsNullOrWhiteSpace(localUserData.uid)) return;
            
            EventHub.Instance.RaiseEvent(new FE.RequestSaveUserData(localUserData));
        }
    }

    #endregion

    #region Event Rapper Methods
    
    // 정적 아이템 객체 데이터
    private void OnLoadTableData(FE.LoadDonutTableSuccess evt) => SetDonutTableData(evt.table);
    private void OnLoadTableData(FE.LoadBakerTableSuccess evt) => SetBakerTableData(evt.table);
    private void OnLoadTableData(FE.LoadIngredientTableSuccess evt) => SetIngredientTableData(evt.table);
    
    // 시스템 객체 데이터
    private void OnLoadTableData(FE.LoadSkillTableSuccess evt) => SetSkillTableData(evt.table);
    private void OnLoadTableData(FE.LoadRecipeTableSuccess evt) => SetRecipeTableData(evt.table);
    private void OnLoadTableData(FE.LoadRecipeNodeTableSuccess evt) => SetRecipeNodeTableData(evt.table);
    private void OnLoadTableData(FE.LoadMerchandiseTableSuccess evt) => SetMerchandiseTableData(evt.table);
    private void OnLoadTableData(FE.LoadProductTableSuccess evt) => SetProductTableData(evt.table);
    private void OnLoadTableData(FE.LoadAchievementTableSuccess evt) => SetAchievementTableData(evt.table);
    
    // 육성 관련 데이터
    private void OnLoadTableData(FE.LoadDonutMergeTableSuccess evt) => SetMergeTableData(evt.table);
    private void OnLoadTableData(FE.LoadDonutModifierTableSuccess evt) => SetModifierData(evt.table);
    private void OnLoadTableData(FE.LoadBakerLevelUpTableSuccess evt) => SetBakerLevelUpData(evt.table);
    
    // 유저 관련 데이터
    private void OnSuccessLoadUserData(FE.LoadUserDataSuccess evt) => SetUserData(evt.userData);
    private void OnSuccessLoadDeckData(FE.LoadDeckDataSuccess evt) => SetDeckData(evt.deckData);
    private void OnSuccessLoadMatchData(FE.LoadMatchmakingDataSuccess evt) => SetMatchMakingData(evt.matchmakingEntry);

    // 일반 콜백
    private void OnCreateNewAccountData(DS.RequestCreateNewDataEvent evt) => CreateNewAccount().Forget();
    
    #endregion

    #region 유저데이터 초기화
    
    private void SetUserData(UserData data) => localUserData = data;
    private void SetDeckData(DeckData data) => localDeckData = data;
    private void SetMatchMakingData(MatchMakingEntry data) => localMatchData = data;
    
    #endregion

    #region 아이템 데이터 초기화

    private void SetDonutTableData(Dictionary<string, DonutData> table) => donutTable = table;
    private void SetBakerTableData(Dictionary<string, BakerData> table) => bakerTable = table;
    private void SetIngredientTableData(Dictionary<string, IngredientData> table) => ingredientTable = table;
    
    #endregion

    #region 시스템 객체 데이터 초기화
    
    private void SetSkillTableData(Dictionary<string, SkillData> table)
    {
        // TODO : SkillData 자식 객체로 자동 형변환 필요
        skillTable = new();

        table.ForEach(x => skillTable.TryAdd(x.Value.uid, x.Value));
    }

    /// <summary> Firebase DB로 부터 도넛 레시피 테이블 캐싱하는 함수 </summary>
    private void SetRecipeTableData(Dictionary<string, RecipeData> table) => recipeTable = table;

    /// <summary> Firebase DB로 부터 레시피 해금용 데이터 테이블 캐싱하는 함수 </summary>
    private void SetRecipeNodeTableData(Dictionary<string, RecipeNodeData> table) => recipeNodeTable = table;

    /// <summary> Firebase DB로 부터 상품 데이터 테이블 캐싱하는 함수 </summary>
    private void SetMerchandiseTableData(Dictionary<string, MerchandiseData> table) => merchandiseTable = table;
    
    /// <summary> Firebase DB로 부터 제품(상품에 들어가는) 데이터 테이블 캐싱하는 함수 </summary>
    private void SetProductTableData(Dictionary<string, ProductData> table) => productTable = table;
    
    private void SetAchievementTableData(Dictionary<string, AchievementData> table) => achievementTable = table;
    
    #endregion

    #region 육성 관련 데이터 초기화
    
    /// <summary> Firebase DB로 부터 도넛 머지테이블 캐싱하는 함수 </summary>
    private void SetMergeTableData(Dictionary<string, DonutMergeData> table)
    {
        donutMergeTable = new();

        foreach (var kvp in table)
        {
            var data = kvp.Value;
            
            donutMergeTable.TryAdd(data.targetLevel, data);
        }
    }

    /// <summary> Firebase DB로 부터 도넛 스탯 변화량을 캐싱하는 함수 </summary>
    private void SetModifierData(Dictionary<string, DonutModifierData> table)
    {
        donutModifierTable = new();

        foreach (var kvp in table)
        {
            var data = kvp.Value;

            donutModifierTable.TryAdd(data.origin, data);
        }
    }

    /// <summary> Firebase DB로 부터 제빵사 레벨업 데이터 캐싱하는 함수 </summary>
    private void SetBakerLevelUpData(Dictionary<string, BakerLevelUpData> table)
    {
        bakerLevelUpTable = new();

        foreach (var kvp in table)
        {
            var data = kvp.Value;

            bakerLevelUpTable.TryAdd(data.level, data);
        }
    }
    
    
    #endregion

    #region Wait Initialized Method

    private async UniTask WaitUntilUserDataInitialized()
    {
        try
        {
            var token = destroyCancellationToken;
            _userInitTcs = new UniTaskCompletionSource<bool>();

            List<UniTask> list = new()
            {
                UniTask.WaitUntil(() => localUserData != null, cancellationToken: token),
                UniTask.WaitUntil(() => localDeckData != null, cancellationToken: token),
                UniTask.WaitUntil(() => localMatchData != null, cancellationToken: token)
            };
            
            await UniTask.WhenAll(list);
            
            _userInitTcs.TrySetResult(true);
            EventHub.Instance.RaiseEvent(new DS.CompleteLoadUserDataEvent());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[DataManager] WaitUntilUserDataInitialized Failed : {e.Message}</color>");
        }
    }
    private async UniTask WaitUntilTableDataInitialized()
    {
        try
        {
            var token = destroyCancellationToken;
            _tableInitTcs = new UniTaskCompletionSource<bool>();

            List<UniTask> list = new()
            {
                UniTask.WaitUntil(() => donutTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => bakerTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => ingredientTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => skillTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => recipeTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => recipeNodeTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => merchandiseTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => productTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => donutMergeTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => donutModifierTable != null, cancellationToken: token),
                UniTask.WaitUntil(() => bakerLevelUpTable != null, cancellationToken: token)
            };
            
            await UniTask.WhenAll(list);
            
            _tableInitTcs.TrySetResult(true);
            EventHub.Instance.RaiseEvent(new DS.CompleteLoadTableDataEvent());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[DataManager] WaitUntilTableDataInitialized Failed : {e.Message}</color>");
        }
    }
    private async UniTask WaitUntilLoadDataComplete()
    {
        try
        {
            await UniTask.WhenAll(_userInitTcs.Task, _tableInitTcs.Task);
            
            EventHub.Instance.RaiseEvent(new DS.CompleteLoadDataEvent());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[DataManager] WaitUntilLoadDataComplete Failed : {e.Message}</color>");
        }
    }

    #endregion
    
    #region 초기 데이터 업로드
    
    private async UniTask CreateNewAccount()
    {
        var defaultData = await FirebaseManager.Instance.LoadDefaultData();
        
        CreateNewUserData(defaultData);
        CreateNewDeckData(defaultData);
        CreateNewMatchMakingData(defaultData);
        
        EventHub.Instance.RaiseEvent(new DS.CompleteCreateNewDataEvent());
    }
    
    private void CreateNewUserData(DefaultUserData defaultUserData)
    {
        if (defaultUserData == null)
        {
            Debug.LogError("기본 설정데이터가 존재하지 않습니다.");
            return;
        }
        
        localUserData = new UserData(defaultUserData.uid);
        
        localUserData.energy = defaultUserData.defaultEnergy;
        localUserData.gold = defaultUserData.defaultGold;
        localUserData.cash = defaultUserData.defaultCash;
        
        localUserData.recipePieces = defaultUserData.defaultRecipePieces;
        localUserData.perfectRecipes = defaultUserData.defaultPerfectRecipes;
        
        localUserData.donuts = new List<DonutInstanceData>();
        localUserData.bakers = new List<BakerInstanceData>();
        localUserData.ingredients = defaultUserData.defaultIngredients;
        localUserData.unlockedRecipes = defaultUserData.defaultUnlockedRecipes;
        
        foreach (var defaultdonut in defaultUserData.defaultdonuts)
        {
            for (int i = 0; i < defaultdonut.Value; i++)
            {
                var newDonut = new DonutInstanceData(donutTable[defaultdonut.Key]);
                localUserData.donuts.Add(newDonut);
            }
        }
        
        foreach (var baker in defaultUserData.defaultBakers)
        {
            BakerInstanceData newBaker = new(bakerTable[baker]);
            localUserData.bakers.Add(newBaker);
        }
        
        localUserData.tutorialIndex = 0;
        localUserData.lastTime = CustomExtensions.ConvertTimeToKr("yyyy-MM-dd HH:mm:ss");
        
        for (int i = 0; i < 6; i++)
        {
            localUserData.trayData.slots.Add(new TraySlotData());
        }
    }
    private void CreateNewDeckData(DefaultUserData defaultUserData)
    {
        localDeckData = new DeckData();

        localDeckData.uid = defaultUserData.uid;
        for (int i = 0; i < 5; i++)
        {
            localDeckData.waitingDonuts.Add(new DonutInstanceData());
        }
        localDeckData.baker = new BakerInstanceData();
        
        EventHub.Instance.RaiseEvent(new FE.RequestSaveDeckData(localDeckData));
    }
    private void CreateNewMatchMakingData(DefaultUserData defaultUserData)
    {
        localMatchData = new MatchMakingEntry();
        
        localMatchData.uid = defaultUserData.uid;
        localMatchData.isOnline = false;
        localMatchData.mmr = 1000;
        
        EventHub.Instance.RaiseEvent(new FE.RequestSaveMatchmakingData(localMatchData));
    }

    #endregion
}
