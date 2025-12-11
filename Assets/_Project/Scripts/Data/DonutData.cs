using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DonutTier
{
    Tier1,
    Tier2,
    Tier3,
}

/// <summary>도넛의 기본 데이터</summary>
[Serializable]
public class DonutData
{
    public string uid;                  //고유 ID
    public string donutName;            //도넛의 이름
    public string donutDescription;     //도넛의 설명
    public DonutTier tier;                    //도넛의 티어
    public int maxLevel;                //도넛의 최고 레벨
    
    public int atk;                  //생산 시 등장하는 최대 공격력
    public int hp;                   //생산 시 등장하는 최대 체력
    public int def;                  //생산 시 등장하는 최대 방어력
    public float crit;               //생산 시 등장하는 최대 크리티컬 확률
    public float mass;                  //도넛의 질량
    
    public List<string> skillIds = new();       //도넛이 보유한 스킬의 ID
    public bool hasEffect;              //충돌시 충돌 이펙트가 표시되는지 판단하는 변수

    public string resourcePath;         //관련 에셋 경로
}

[FirestoreData]
public class DonutDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string donutName { get; set; }
    [FirestoreProperty] public string donutDescription { get; set; }
    [FirestoreProperty] public DonutTier tier { get; set; }
    [FirestoreProperty] public int maxLevel { get; set; }
    
    [FirestoreProperty] public int atk { get; set; }
    [FirestoreProperty] public int hp { get; set; }
    [FirestoreProperty] public int def { get; set; }
    [FirestoreProperty] public float crit { get; set; }
    [FirestoreProperty] public float mass { get; set; }
    
    [FirestoreProperty] public List<string> skillIds { get; set; }
    [FirestoreProperty] public bool hasEffect { get; set; }
    
    [FirestoreProperty] public string resourcePath { get; set; }

    public static DonutDataDto CurrentDto(DonutData data)
    {
        var dto = new DonutDataDto();
        dto.uid = data.uid;
        dto.donutName = data.donutName;
        dto.donutDescription = data.donutDescription;
        dto.tier = data.tier;
        dto.maxLevel = data.maxLevel;
        
        dto.atk = data.atk;
        dto.hp = data.hp;
        dto.def = data.def;
        dto.crit = data.crit;
        dto.mass = data.mass;
        
        dto.skillIds = data.skillIds;
        dto.hasEffect = data.hasEffect;
        
        dto.resourcePath = data.resourcePath;
        
        return dto;
    }
}