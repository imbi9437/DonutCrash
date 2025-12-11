using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>도넛 생산을 위한 재료 아이템 데이터</summary>
[Serializable]
public class IngredientData
{
    public string uid;                      //고유 ID
    public string ingredientName;           //재료 아이템 이름
    public string ingredientDescription;    //재료 아이템 설명
    public string resourcePath;             //관련 에셋 경로
}

[FirestoreData]
public class IngredientDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string ingredientName { get; set; }
    [FirestoreProperty] public string ingredientDescription { get; set; }
    [FirestoreProperty] public string resourcePath { get; set; }
    
    public static IngredientDataDto CurrentDto(IngredientData data)
    {
        return new IngredientDataDto()
        {
            uid = data.uid, 
            ingredientName = data.ingredientName,
            ingredientDescription = data.ingredientDescription,
            resourcePath = data.resourcePath
        };
    }
}