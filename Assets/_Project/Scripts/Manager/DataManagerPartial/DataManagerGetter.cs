using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO : Deep Clone 추가 및 메모리 누수 방지
public partial class DataManager
{
    #region UserData Getter
    
    public string UserUid => localUserData.uid;
    public string UserNickName => localUserData.nickname;
    public string UserProfileBaker => localUserData.profileBaker;

    public int UserEnergy => localUserData.energy;
    public int UserGold => localUserData.gold;
    public int UserCash => localUserData.cash;

    public int PerfectRecipe => localUserData.perfectRecipes;
    public int RecipePieces => localUserData.recipePieces;

    /// <summary> 깊은 복사 반환 사용시 주의 필요 </summary>
    public List<DonutInstanceData> Donuts
    {
        get
        {
            List<DonutInstanceData> temp = new();
            if (localUserData.donuts == null) return temp;
            foreach (var donut in localUserData.donuts)
            {
                temp.Add(DonutInstanceData.CopyTo(donut));
            }

            return temp;
        }
    }

    /// <summary> 깊은 복사 반환 사용시 주의 필요 </summary>
    public List<BakerInstanceData> Bakers
    {
        get
        {
            List<BakerInstanceData> temp = new();
            if (localUserData.bakers == null) return temp;
            foreach (var baker in localUserData.bakers)
            {
                temp.Add(BakerInstanceData.CopyTo(baker));
            }

            return temp;
        }
    }

    /// <summary> 깊은 복사 반환 사용시 주의 필요 </summary>
    public Dictionary<string, int> Ingredients
    {
        get
        {
            Dictionary<string, int> temp = new();

            foreach (var ingredient in localUserData.ingredients)
            {
                temp.Add(ingredient.Key, ingredient.Value);
            }

            return temp;
        }
    }

    /// <summary> 깊은 복사 반환 사용시 주의 필요 </summary>
    public List<string> UnlockRecipe
    {
        get
        {
            List<string> temp = new();

            foreach (var recipe in localUserData.unlockedRecipes)
            {
                temp.Add(recipe);
            }

            return temp;
        }
    }

    public int TutorialIndex => localUserData.tutorialIndex;
    public DateTime LastTime => DateTime.Parse(localUserData.lastTime);

    public Dictionary<string, int> PurchaseInfo
    {
        get
        {
            Dictionary<string, int> temp = new();

            foreach (var kvp in localUserData.purchaseInfo)
            {
                temp.Add(kvp.Key, kvp.Value);
            }

            return temp;
        }
    }

    public TrayData TrayData => TrayData.CopyTo(localUserData.trayData);
    
    /// <summary> 깊은 복사 반환 사용시 주의 필요 </summary>
    public DeckData DeckData
    {
        get
        {
            var deck = DeckData.CopyTo(localDeckData);

            foreach (var donut in deck.waitingDonuts)
            {
                if (donut == null) continue;
                if (donutTable.TryGetValue(donut.origin, out var origin) == false) continue;
                if (donutModifierTable.TryGetValue(donut.origin, out var modifier) == false) continue;

                int lv = donut.level;
                int bonusMultiplier = lv / 5;
                
                donut.atk = origin.atk + lv * modifier.atkPerLevel + bonusMultiplier * modifier.atkBonus;
                donut.def = origin.def + lv * modifier.defPerLevel + bonusMultiplier * modifier.defBonus;
                donut.hp = origin.hp + lv * modifier.hpPerLevel + bonusMultiplier * modifier.hpBonus;
                donut.crit = origin.crit + lv * modifier.critPerLevel + bonusMultiplier * modifier.critBonus;
                donut.mass = origin.mass + lv * modifier.massPerLevel + bonusMultiplier * modifier.massBonus;
            }

            return deck;
        }
    }

    public int MMR => localMatchData.mmr;
    public bool IsOnline => localMatchData.isOnline;

    public bool TryGetUserDonut(string uid, out DonutInstanceData data)
    {
        var origin = localUserData.donuts.FirstOrDefault(d => d.uid == uid);
        data = DonutInstanceData.CopyTo(origin);
        return origin != null;
    }
    
    #endregion

    #region 정적 아이템 객체 데이터 Getter

    public bool TryGetDonutData(string uid, out DonutData data)
    {
        data = null;
        if (donutTable.TryGetValue(uid, out data) == false) return false;
        
        return donutTable.TryGetValue(uid, out data);
    }
    public bool TryGetBakerData(string uid, out BakerData data) => bakerTable.TryGetValue(uid, out data);
    public bool TryGetIngredientData(string uid, out IngredientData data) => ingredientTable.TryGetValue(uid, out data);


    public List<DonutData> GetDonutTable() => donutTable.Values.ToList();
    public List<BakerData> GetBakerTable() => bakerTable.Values.ToList();
    public List<IngredientData> GetIngredientTable() => ingredientTable.Values.ToList();

    #endregion

    #region 시스템 객체 데이터 Getter

    public bool TryGetSkillData(string uid, out SkillData data) => skillTable.TryGetValue(uid, out data);
    public bool TryGetRecipeData(string recipeId, out RecipeData data) => recipeTable.TryGetValue(recipeId, out data);
    public bool TryGetResultRecipeData(string resultId, out RecipeData data)
    {
        data = recipeTable.Values.FirstOrDefault(x => x.result.itemId.Contains(resultId));
        return data != null;
    }
    public bool TryGetRecipeNodeData(string nodeId, out RecipeNodeData data) => recipeNodeTable.TryGetValue(nodeId, out data);
    public bool TryGetPrevRecipeNodeData(string nodeId, out RecipeNodeData data)
    {
        data = recipeNodeTable.Values.FirstOrDefault(x => x.nextNodes.Contains(nodeId));
        return data != null;
    }
    public bool TryGetMerchandiseData(string merchandiseId, out MerchandiseData data) => merchandiseTable.TryGetValue(merchandiseId, out data);
    public bool TryGetProductData(string productId, out ProductData data) => productTable.TryGetValue(productId, out data);
    public bool TryGetAchievementData(string achievementId, out AchievementData data) => achievementTable.TryGetValue(achievementId, out data);

    public List<SkillData> GetSkillTable() => skillTable.Values.ToList();
    public List<RecipeData> GetRecipeTable() => recipeTable.Values.ToList();
    public List<RecipeNodeData> GetRecipeNodeTable() => recipeNodeTable.Values.ToList();
    public List<MerchandiseData> GetMerchandiseTable() => merchandiseTable.Values.ToList();
    public List<ProductData> GetProductTable() => productTable.Values.ToList();
    public List<AchievementData> GetAchievementTable() => achievementTable.Values.ToList();

    #endregion

    #region 육성 관련 데이터 Getter

    public bool TryGetMergeData(int level, out DonutMergeData data) => donutMergeTable.TryGetValue(level, out data);
    public bool TryGetModifierData(string uid, out DonutModifierData data) => donutModifierTable.TryGetValue(uid, out data);
    public bool TryGetBakerLevelUpData(int level, out BakerLevelUpData data) => bakerLevelUpTable.TryGetValue(level, out data);

    public List<DonutMergeData> GetDonutMergeTable() => donutMergeTable.Values.ToList();
    public List<DonutModifierData> GetDonutModifierTable() => donutModifierTable.Values.ToList();
    public List<BakerLevelUpData> GetBakerLevelUpTable() => bakerLevelUpTable.Values.ToList();

    #endregion

    //Todo : 테스트용 메서드'
    public UserData User => localUserData;
    public void TryGetNodeDataList(List<string> nodes)
    {
        foreach (var r in recipeNodeTable)
        {
            nodes.Add(r.Key);
        }
    }
    public void TryGetRecipeDataList(List<string> recipes)
    {
        foreach (var r in recipeTable)
        {
            recipes.Add(r.Key);
        }
    }
    public int GetIngredientCount(string uid)
    {
        if (!localUserData.ingredients.TryGetValue(uid, out int count))
        {
            localUserData.ingredients.Add(uid, 0);
            Debug.Log($"{uid} : {count}");
            return 0;
        }
        return count;
    }

}
