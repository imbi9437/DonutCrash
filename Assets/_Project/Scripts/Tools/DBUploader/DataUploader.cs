using _Project.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using TCS = TableConverterSettings;

public class DataUploader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button uploadButton;
    [SerializeField] private TMP_Text fileNameText;
    [SerializeField] private Button quitButton;

    private TableConverterSettings settings;
    private Dictionary<TCS.DBDataType, Action<TCS.DBDataType>> uploadActions;

    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase DB { get; private set; }
    private FirebaseFirestore Firestore { get; set; }
    
    private CollectionReference tableCollection;


    private void Awake()
    {
        settings = Resources.Load<TableConverterSettings>("TableConverterSettings");

        InitDropdown();
        InitUploadActions();
        
        quitButton.onClick.AddListener(Application.Quit);
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveAllListeners();
        uploadButton.onClick.RemoveAllListeners();
    }

    private async void Start()
    {
        logText.text = "서버 접속 중";
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            logText.text = "서버 접속 성공";

            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            Firestore = FirebaseFirestore.DefaultInstance;

            InitButton();
            
            tableCollection = Firestore.Collection("table");
        }
        else
        {
            logText.text = $"서버 접속 실패 : {status}";
        }
    }

    #region UI Functions

    private void InitDropdown()
    {
        dropdown.ClearOptions();

        List<string> options = new();

        foreach (TCS.DBDataType dataType in Enum.GetValues(typeof(TCS.DBDataType)))
        {
            options.Add(settings.ConsoleName[dataType]);
        }

        dropdown.AddOptions(options);

        dropdown.onValueChanged.AddListener(ChangedOptions);
        ChangedOptions(0);
    }

    private void ChangedOptions(int index)
    {
        string fileName = settings.GetFileName(index);
        fileNameText.text = $"File Name : {fileName}";
    }

    private void InitButton()
    {
        uploadButton.onClick.AddListener(StartUpload);
    }

    #endregion

    private void InitUploadActions()
    {
        uploadActions = new()
        {
            { TCS.DBDataType.Donut , ReadData<DonutData>},
            { TCS.DBDataType.Baker , ReadData<BakerData>},
            { TCS.DBDataType.Ingredient, ReadData<IngredientData>},
            
            { TCS.DBDataType.Skill, ReadData<SkillData>},
            { TCS.DBDataType.Recipe, ReadData<RecipeData>},
            { TCS.DBDataType.RecipeNode, ReadData<RecipeNodeData>},
            { TCS.DBDataType.Merchandise, ReadData<MerchandiseData>},
            { TCS.DBDataType.Product, ReadData<ProductData>},
            { TCS.DBDataType.Achievement, ReadData<AchievementData>},
            
            { TCS.DBDataType.DonutMerge, ReadData<DonutMergeData>},
            { TCS.DBDataType.DonutModifier, ReadData<DonutModifierData>},
            { TCS.DBDataType.BakerLevelUp, ReadData<BakerLevelUpData>},
            
            { TCS.DBDataType.DefaultUser, ReadData<DefaultUserData>},
        };
    }
    
    private void StartUpload()
    {
        logText.text = "데이터 확인 중";
        uploadButton.interactable = false;
        dropdown.interactable = false;
        
        var type = (TCS.DBDataType)dropdown.value;
        uploadActions[type]?.Invoke(type);
    }

    private void ReadData<T>(TCS.DBDataType type)
    {
        string url = settings.GetAPIUrl(type);
        var result = RestAPI.Get<List<T>>(url);
        result.callback += (data) => UploadDB(data).Forget();
        result.errorCallback += (e) =>
        {
            uploadButton.interactable = true;
            dropdown.interactable = true;
            logText.text = $"테이블 읽기 실패 : {e}";
        };
    }

    private async UniTask UploadDB<T>(List<T> list)
    {
        logText.text = "데이터 업로드 중";
        
        try
        {
            if (list == null || list.Count <= 0)
            {
                logText.text = "테이블에 데이터가 존재하지 않습니다.";
                dropdown.interactable = true;
                uploadButton.interactable = true;
                return;
            }

            switch (list)
            {
                case List<DonutData> donutList:
                    await UploadDonut(donutList, "donut_table");
                    break;
                case List<BakerData> bakerList:
                    await UploadBaker(bakerList, "baker_table");
                    break;
                case List<IngredientData> ingredientList:
                    await UploadIngredient(ingredientList, "ingredient_table");
                    break;
                case List<SkillData> skillList:
                    await UploadSkill(skillList, "skill_table");
                    break;
                case List<RecipeData> recipeList:
                    await UploadRecipe(recipeList, "recipe_table");
                    break;
                case List<RecipeNodeData> recipeNodeList:
                    await UploadRecipeNode(recipeNodeList, "recipeNode_table");
                    break;
                case List<MerchandiseData> merchandiseList:
                    await UploadMerchandise(merchandiseList, "merchandise_table");
                    break;
                case List<ProductData> productList:
                    await UploadProduct(productList, "product_table");
                    break;
                case List<AchievementData> achievementList:
                    await UploadAchievement(achievementList, "achievement_table");
                    break;
                case List<DonutMergeData> donutMergeList:
                    await UploadDonutMerge(donutMergeList, "donutMerge_table");
                    break;
                case List<DonutModifierData> donutModifierList:
                    await UploadDonutModifier(donutModifierList, "donutModifier_table");
                    break;
                case List<BakerLevelUpData> bakerLevelUpList:
                    await UploadBakerLevelUp(bakerLevelUpList, "bakerLevelUp_table");
                    break;
                case List<DefaultUserData> defaultUserList:
                    await UploadDefaultUser(defaultUserList, "defaultUser_table");
                    break;
            }
            
            uploadButton.interactable = true;
            dropdown.interactable = true;
            logText.text = "데이터 업로드 완료";
        }
        catch (Exception e)
        {
            logText.text = $"데이터 업로드 실패 : {e.Message}";
            
            uploadButton.interactable = true;
            dropdown.interactable = true;
            
            Debug.LogError(e);
        }
    }

    private async UniTask UploadDonut(List<DonutData> list, string path)
    {
        var docs = tableCollection.Document(path);
            
        Dictionary<string, DonutDataDto> dto = new();
        dto.SetDto(list);
            
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadBaker(List<BakerData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, BakerDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadIngredient(List<IngredientData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, IngredientDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    
    private async UniTask UploadSkill(List<SkillData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, SkillDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadRecipe(List<RecipeData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, RecipeDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadRecipeNode(List<RecipeNodeData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, RecipeNodeDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }

    private async UniTask UploadMerchandise(List<MerchandiseData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, MerchandiseDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadProduct(List<ProductData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, ProductDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    
    private async UniTask UploadDonutMerge(List<DonutMergeData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, DonutMergeDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadDonutModifier(List<DonutModifierData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, DonutModifierDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    private async UniTask UploadBakerLevelUp(List<BakerLevelUpData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, BakerLevelUpDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }

    private async UniTask UploadDefaultUser(List<DefaultUserData> list, string path)
    {
        var docs = tableCollection.Document(path);

        var dto = DefaultUserDataDto.CurrentDto(list[0]);
        
        await docs.SetAsync(dto).AsUniTask();
    }
    
    private async UniTask UploadAchievement(List<AchievementData> list, string path)
    {
        var docs = tableCollection.Document(path);

        Dictionary<string, AchievementDataDto> dto = new();
        dto.SetDto(list);
        
        await docs.SetAsync(dto).AsUniTask();
    }
}