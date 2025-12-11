using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>도넛의 머지를 위한 조건 데이터</summary>

[Serializable]
public class DonutMergeData
{
    public string uid;          //고유 ID
    public int targetLevel;     //다음 레벨
    public int requireLevel;    //필요한 도넛의 레벨
    public int requireCount;    //필요한 도넛의 개수
    public int requireGold;     //머지 시 요구되는 골드
    public float successRate;   //도넛의 머지 성공 확률 (0 ~ 1)
}

[FirestoreData]
public class DonutMergeDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public int targetLevel { get; set; }
    [FirestoreProperty] public int requireLevel { get; set; }
    [FirestoreProperty] public int requireCount { get; set; }
    [FirestoreProperty] public int requireGold { get; set; }
    [FirestoreProperty] public float successRate { get; set; }

    public static DonutMergeDataDto CurrentDto(DonutMergeData data)
    {
        return new DonutMergeDataDto()
        {
            uid = data.uid,
            targetLevel = data.targetLevel,
            requireLevel = data.requireLevel,
            requireCount = data.requireCount,
            requireGold = data.requireGold,
            successRate = data.successRate
        };
    }
}