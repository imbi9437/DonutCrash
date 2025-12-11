using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName = "TableConverterSettings", menuName = "Tools/TableConverterSettings", order = 0)]
public class TableConverterSettings : ScriptableObject
{
    public enum DBDataType
    {
        Donut,
        Baker,
        Ingredient,
        
        Skill,
        Recipe,
        RecipeNode,
        Merchandise,
        Product,
        Achievement,

        DonutMerge,
        DonutModifier,
        BakerLevelUp,
        
        DefaultUser,
    }
    
    public string apiUrl;

    public readonly Dictionary<DBDataType, string> ConsoleName = new()
    {
        { DBDataType.Donut, "도넛" },
        { DBDataType.Baker, "마녀 제빵사" },
        { DBDataType.Ingredient, "재료" },
        
        { DBDataType.Skill, "스킬" },
        { DBDataType.Recipe, "레시피" },
        { DBDataType.RecipeNode, "레시피 해금 트리" },
        { DBDataType.Merchandise, "상점 상품"},
        { DBDataType.Product, "상품 판매 제품"},
        { DBDataType.Achievement , "업적"},
        
        { DBDataType.DonutMerge, "머지" },
        { DBDataType.DonutModifier, "도넛 스탯 변화량" },
        { DBDataType.BakerLevelUp, "마녀 제빵사 경험치 테이블" },
        
        { DBDataType.DefaultUser, "초기 유저 데이터"},
    };

    public string GetFileName(int index) => GetFileName((DBDataType)index);
    public string GetFileName(DBDataType type) => $"{type.ToString()}Data";
    public string GetAPIUrl(DBDataType type) => $"{apiUrl}?file={GetFileName(type)}&sheet=Table";
}