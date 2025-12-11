using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ComponentChanger : MonoBehaviour
{
    [MenuItem("Tools/ComponentChanger")]
    public static void ChangeComponent()
    {
        Debug.Log("ComponentChanger");
        var settings = Resources.Load<ComponentChangerSettings>("ComponentChangerSettings");

        var guids = AssetDatabase.FindAssets($"t:Prefab", settings.filePath);
        MonoScript customButtonScript = FindMonoScript("CustomButton");
        Debug.Log(customButtonScript.name);
        foreach (string guid in guids)
        {
            bool isDirty = false;
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            Debug.Log(asset.name);
            var temp = asset.GetComponentsInChildren<Button>(true);
            foreach (Button i in temp)
            {
                if (i is CustomButton)
                    continue;
                
                // GameObject go = i.gameObject;
                // DestroyImmediate(i, true);
                // 참조하는 스크립트의 guid를 변경
                SerializedObject so = new SerializedObject(i);
                so.Update();
                
                SerializedProperty prop = so.FindProperty("m_Script");
                if (prop != null)
                {
                    prop.objectReferenceValue = customButtonScript;
                    so.ApplyModifiedProperties();
                    
                    isDirty = true;
                }
            }
            
            var temp2 = asset.GetComponentsInChildren<AudioSource>(true);
            foreach (AudioSource i in temp2)
            {
                DestroyImmediate(i, true);

                isDirty = true;
            }
            
            if(isDirty) EditorUtility.SetDirty(asset);
        }
        
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/ChangeSpriteInImage")]
    public static void ChangeSpriteInImage()
    {
        var settings = Resources.Load<ComponentChangerSettings>("ComponentChangerSettings");
        var guids = AssetDatabase.FindAssets($"t:Prefab", settings.filePath);
        foreach (string guid in guids)
        {
            bool isDirty = false;
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            Debug.Log(asset.name);
            var temp = asset.GetComponentsInChildren<Image>(true);
            foreach (Image i in temp)
            {
                if (i.sprite != settings.oldOne)
                    continue;
                
                i.sprite = settings.newOne;
                isDirty = true;
            }
            
            if(isDirty) EditorUtility.SetDirty(asset);
        }
        
        AssetDatabase.SaveAssets();
    }

    private static MonoScript FindMonoScript(string scriptName)
    {
        string[] guids = AssetDatabase.FindAssets($"t:MonoScript {scriptName}");
        if (guids.Length == 0)
            return null;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null && script.name == scriptName)
                return script;
        }
        
        return null;
    }
}
