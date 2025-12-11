using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 도넛의 레벨별 스탯 증가량 </summary>

[Serializable]
public class DonutModifierData
{
    public string uid;              //고유 ID
    public string origin;           //도넛 기본 데이터 ID
    public int hpPerLevel;          //레벨당 체력 증가량
    public int hpBonus;             //5레벨당 체력 추가 증가량
    public int defPerLevel;         //레벨당 방어력 증가량
    public int defBonus;            //5레벨당 방어력 추가 증가량
    public int atkPerLevel;         //레벨당 공격력 증가량
    public int atkBonus;            //5레벨당 공격력 추가 증가량
    public float critPerLevel;      //레벨당 크리티컬 확률 증가량
    public float critBonus;         //5레벨당 크리티컬 확률 추가 증가량
    public float massPerLevel;      //레벨당 질량 증가량
    public float massBonus;         //5레벨당 질량 추가 증가량
}

[FirestoreData]
public class DonutModifierDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string origin { get; set; }
    [FirestoreProperty] public int hpPerLevel { get; set; }
    [FirestoreProperty] public int hpBonus { get; set; }
    [FirestoreProperty] public int defPerLevel { get; set; }
    [FirestoreProperty] public int defBonus { get; set; }
    [FirestoreProperty] public int atkPerLevel { get; set; }
    [FirestoreProperty] public int atkBonus { get; set; }
    [FirestoreProperty] public float critPerLevel { get; set; }
    [FirestoreProperty] public float critBonus { get; set; }
    [FirestoreProperty] public float massPerLevel { get; set; }
    [FirestoreProperty] public float massBonus { get; set; }

    public static DonutModifierDataDto CurrentDto(DonutModifierData data)
    {
        return new DonutModifierDataDto()
        {
            uid = data.uid,
            origin = data.origin,
            hpPerLevel = data.hpPerLevel,
            hpBonus = data.hpBonus,
            defPerLevel = data.defPerLevel,
            defBonus = data.defBonus,
            atkPerLevel = data.atkPerLevel,
            atkBonus = data.atkBonus,
            critPerLevel = data.critPerLevel,
            critBonus = data.critBonus,
            massPerLevel = data.massPerLevel,
            massBonus = data.massBonus
        };
    }
}