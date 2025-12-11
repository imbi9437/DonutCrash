using System.Collections.Generic;

namespace _Project.Scripts.Extensions
{
    public static class DataExtensions
    {
        #region Convert Dto To UserData
        
        /// <summary> UserDataDto To UserData </summary>
        public static void SetData(this UserData userData, UserDataDto dto)
        {
            userData.uid = dto.uid;
            userData.nickname = dto.nickname;
            userData.profileBaker = dto.profileBaker;
            
            userData.energy = dto.energy;
            userData.gold = dto.gold;
            userData.cash = dto.cash;
            
            userData.recipePieces = dto.recipePieces;
            userData.perfectRecipes = dto.perfectRecipes;
            
            userData.unlockedRecipes = dto.unlockedRecipes;
            
            userData.tutorialIndex = dto.tutorialIndex;
            userData.lastTime = dto.lastTime;
            
            userData.achievement = dto.achievement;
            userData.achieveReward = dto.achieveReward;
        }

        /// <summary> UserDonutDataDto To UserData </summary>
        public static void SetData(this UserData userData, UserDonutDataDto dto)
        {
            userData.donuts = new List<DonutInstanceData>();

            foreach (var donutDto in dto.donuts)
            {
                var donut = new DonutInstanceData();
                donut.SetData(donutDto);
                
                userData.donuts.Add(donut);
            }
        }

        /// <summary> UserBakerDataDto To UserData </summary>
        public static void SetData(this UserData userData, UserBakerDataDto dto)
        {
            userData.bakers = new List<BakerInstanceData>();

            foreach (var bakerDto in dto.bakers)
            {
                var baker = new BakerInstanceData();
                baker.SetData(bakerDto);
                
                userData.bakers.Add(baker);
            }
        }

        public static void SetData(this UserData userData, UserIngredientDataDto dto)
        {
            userData.ingredients = dto.ingredients;
        }

        public static void SetData(this UserData userData, TrayDataDto dto)
        {
            userData.trayData = new TrayData();
            userData.trayData.SetData(dto);
        }

        public static void SetData(this UserData userData, UserPurchaseDataDto dto)
        {
            userData.purchaseInfo = dto.purchaseInfo;
        }

        public static void SetData(this DeckData deckData, DeckDataDto dto)
        {
            deckData.uid = dto.uid;
            deckData.baker.SetData(dto.baker);
            deckData.waitingDonuts = new List<DonutInstanceData>();

            foreach (var donutDto in dto.waitingDonuts)
            {
                var donut = new DonutInstanceData();
                donut.SetData(donutDto);
                deckData.waitingDonuts.Add(donut);
            }
        }

        public static void SetData(this MatchMakingEntry matchMaking, MatchMakingEntryDto dto)
        {
            matchMaking.uid = dto.uid;
            matchMaking.isOnline = dto.isOnline;
            matchMaking.mmr = dto.mmr;
        }
        
        #endregion

        #region Convert Dto To InstanceData

        public static void SetData(this DonutInstanceData donut, DonutInstanceDataDto dto)
        {
            donut.uid = dto.uid;
            donut.origin = dto.origin;
            donut.level = dto.level;
            donut.isLock = dto.isLock;
        }
        public static void SetData(this BakerInstanceData baker, BakerInstanceDataDto dto)
        {
            baker.uid = dto.uid;
            baker.origin = dto.origin;
            baker.level = dto.level;
            baker.exp = dto.exp;
        }
        public static void SetData(this TrayData tray, TrayDataDto dto)
        {
            tray.grade = dto.grade;
            tray.startTime = dto.startTime;
            tray.endTime = dto.endTime;
            
            tray.slots = new List<TraySlotData>();

            foreach (var slotDataDto in dto.slots)
            {
                var slot = new TraySlotData();
                slot.SetData(slotDataDto);
                tray.slots.Add(slot);
            }
        }
        public static void SetData(this TraySlotData slot, TraySlotDataDto dto)
        {
            slot.resultId = dto.resultId;
            slot.count = dto.count;
        }
        
        #endregion

        #region Convert Dto To StaticData
        
        public static void SetData(this DonutData donut, DonutDataDto dto)
        {
            donut.uid = dto.uid;
            donut.donutName = dto.donutName;
            donut.donutDescription = dto.donutDescription;
            donut.tier = dto.tier;
            donut.maxLevel = dto.maxLevel;
            
            donut.atk = dto.atk;
            donut.hp = dto.hp;
            donut.def = dto.def;
            donut.crit = dto.crit;
            donut.mass = dto.mass;
            
            donut.skillIds = dto.skillIds;
            donut.hasEffect = dto.hasEffect;
            
            donut.resourcePath = dto.resourcePath;
        }

        public static void SetData(this BakerData baker, BakerDataDto dto)
        {
            baker.uid = dto.uid;
            baker.bakerName = dto.bakerName;
            baker.bakerDescription = dto.bakerDescription;
            baker.skills = dto.skills;
            baker.resourcePath = dto.resourcePath;
        }

        public static void SetData(this IngredientData ingredient, IngredientDataDto dto)
        {
            ingredient.uid = dto.uid;
            ingredient.ingredientName = dto.ingredientName;
            ingredient.ingredientDescription = dto.ingredientDescription;
            ingredient.resourcePath = dto.resourcePath;       
        }

        public static void SetData(this SkillData skill, SkillDataDto dto)
        {
            skill.uid = dto.uid;
            skill.logicUid = dto.logicUid;
            skill.skillName = dto.skillName;
            skill.description = dto.description;
            skill.timing = dto.timing;
            skill.cooldown = dto.cooldown;
            skill.value1 = dto.value1;
            skill.value2 = dto.value2;
            skill.value3 = dto.value3;
            skill.value4 = dto.value4;
            skill.fValue1 = dto.fValue1;
            skill.fValue2 = dto.fValue2;
            skill.fValue3 = dto.fValue3;
            skill.fValue4 = dto.fValue4;
            skill.resourcePath = dto.resourcePath;       
        }

        public static void SetData(this RecipeData recipe, RecipeDataDto dto)
        {
            recipe.uid = dto.uid;
            recipe.recipeName = dto.recipeName;
            recipe.recipeDescription = dto.recipeDescription;
            recipe.requireGold = dto.requireGold;
            
            recipe.ingredients = new List<RecipeElement>();
            dto.ingredients.ForEach(x => recipe.ingredients.Add(SetElementData(x)));
            recipe.result = SetElementData(dto.result);
            
            RecipeElement SetElementData(RecipeElementDto elementDto)
            {
                return new RecipeElement() { itemId = elementDto.itemId, count = elementDto.count };
            }
        }

        public static void SetData(this RecipeNodeData recipeNode, RecipeNodeDataDto dto)
        {
            recipeNode.uid = dto.uid;
            recipeNode.recipeId = dto.recipeId;
            recipeNode.nextNodes = dto.nextNodes;
        }

        public static void SetData(this MerchandiseData merchandise, MerchandiseDataDto dto)
        {
            merchandise.uid = dto.uid;
            merchandise.merchandiseName = dto.merchandiseName;
            merchandise.merchandiseType = dto.merchandiseType;
            merchandise.priceType = dto.priceType;
            merchandise.price = dto.price;
            merchandise.stockCount = dto.stockCount;
            merchandise.productIds = dto.productIds;
            merchandise.resourcePath = dto.resourcePath;
        }

        public static void SetData(this ProductData product, ProductDataDto dto)
        {
            product.uid = dto.uid;
            product.productType = dto.productType;
            product.productId = dto.productId;
            product.productValue = dto.productValue;
            product.resourcePath = dto.resourcePath;
        }

        public static void SetData(this DonutMergeData donutMerge, DonutMergeDataDto dto)
        {
            donutMerge.uid = dto.uid;
            donutMerge.targetLevel = dto.targetLevel;
            donutMerge.requireLevel = dto.requireLevel;
            donutMerge.requireCount = dto.requireCount;
            donutMerge.requireGold = dto.requireGold;
            donutMerge.successRate = dto.successRate;
        }

        public static void SetData(this DonutModifierData donutModifier, DonutModifierDataDto dto)
        {
            donutModifier.uid = dto.uid;
            donutModifier.origin = dto.origin;
            donutModifier.hpPerLevel = dto.hpPerLevel;
            donutModifier.hpBonus = dto.hpBonus;
            donutModifier.defPerLevel = dto.defPerLevel;
            donutModifier.defBonus = dto.defBonus;
            donutModifier.atkPerLevel = dto.atkPerLevel;
            donutModifier.atkBonus = dto.atkBonus;
            donutModifier.critPerLevel = dto.critPerLevel;
            donutModifier.critBonus = dto.critBonus;
            donutModifier.massPerLevel = dto.massPerLevel;
            donutModifier.massBonus = dto.massBonus;
        }

        public static void SetData(this BakerLevelUpData bakerLevelUp, BakerLevelUpDataDto dto)
        {
            bakerLevelUp.uid = dto.uid;
            bakerLevelUp.level = dto.level;
            bakerLevelUp.requireExp = dto.requireExp;
        }

        public static void SetData(this AchievementData achievement, AchievementDataDto dto)
        {
            achievement.uid = dto.uid;
            achievement.achievementName = dto.achievementName;
            achievement.type = dto.type;
            
            achievement.level = dto.level;
            achievement.targetScore = dto.targetScore;
            achievement.nextAchievement = dto.nextAchievement;

            achievement.rewardProducts = dto.rewardProducts;
        }

        public static void SetData(this DefaultUserData data, DefaultUserDataDto dto, string authUid)
        {
            data.uid = authUid;
            
            data.defaultEnergy = dto.defaultEnergy;
            data.defaultGold = dto.defaultGold;
            data.defaultCash = dto.defaultCash;
            
            data.defaultRecipePieces = dto.defaultRecipePieces;
            data.defaultPerfectRecipes = dto.defaultPerfectRecipes;

            data.defaultdonuts = dto.defaultdonuts;
            data.defaultBakers = dto.defaultBakers;
            data.defaultIngredients = dto.defaultIngredients;
            
            data.defaultUnlockedRecipes = dto.defaultUnlockedRecipes;
        }
        
        #endregion

        #region Convert Dto To Table
        
        public static void SetData(this Dictionary<string, DonutData> table, Dictionary<string, DonutDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var donut = new DonutData();
                donut.SetData(dto.Value);
                table.Add(dto.Key, donut);
            }
        }

        public static void SetData(this Dictionary<string, BakerData> table, Dictionary<string, BakerDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var baker = new BakerData();
                baker.SetData(dto.Value);
                table.Add(dto.Key, baker);
            }
        }

        public static void SetData(this Dictionary<string, IngredientData> table, Dictionary<string, IngredientDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new IngredientData();
                data.SetData(dto.Value);
                
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, SkillData> table, Dictionary<string, SkillDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new SkillData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, RecipeData> table, Dictionary<string, RecipeDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new RecipeData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, RecipeNodeData> table, Dictionary<string, RecipeNodeDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new RecipeNodeData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, MerchandiseData> table, Dictionary<string, MerchandiseDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new MerchandiseData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, ProductData> table, Dictionary<string, ProductDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new ProductData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, DonutMergeData> table, Dictionary<string, DonutMergeDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new DonutMergeData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, DonutModifierData> table, Dictionary<string, DonutModifierDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new DonutModifierData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }
        
        public static void SetData(this Dictionary<string, BakerLevelUpData> table, Dictionary<string, BakerLevelUpDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new BakerLevelUpData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);
            }
        }

        public static void SetData(this Dictionary<string, AchievementData> table, Dictionary<string, AchievementDataDto> dtoTable)
        {
            table.Clear();

            foreach (var dto in dtoTable)
            {
                var data = new AchievementData();
                data.SetData(dto.Value);
                table.Add(dto.Key, data);;
            }
        }
        
        #endregion


        #region Convert Table To Dto

        public static void SetDto(this Dictionary<string, DonutDataDto> dtoDic, List<DonutData> table)
        {
            dtoDic.Clear();

            foreach (var donut in table)
            {
                var dto = DonutDataDto.CurrentDto(donut);
                dtoDic.Add(donut.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, BakerDataDto> dtoDic, List<BakerData> table)
        {
            dtoDic.Clear();

            foreach (var baker in table)
            {
                var dto = BakerDataDto.CurrentDto(baker);
                dtoDic.Add(baker.uid, dto);
            }
        }
        
        public static void SetDto(this Dictionary<string, IngredientDataDto> dtoDic, List<IngredientData> table)
        {
            dtoDic.Clear();

            foreach (var ingredient in table)
            {
                var dto = IngredientDataDto.CurrentDto(ingredient);
                dtoDic.Add(ingredient.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, SkillDataDto> dtoDic, List<SkillData> table)
        {
            dtoDic.Clear();

            foreach (var skill in table)
            {
                var dto = SkillDataDto.CurrentDto(skill);
                dtoDic.Add(skill.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, RecipeDataDto> dtoDic, List<RecipeData> table)
        {
            dtoDic.Clear();

            foreach (var recipe in table)
            {
                var dto = RecipeDataDto.CurrentDto(recipe);
                dtoDic.Add(recipe.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, RecipeNodeDataDto> dtoDic, List<RecipeNodeData> table)
        {
            dtoDic.Clear();

            foreach (var node in table)
            {
                var dto = RecipeNodeDataDto.CurrentDto(node);
                dtoDic.Add(node.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, MerchandiseDataDto> dtoDic, List<MerchandiseData> table)
        {
            dtoDic.Clear();

            foreach (var merchandise in table)
            {
                var dto = MerchandiseDataDto.CurrentDto(merchandise);
                dtoDic.Add(merchandise.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, ProductDataDto> dtoDic, List<ProductData> table)
        {
            dtoDic.Clear();

            foreach (var product in table)
            {
                var dto = ProductDataDto.CurrentDto(product);
                dtoDic.Add(product.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, DonutMergeDataDto> dtoDic, List<DonutMergeData> table)
        {
            dtoDic.Clear();

            foreach (var merge in table)
            {
                var dto = DonutMergeDataDto.CurrentDto(merge);
                dtoDic.Add(merge.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, DonutModifierDataDto> dtoDic, List<DonutModifierData> table)
        {
            dtoDic.Clear();

            foreach (var donutModifierData in table)
            {
                var dto = DonutModifierDataDto.CurrentDto(donutModifierData);
                dtoDic.Add(donutModifierData.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, BakerLevelUpDataDto> dtoDic, List<BakerLevelUpData> table)
        {
            dtoDic.Clear();

            foreach (var bakerLevelUpData in table)
            {
                var dto = BakerLevelUpDataDto.CurrentDto(bakerLevelUpData);
                dtoDic.Add(bakerLevelUpData.uid, dto);
            }
        }

        public static void SetDto(this Dictionary<string, AchievementDataDto> dtoDic, List<AchievementData> table)
        {
            dtoDic.Clear();

            foreach (var achievementData in table)
            {
                var dto = AchievementDataDto.CurrentDto(achievementData);
                dtoDic.Add(achievementData.uid, dto);
            }
        }

        #endregion
        
        #region Calculate Donut Status

        public static void CalcDonutStatus(this DonutInstanceData data)
        {
            if (data == null)
                return;
            if (DataManager.Instance.TryGetDonutData(data.origin, out DonutData origin) == false)
                return;
            if (DataManager.Instance.TryGetModifierData(data.origin, out DonutModifierData modifier) == false)
                return;

            int lv = data.level;
            int bonusMultiplier = lv / 5;

            data.atk = origin.atk + lv * modifier.atkPerLevel + bonusMultiplier * modifier.atkBonus;
            data.def = origin.def + lv * modifier.defPerLevel + bonusMultiplier * modifier.defBonus;
            data.hp = origin.hp + lv * modifier.hpPerLevel + bonusMultiplier * modifier.hpBonus;
            data.crit = origin.crit + lv * modifier.critPerLevel + bonusMultiplier * modifier.critBonus;
            data.mass = origin.mass + lv * modifier.massPerLevel + bonusMultiplier * modifier.massBonus;
        }
        
        #endregion Calc Donut Status
    }
}
