using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>마녀 제빵사의 인스턴스 데이터</summary>

[Serializable]
public class BakerInstanceData
{
    public string uid;          //고유 ID
    public string origin;       //기본 데이터 ID
    public int level;           //현재 마녀 제빵사의 레벨
    public int exp;             //보유 경험치

    public BakerInstanceData() { }
    public BakerInstanceData(BakerData data)
    {
        uid = Guid.NewGuid().ToString();
        origin = data.uid;
        level = 1;
        exp = 0;
    }
    
    public static BakerInstanceData CopyTo(BakerInstanceData origin)
    {
        if (origin == null)
        {
            Debug.Log($"<color=red>[{typeof(BakerInstanceData)}] 원본이 존재하지 않습니다.</color>");
            return null;
        }
        
        BakerInstanceData copy = new()
        {
            uid = origin.uid, 
            origin = origin.origin, 
            level = origin.level, 
            exp = origin.exp
        };

        return copy;
    }

    public static bool operator ==(BakerInstanceData a, BakerInstanceData b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null)) return b.IsNullLike();
        if (ReferenceEquals(b, null)) return a.IsNullLike();

        bool uidCheck = a.uid == b.uid;
        bool refCheck = ReferenceEquals(a, b);
        
        return uidCheck && refCheck;
    }
    public static bool operator !=(BakerInstanceData a, BakerInstanceData b) => !(a == b);
    
    public bool IsNullLike() => string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(origin);
}

[FirestoreData]
public class BakerInstanceDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string origin { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int exp { get; set; }

    public static BakerInstanceDataDto CurrentDto(BakerInstanceData data)
    {
        var dto = new BakerInstanceDataDto();
        
        if (data == null) return dto;
        
        dto.uid = data.uid;
        dto.origin = data.origin;
        dto.level = data.level;
        dto.exp = data.exp;
        
        return dto;
    }
}