using _Project.Scripts.EventStructs;
using _Project.Scripts.Manager.Firebase;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using DS = _Project.Scripts.EventStructs.DataStructs;

public partial class FirebaseManager : MonoSingleton<FirebaseManager>
{
    private FirebaseApp App { get; set; }
    private FirebaseValidator Validator { get; set; }


    #region Unity Message Events

    protected override void Awake()
    {
        base.Awake();
        Validator = new FirebaseValidator();
        
        RegisterFirebaseEvent();        
        RegisterFirebaseAuthEvent();    //Firebase Auth 구독 이벤트 함수
        RegisterFirestoreEvent();       //Firestore 구독 이벤트 함수
        RegisterRealtimeEvent();
    }

    private void Start() => InitializeFirebase().Forget();

    private void OnDestroy()
    {
        UnRegisterFirebaseEvent();
        UnRegisterFirebaseAuthEvent();  //Firebase Auth 구독 해지 이벤트 함수
        UnRegisterFirestoreEvent();     //Firestore 구독 해지 이벤트 함수
        UnRegisterRealtimeEvent();
    }

    #endregion Unity Message Events
    
    #region Event Rapper Methods

    private void OnLoginSuccess(FE.FirebaseLoginSuccess evt) => CacheFirebaseField(evt.uid);
    private void OnRegisterSuccess(FE.FirebaseRegisterSuccess evt) => CacheFirebaseField(evt.uid);

    #endregion Event Rapper Methods
    
    
    private void RegisterFirebaseEvent()
    {
        EventHub.Instance.RegisterEvent<FE.FirebaseLoginSuccess>(OnLoginSuccess);
        EventHub.Instance.RegisterEvent<FE.FirebaseRegisterSuccess>(OnRegisterSuccess);
    }
    private void UnRegisterFirebaseEvent()
    {
        EventHub.Instance?.UnRegisterEvent<FE.FirebaseLoginSuccess>(OnLoginSuccess);
        EventHub.Instance?.UnRegisterEvent<FE.FirebaseRegisterSuccess>(OnRegisterSuccess);
    }
    
    
    /// <summary> Firebase 초기화 함수 </summary>
    private async UniTask InitializeFirebase()
    {
        try
        {
            Debug.Log($"<color=yellow>[{nameof(FirebaseManager)}] Firebase Initialize Start</color>");
            
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (status == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;
                Auth = FirebaseAuth.DefaultInstance;
                Database = FirebaseDatabase.DefaultInstance;
                Firestore = FirebaseFirestore.DefaultInstance;

                _rootRef = Database.RootReference;
                
                Database.SetPersistenceEnabled(false);
                _rootRef.KeepSynced(true);
                
                bool isLogin = Auth.CurrentUser != null;
                
                Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase Initialize Success</color>");
                
                EventHub.Instance?.RaiseEvent(new FE.FirebaseInitSuccess(isLogin));
                if (isLogin) EventHub.Instance?.RaiseEvent(new FE.FirebaseLoginSuccess(Auth.CurrentUser.UserId));

                if (isLogin)
                {
                    foreach (var info in Auth.CurrentUser.ProviderData)
                    {
                        #if UNITY_ANDROID && !UNITY_EDITOR
                        if (info.ProviderId == PlayGamesAuthProvider.ProviderId)
                            EventHub.Instance?.RaiseEvent(new GooglePlayGamesStructs.RequestAuthEvent());
                        #endif
                    }
                }
            }
            else
            {
                Debug.Log($"<color=yellow>[{nameof(FirebaseManager)}] Firebase Initialize Failed : {status.ToString()}</color>");
                EventHub.Instance?.RaiseEvent(new FE.FirebaseInitFailed(status.ToString()));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[{nameof(FirebaseManager)}] Firebase Initialize Failed : {e.Message}</color>");
            EventHub.Instance?.RaiseEvent(new FE.FirebaseInitFailed(e.Message));
        }
    }
    
    /// <summary> Firebase 필드 캐싱 함수 </summary>
    private void CacheFirebaseField(string uid)
    {
        CacheFirestoreField(uid);
        CacheRealtimeField();
        EventHub.Instance.RaiseEvent(new FE.CompleteFirebaseCache());
    }
}
