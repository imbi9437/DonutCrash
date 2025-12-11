using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>알까기 배틀을 위한 덱 구성 데이터</summary>
//TODO : 게임 세팅 연동
[Serializable]
public class DeckData
{
    public string uid;                  //UserData의 UID
    public List<DonutInstanceData> fieldDonuts = new();   //먼저 필드에 배치될 도넛들의 Instance ID
    public List<DonutInstanceData> waitingDonuts = new(); //먼저 필드에 배치되지 않는 도넛들의 Instance ID
    public BakerInstanceData baker = new();            //UserData에 존재하는 제빵사의 Instance ID

    public int deathCount = 5;
    
    public static DeckData CopyTo(DeckData origin)
    {
        if (origin == null)
        {
            Debug.Log($"<color=red>[{typeof(DeckData)}] 원본이 존재하지 않습니다.</color>");
            return null;
        }
        
        DeckData copy = new()
        {
            uid = origin.uid,
            fieldDonuts = new List<DonutInstanceData>(),
            waitingDonuts = new List<DonutInstanceData>(),
            baker = BakerInstanceData.CopyTo(origin.baker),
            deathCount = 5,
        };

        foreach (var donut in origin.fieldDonuts)
        {
            copy.fieldDonuts.Add(DonutInstanceData.CopyTo(donut));
        }

        foreach (var donut in origin.waitingDonuts)
        {
            copy.waitingDonuts.Add(DonutInstanceData.CopyTo(donut));
        }

        return copy;
    }

    public static bool operator ==(DeckData a, DeckData b)
    {
        if (ReferenceEquals(a, null)  && ReferenceEquals(b,null)) return true;
        else if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        
        bool uidCheck = a.uid == b.uid;
        bool refCheck = ReferenceEquals(a, b);
        
        return uidCheck && refCheck;
    }
    
    public static bool operator !=(DeckData a, DeckData b) => !(a == b);
}


[FirestoreData]
public class DeckDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public List<DonutInstanceDataDto> waitingDonuts { get; set; }
    [FirestoreProperty] public BakerInstanceDataDto baker { get; set; }

    public static DeckDataDto CurrentDto(DeckData data)
    {
        return new DeckDataDto()
        {
            uid = data.uid,
            waitingDonuts = data.waitingDonuts.ConvertAll(DonutInstanceDataDto.CurrentDto),
            baker = BakerInstanceDataDto.CurrentDto(data.baker)
        };
    }
}