using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary> 도넛의 인스턴스 데이터 </summary>

[Serializable]
public class DonutInstanceData
{
    public string uid;          //고유 ID
    public string name;    //도넛 이름
    public string origin;       //도넛의 기본 데이터 ID
    public int level;           //현재 도넛의 레벨
    public int atk;             //현재 도넛의 공격력
    public int def;             //현재 도넛의 방어력
    public int hp;              //현재 도넛의 체력

    public float crit;          //현재 도넛의 크리티컬 확률
    public float mass;          //현재 도넛의 질량

    public bool isLock;         //현재 도넛의 머지 잠금

    public DonutInstanceData() { }
    public DonutInstanceData(DonutData data)
    {
        uid = Guid.NewGuid().ToString();
        origin = data.uid;
        name = data.donutName;
        level = 1;

        atk = data.atk;
        def = data.def;
        hp = data.hp;

        crit = data.crit;
        mass = data.mass;

        isLock = false;
    }

    public static DonutInstanceData CopyTo(DonutInstanceData origin)
    {
        if (origin == null)
        {
            Debug.Log($"<color=red>[{typeof(DonutInstanceData)}] 원본이 존재하지 않습니다.</color>");
            return null;
        }

        DonutInstanceData copy = new()
        {
            uid = origin.uid,
            origin = origin.origin,
            name = origin.name,
            level = origin.level,
            atk = origin.atk,
            def = origin.def,
            hp = origin.hp,
            crit = origin.crit,
            mass = origin.mass,
            isLock = origin.isLock
        };

        return copy;
    }

    public static bool operator ==(DonutInstanceData a, DonutInstanceData b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null)) return b.IsNullLike();
        if (ReferenceEquals(b, null)) return a.IsNullLike();

        bool uidCheck = a.uid == b.uid;
        bool originCheck = a.origin == b.origin;
        bool refCheck = ReferenceEquals(a, b);

        return uidCheck && originCheck && refCheck;
    }

    public static bool operator !=(DonutInstanceData a, DonutInstanceData b) => !(a == b);

    private bool IsNullLike() => string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(origin);
}

[FirestoreData]
public class DonutInstanceDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string origin { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public bool isLock { get; set; }
    
    public static DonutInstanceDataDto CurrentDto(DonutInstanceData data)
    {
        var dto = new DonutInstanceDataDto();
        
        if (data == null) return dto;
        
        dto.uid = data.uid;
        dto.origin = data.origin;
        dto.level = data.level;
        dto.isLock = data.isLock;

        return dto;
    }
}