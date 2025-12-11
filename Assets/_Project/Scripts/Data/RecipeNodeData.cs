using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>트리 형태의 레시피 해금을 위한 노드 데이터</summary>
[Serializable]
public class RecipeNodeData
{
    public string uid;              //고유 ID
    public string recipeId;         //해금되는 레시피 ID
    public List<string> nextNodes = new();  //해금 가능해지는 다음 노드 ID들 정보
}

[FirestoreData]
public class RecipeNodeDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string recipeId { get; set; }
    [FirestoreProperty] public List<string> nextNodes { get; set; }

    public static RecipeNodeDataDto CurrentDto(RecipeNodeData data)
    {
        return new RecipeNodeDataDto()
        {
            uid = data.uid, 
            recipeId = data.recipeId, 
            nextNodes = data.nextNodes
        };
    }
}