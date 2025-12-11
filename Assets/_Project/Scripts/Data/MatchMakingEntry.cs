using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 매치메이킹을 위한 데이터 객체 </summary>

//TODO : 게임 세팅 연동
[Serializable]
public class MatchMakingEntry
{
    public string uid;          // 유저 데이터의 고유 ID
    public bool isOnline;       // 해당 유저가 접속중인지 판단하는 변수
    public int mmr;             // 해당 유저의 매치메이킹을 위한 점수

    public MatchMakingEntry() { }
}

[FirestoreData]
public class MatchMakingEntryDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public bool isOnline { get; set; }
    [FirestoreProperty] public int mmr { get; set; }
    
    public static MatchMakingEntryDto CurrentDto(MatchMakingEntry data)
    {
        return new MatchMakingEntryDto()
        {
            uid = data.uid, 
            isOnline = data.isOnline, 
            mmr = data.mmr
        };
    }
}