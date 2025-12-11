using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CustomExtensions
{
    public static bool WaitUntilLoaded(this AsyncOperation op)
    {
        return op.isDone == false && op.progress < 0.9f;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (action == null)
            return;

        foreach (T i in source)
        {
            action(i);
        }
    }
    
    public static string SerializeObject(this object obj, Formatting formatting = Formatting.None) => JsonConvert.SerializeObject(obj, formatting);

    public static T DeSerializeObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json);
    
    public static UniTask<T> DeserializeObjectAsync<T>(this string json)
    {
        return UniTask.RunOnThreadPool(json.DeSerializeObject<T>);
    }

    public static string ConvertTimeToKr(string format)
    {
        DateTime time = DateTime.UtcNow;
        DateTime convertedTime = time.AddHours(9);
        return convertedTime.ToString(format);
    }

    public static void GetExp(this BakerInstanceData bakerInstanceData, int value)
    {
        if (bakerInstanceData.IsNullLike())
        {
            Debug.LogError($"BakerInstanceData was Null Like");
            return;
        }
        
        if ((DataManager.Instance?.TryGetBakerLevelUpData(bakerInstanceData.level + 1, out BakerLevelUpData blud) ?? false) == false)
        {
            Debug.LogError($"BakerLevelUpTable not found for {bakerInstanceData.level + 1}");
            return;
        }

        int requireExp = blud.requireExp;
        bakerInstanceData.exp += value;
        if (bakerInstanceData.exp >= requireExp)
        {
            bakerInstanceData.exp -= requireExp;
            bakerInstanceData.level += 1;
        }
    }

    public static int ToPercent(this float value) => Mathf.RoundToInt(value * 100);

    public static List<T> Shuffle<T>(this List<T> list)
    {
        List<T> result = list.ToList();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (result[k], result[n]) = (result[n], result[k]);
        }

        return result;
    }
}
