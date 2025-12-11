using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>도넛 생산을 위한 레시피 데이터</summary>

[Serializable]
public class RecipeData
{
    public string uid;                              //고유 ID
    public string recipeName;                       //레시피 이름
    public string recipeDescription;                //레시피 설명

    public int requireGold;                         //소모 골드
    public List<RecipeElement> ingredients = new(); //재료 정보
    public RecipeElement result = new();                    //결과물 정보
}

/// <summary>레시피의 재료 및 결과물 정보 컨테이너</summary>
[Serializable]
public class RecipeElement
{
    public string itemId;
    public int count;
}

[FirestoreData]
public class RecipeDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string recipeName { get; set; }
    [FirestoreProperty] public string recipeDescription { get; set; }
    [FirestoreProperty] public int requireGold { get; set; }
    [FirestoreProperty] public List<RecipeElementDto> ingredients { get; set; }
    [FirestoreProperty] public RecipeElementDto result { get; set; }

    public static RecipeDataDto CurrentDto(RecipeData data)
    {
        var dto = new RecipeDataDto();
        
        dto.uid = data.uid;
        dto.recipeName = data.recipeName;
        dto.recipeDescription = data.recipeDescription;
        dto.requireGold = data.requireGold;
        
        dto.ingredients = new List<RecipeElementDto>();
        data.ingredients.ForEach(x => dto.ingredients.Add(RecipeElementDto.CurrentDto(x)));
        dto.result = RecipeElementDto.CurrentDto(data.result);
        
        return dto;
    }
}

[FirestoreData]
public class RecipeElementDto
{
    [FirestoreProperty] public string itemId { get; set; }
    [FirestoreProperty] public int count { get; set; }

    public static RecipeElementDto CurrentDto(RecipeElement data)
    {
        return new RecipeElementDto()
        {
            itemId = data.itemId,
            count = data.count
        };
    }
}