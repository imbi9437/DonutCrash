using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TestManagerSpawner
{
    public static bool IsDebugMode;
    public static bool ResetUserData;
    public static bool DeleteAccount;
    
    static TestManagerSpawner()
    {
        EditorPrefs.DeleteKey("IsFirstTime");
        EditorPrefs.DeleteKey("IsResetData");
        EditorPrefs.DeleteKey("IsResetTool");
        
        IsDebugMode = EditorPrefs.GetBool("IsDebugMode", false);
        Menu.SetChecked("Tools/ManagerSpawner/ChangeDebugMode", IsDebugMode);
    }

    [MenuItem("Tools/ManagerSpawner/ChangeDebugMode")]
    public static void ChangeDebugMode()
    {
        IsDebugMode = !IsDebugMode;
        EditorPrefs.SetBool("IsDebugMode", IsDebugMode);
        Menu.SetChecked("Tools/ManagerSpawner/ChangeDebugMode", IsDebugMode);
    }

    [MenuItem("Tools/ManagerSpawner/ResetData")]
    public static void ResetData()
    {
        if (EditorApplication.isPlaying == false) return;

        string uid = DataManager.Instance.UserUid;
        EventHub.Instance.RaiseEvent(new DataStructs.RequestCreateNewDataEvent(uid));
    }

    [MenuItem("Tools/ManagerSpawner/DeleteAccount")]
    public static async void DeleteData()
    {
        if (EditorApplication.isPlaying == false) return;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestLogoutEvent());
        
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        EditorApplication.ExitPlaymode();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CheckDebugMode()
    {
        if (IsDebugMode == false) return;
        
        EventHub eventHub = Object.FindAnyObjectByType<EventHub>();
        DataManager dataManager = Object.FindAnyObjectByType<DataManager>();
        FirebaseManager firebaseManager = Object.FindAnyObjectByType<FirebaseManager>();
        
        bool hasHub = eventHub != null;
        bool hasData = dataManager != null;
        bool hasFirebase = firebaseManager != null;

        if (hasHub == false || hasData == false || hasFirebase == false)
        {
            Debug.Log("Manager 프리펩 없음");
            GameObject prefab = Resources.Load<GameObject>("Manager");
            var obj = Object.Instantiate(prefab);
            obj.name = "Manager";
        }
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseInitSuccess>(OnCompleteInit);
    }
    
    private static void OnCompleteInit(FirebaseEvents.FirebaseInitSuccess evt) => OnCompleteInit(evt.isLogin);
    private static void OnCompleteRegister(FirebaseEvents.FirebaseRegisterSuccess evt) => OnCompleteRegister(evt.uid);
    

    private static void OnCompleteInit(bool isLogin)
    {
        if (isLogin)
        {
            EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestLoadTableData());
        }
        else
        {
            EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseRegisterSuccess>(OnCompleteRegister);
            EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestCreateGuestEvent());
        }
    }

    private static void OnCompleteRegister(string uid)
    {
        EventHub.Instance.RaiseEvent(new DataStructs.RequestCreateNewDataEvent(uid));
        EventHub.Instance.UnRegisterEvent<FirebaseEvents.FirebaseRegisterSuccess>(OnCompleteRegister);
    }
}
