using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    PVPVictory,
    TryMerge,
    UnlockDonut,
}

[Serializable]
public class AchievementData
{
    public string uid;
    public string achievementName;
    public AchievementType type;

    public int level;
    public int targetScore;
    public string nextAchievement;

    public List<string> rewardProducts;
}

[FirestoreData]
public class AchievementDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string achievementName { get; set; }
    [FirestoreProperty] public AchievementType type { get; set; }
    
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int targetScore { get; set; }
    [FirestoreProperty] public string nextAchievement { get; set; }
    
    [FirestoreProperty] public List<string> rewardProducts { get; set; }

    public static AchievementDataDto CurrentDto(AchievementData data)
    {
        return new AchievementDataDto()
        {
            uid = data.uid,
            achievementName = data.achievementName,
            type = data.type,
            level = data.level,
            targetScore = data.targetScore,
            nextAchievement = data.nextAchievement,
            rewardProducts = data.rewardProducts
        };
    }
}