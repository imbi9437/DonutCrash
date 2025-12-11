using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class FontChanger
{
    [MenuItem("Tools/FontChanger")]
    public static void ChangeFont()
    {
        var settings = Resources.Load<FontChangerSettings>("FontChangerSettings");
        TMP_FontAsset fontAsset = settings.fontAsset;
        
        var guids = AssetDatabase.FindAssets("t:Prefab", settings.filePath);

        foreach (string guid in guids)
        {
            bool isDirty = false;
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            var tmp = asset.GetComponentsInChildren<TMP_Text>();
            
            foreach (var text in tmp)
            {
                if (text.font == fontAsset) continue;
                
                text.font = fontAsset;
                isDirty = true;
            }
            
            if (isDirty) EditorUtility.SetDirty(asset);
        }
        
        AssetDatabase.SaveAssets();
    }
}


[CreateAssetMenu(fileName = "FontChangerSettings", menuName = "Tools/FontChangerSettings")]
public class FontChangerSettings : ScriptableObject
{
    public string[] filePath;
    public TMP_FontAsset fontAsset;
}