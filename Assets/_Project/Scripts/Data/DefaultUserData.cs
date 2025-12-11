using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultUserData
{
    public string uid;
    
    public int defaultEnergy;
    public int defaultGold;
    public int defaultCash;
    
    public int defaultRecipePieces;
    public int defaultPerfectRecipes;
    
    public Dictionary<string, int> defaultdonuts = new();
    public Dictionary<string, int> defaultIngredients = new();
    public List<string> defaultBakers;
    
    public List<string> defaultUnlockedRecipes;
}

[FirestoreData]
public class DefaultUserDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    
    [FirestoreProperty] public int defaultEnergy { get; set; }
    [FirestoreProperty] public int defaultGold { get; set; }
    [FirestoreProperty] public int defaultCash { get; set; }
    
    [FirestoreProperty] public int defaultRecipePieces { get; set; }
    [FirestoreProperty] public int defaultPerfectRecipes { get; set; }
    
    [FirestoreProperty] public Dictionary<string, int> defaultdonuts { get; set; }
    [FirestoreProperty] public Dictionary<string, int> defaultIngredients { get; set; }
    [FirestoreProperty] public List<string> defaultBakers { get; set; }
    
    [FirestoreProperty] public List<string> defaultUnlockedRecipes { get; set; }

    public static DefaultUserDataDto CurrentDto(DefaultUserData data)
    {
        return new DefaultUserDataDto()
        {
            uid = data.uid,
            defaultEnergy = data.defaultEnergy,
            defaultGold = data.defaultGold,
            defaultCash = data.defaultCash,
            defaultRecipePieces = data.defaultRecipePieces,
            defaultPerfectRecipes = data.defaultPerfectRecipes,
            defaultdonuts = data.defaultdonuts,
            defaultIngredients = data.defaultIngredients,
            defaultBakers = data.defaultBakers,
            defaultUnlockedRecipes = data.defaultUnlockedRecipes
        };
    }
}