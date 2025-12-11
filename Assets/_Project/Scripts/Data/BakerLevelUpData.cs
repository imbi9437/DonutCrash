using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 마녀 제빵사 레벨업 필요 경험치 데이터 </summary>

[Serializable]
public class BakerLevelUpData
{
    public string uid;          //고유 ID
    public int level;           //다음 레벨
    public int requireExp;      //필요 경험치
}

[FirestoreData]
public class BakerLevelUpDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int requireExp { get; set; }

    public static BakerLevelUpDataDto CurrentDto(BakerLevelUpData data)
    {
        return new BakerLevelUpDataDto()
        {
            uid = data.uid, 
            level = data.level,
            requireExp = data.requireExp
        };
    }
}