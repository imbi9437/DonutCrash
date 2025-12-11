using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>마녀 제빵사 기본 데이터</summary>

[Serializable]
public class BakerData
{
    public string uid;                  //고유 ID
    public string bakerName;            //마녀 제빵사 이름
    public string bakerDescription;     //마녀 제빵사 설명
    public List<string> skills = new(); //마녀 스킬 ID

    public string resourcePath;         //관련 에셋 경로
}

[FirestoreData]
public class BakerDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string bakerName { get; set; }
    [FirestoreProperty] public string bakerDescription { get; set; }
    [FirestoreProperty] public List<string> skills { get; set; }
    [FirestoreProperty] public string resourcePath { get; set; }

    public static BakerDataDto CurrentDto(BakerData data)
    {
        return new BakerDataDto()
        {
            uid = data.uid,
            bakerName = data.bakerName,
            bakerDescription = data.bakerDescription,
            skills = data.skills,
            resourcePath = data.resourcePath
        };
    }
}