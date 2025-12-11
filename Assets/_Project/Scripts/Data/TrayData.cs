using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 도넛 생산용 트레이 데이터
/// </summary>
[Serializable]
public class TrayData
{
    public List<TraySlotData> slots = new();
    public int grade = 1;
    public string startTime;
    public string endTime;

    public static TrayData CopyTo(TrayData origin)
    {
        TrayData temp = new() { grade = origin.grade, startTime = origin.startTime, endTime = origin.endTime };
        foreach (var r in origin.slots)
        {
            temp.slots.Add(TraySlotData.CopyTo(r));
        }
        return temp;
    }
}

[Serializable]
public class TraySlotData
{
    public string resultId;
    public int count;

    public static TraySlotData CopyTo(TraySlotData origin)
    {
        TraySlotData temp = new() { resultId = origin.resultId, count = origin.count };

        return temp;
    }
}

[FirestoreData]
public class TrayDataDto
{
    [FirestoreProperty] public List<TraySlotDataDto> slots { get; set; }
    [FirestoreProperty] public int grade { get; set; }
    [FirestoreProperty] public string startTime { get; set; }
    [FirestoreProperty] public string endTime { get; set; }

    public static TrayDataDto CurrentDto(TrayData data)
    {
        return new TrayDataDto()
        {
            slots = data.slots.ConvertAll(TraySlotDataDto.CurrentDto), 
            grade = data.grade, 
            startTime = data.startTime, 
            endTime = data.endTime
        };
    }
}

[FirestoreData]
public class TraySlotDataDto
{
    [FirestoreProperty] public string resultId { get; set; }
    [FirestoreProperty] public int count { get; set; }

    public static TraySlotDataDto CurrentDto(TraySlotData data)
    {
        return new TraySlotDataDto()
        {
            resultId = data.resultId, 
            count = data.count
        };
    }
}