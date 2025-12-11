using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 리더보드용 데이터 객체 </summary>
[Serializable]
public class LeaderboardEntry
{
    public string uid;                      // 유저데이터의 고유 ID
    public string nickName;
    public long score;                      // 리더보드의 점수 기획 상 MMR의 변화값
    public bool isRewardReceived;           // 관련 리더보드의 보상을 수령했는지 판단하는 변수

    public LeaderboardEntry(string uid, string nickName)
    {
        this.uid = uid;
        this.nickName = nickName;
        score = 0;
        isRewardReceived = false;
    }
}