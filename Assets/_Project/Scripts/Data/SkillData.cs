using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using UnityEngine;

public enum Timing
{
    Spawn,
    TurnStart,
    TurnProcess,
    TurnEnd,
    TeamTurnStart,
    TeamTurnProcess,
    TeamTurnEnd,
    EnemyTurnStart,
    EnemyTurnProcess,
    EnemyTurnEnd,
    BattleStart,
    BattleEnd,
    Hit,
}

/// <summary> DB에서 받아올 스킬의 데이터 클래스 </summary>
[Serializable]
public class SkillData
{
    public string uid;          //고유 id
    public string logicUid;     //스킬 로직 id
    public string skillName;    //스킬 이름
    public string description;  //스킬 설명
    public Timing timing;       //스킬 발동 타이밍 인덱스
    public int cooldown;        //쿨다운
    public int value1;          //정수형 스킬 밸류1
    public int value2;          //정수형 스킬 밸류2
    public int value3;          //정수형 스킬 밸류3
    public int value4;          //정수형 스킬 밸류4
    public float fValue1;       //실수형 스킬 밸류1
    public float fValue2;       //실수형 스킬 밸류2
    public float fValue3;       //실수형 스킬 밸류3
    public float fValue4;       //실수형 스킬 밸류4
    [JsonIgnore] public int skillLevel;     //마녀 스킬을 위한 스킬레벨. 스킬을 부여할 때 마녀 레벨에 맞게 부여할 필드

    public string resourcePath; //관련 에셋 경로

    
    public SkillData() { }
    // 깊은 복사를 위한 생성자
    public SkillData(SkillData skillData)
    {
        uid = skillData.uid;
        logicUid = skillData.logicUid;
        skillName = skillData.skillName;
        description = skillData.description;
        timing = skillData.timing;
        cooldown = skillData.cooldown;
        value1 = skillData.value1;
        value2 = skillData.value2;
        value3 = skillData.value3;
        value4 = skillData.value4;
        fValue1 = skillData.fValue1;
        fValue2 = skillData.fValue2;
        fValue3 = skillData.fValue3;
        fValue4 = skillData.fValue4;
        skillLevel = skillData.skillLevel;
        
        resourcePath = skillData.resourcePath;
    }

    // 원본 스킬 데이터의 수정 없이 스킬레벨을 할당하기 위한 생성자
    public SkillData(SkillData skillData, int skillLevel)
    {
        uid = skillData.uid;
        logicUid = skillData.logicUid;
        skillName = skillData.skillName;
        description = skillData.description;
        timing = skillData.timing;
        cooldown = skillData.cooldown;
        value1 = skillData.value1;
        value2 = skillData.value2;
        value3 = skillData.value3;
        value4 = skillData.value4;
        fValue1 = skillData.fValue1;
        fValue2 = skillData.fValue2;
        fValue3 = skillData.fValue3;
        fValue4 = skillData.fValue4;
        this.skillLevel = skillLevel;
        
        resourcePath = skillData.resourcePath;
    }
}

[FirestoreData]
public class SkillDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string logicUid { get; set; }
    [FirestoreProperty] public string skillName { get; set; }
    [FirestoreProperty] public string description { get; set; }
    [FirestoreProperty] public Timing timing { get; set; }
    [FirestoreProperty] public int cooldown { get; set; }
    [FirestoreProperty] public int value1 { get; set; }
    [FirestoreProperty] public int value2 { get; set; }
    [FirestoreProperty] public int value3 { get; set; }
    [FirestoreProperty] public int value4 { get; set; }
    [FirestoreProperty] public float fValue1 { get; set; }
    [FirestoreProperty] public float fValue2 { get; set; }
    [FirestoreProperty] public float fValue3 { get; set; }
    [FirestoreProperty] public float fValue4 { get; set; }
    [FirestoreProperty] public string resourcePath { get; set; }

    public static SkillDataDto CurrentDto(SkillData data)
    {
        return new SkillDataDto()
        {
            uid = data.uid,
            logicUid = data.logicUid,
            skillName = data.skillName,
            description = data.description,
            timing = data.timing,
            cooldown = data.cooldown,
            value1 = data.value1,
            value2 = data.value2,
            value3 = data.value3,
            value4 = data.value4,
            fValue1 = data.fValue1,
            fValue2 = data.fValue2,
            fValue3 = data.fValue3,
            fValue4 = data.fValue4,
            resourcePath = data.resourcePath
        };
    }
}