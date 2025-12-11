using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>유저 데이터</summary>

[Serializable]
public class UserData
{
    public string uid;
    public string nickname;
    public string profileBaker;

    public int energy;
    public int gold;
    public int cash;

    public int recipePieces;
    public int perfectRecipes;

    public List<DonutInstanceData> donuts = new();
    public List<BakerInstanceData> bakers = new();
    public Dictionary<string, int> ingredients = new();
    public List<string> unlockedRecipes = new();
    
    public int tutorialIndex;
    public string lastTime;

    public Dictionary<string, int> purchaseInfo = new();

    public TrayData trayData = new();
    
    //todo : 시간 제한으로 인한 임시 필드
    public Dictionary<string, int> achievement = new();     //업적 현재 점수
    public Dictionary<string, bool> achieveReward = new();  //업적 보상 수령 확인용

    public UserData() { }
    public UserData(string uid) => this.uid = uid;
}


/// <summary> Firestore 통신용 UserData DTO 클래스 </summary>
[FirestoreData]
public class UserDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string nickname { get; set; }
    [FirestoreProperty] public string profileBaker { get; set; }
    
    [FirestoreProperty] public int energy { get; set; }
    [FirestoreProperty] public int gold { get; set; }
    [FirestoreProperty] public int cash { get; set; }
    
    [FirestoreProperty] public int recipePieces { get; set; }
    [FirestoreProperty] public int perfectRecipes { get; set; }
    
    [FirestoreProperty] public List<string> unlockedRecipes { get; set; }
    
    [FirestoreProperty] public int tutorialIndex { get; set; }
    [FirestoreProperty] public string lastTime { get; set; }
    
    [FirestoreProperty] public Dictionary<string, int> achievement { get; set; }
    [FirestoreProperty] public Dictionary<string, bool> achieveReward { get; set; }

    public static UserDataDto CurrentDto(UserData data)
    {
        var dto = new UserDataDto()
        {
            uid = data.uid, 
            nickname = data.nickname,
            profileBaker = data.profileBaker,
            
            energy = data.energy,
            gold = data.gold,
            cash = data.cash,
            
            recipePieces = data.recipePieces,
            perfectRecipes = data.perfectRecipes,
            
            unlockedRecipes = data.unlockedRecipes,
            
            tutorialIndex = data.tutorialIndex,
            lastTime = CustomExtensions.ConvertTimeToKr("yyyy-MM-dd HH:mm:ss"),
            
            achievement = data.achievement,
            achieveReward = data.achieveReward
        };
        
        return dto;
    }
}

/// <summary> Firestore 통신용 User Donut Data DTO 클래스 </summary>
[FirestoreData]
public class UserDonutDataDto
{
    [FirestoreProperty] public List<DonutInstanceDataDto> donuts { get; set; }
    
    public static UserDonutDataDto CurrentDto(UserData data)
    {
        var dto = new UserDonutDataDto();
        
        dto.donuts = new List<DonutInstanceDataDto>();

        foreach (var donut in data.donuts)
        {
            dto.donuts.Add(DonutInstanceDataDto.CurrentDto(donut));
        }

        return dto;
    }
}

[FirestoreData]
public class UserBakerDataDto
{
    [FirestoreProperty] public List<BakerInstanceDataDto> bakers { get; set; }
    
    public static UserBakerDataDto CurrentDto(UserData data)
    {
        var dto = new UserBakerDataDto();
        
        dto.bakers = new List<BakerInstanceDataDto>();

        foreach (var baker in data.bakers)
        {
            dto.bakers.Add(BakerInstanceDataDto.CurrentDto(baker));       
        }
        return dto;
    }
}

[FirestoreData]
public class UserIngredientDataDto
{
    [FirestoreProperty] public Dictionary<string, int> ingredients { get; set; }
    
    public static UserIngredientDataDto CurrentDto(UserData data)
    {
        return new UserIngredientDataDto()
        {
            ingredients = data.ingredients
        };
    }
}

[FirestoreData]
public class UserPurchaseDataDto
{
    [FirestoreProperty] public Dictionary<string, int> purchaseInfo { get; set; }

    public static UserPurchaseDataDto CurrentDto(UserData data)
    {
        return new UserPurchaseDataDto() { purchaseInfo = data.purchaseInfo };
    }
}