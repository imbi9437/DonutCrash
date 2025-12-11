using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using DE = _Project.Scripts.EventStructs.DataStructs;

public class Title : MonoBehaviour
{
    private Action _onCompleteLoadTableCallback;
    
    private void Start()
    {
        EventHub.Instance.RegisterEvent<FE.CompleteFirebaseCache>(OnCompleteCache);
        
        EventHub.Instance.RegisterEvent<FE.FirebaseRegisterSuccess>(SetLoadTableCallback);
        EventHub.Instance.RegisterEvent<FE.FirebaseLoginSuccess>(SetLoadTableCallback);
        
        EventHub.Instance.RegisterEvent<FE.LoadTableSuccess>(OnCompleteLoadTable);
        
        EventHub.Instance.RegisterEvent<DataStructs.CompleteLoadDataEvent>(OnCompleteLoadData);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<FE.CompleteFirebaseCache>(OnCompleteCache);
        
        EventHub.Instance?.UnRegisterEvent<FE.FirebaseRegisterSuccess>(SetLoadTableCallback);
        EventHub.Instance?.UnRegisterEvent<FE.FirebaseLoginSuccess>(SetLoadTableCallback);
        
        EventHub.Instance?.UnRegisterEvent<FE.LoadTableSuccess>(OnCompleteLoadTable);
        
        EventHub.Instance?.UnRegisterEvent<DataStructs.CompleteLoadDataEvent>(OnCompleteLoadData);
    }
    
    private void OnCompleteCache(FE.CompleteFirebaseCache evt) => RequestLoadTable();
    private void OnCompleteLoadTable(FE.LoadTableSuccess evt) => _onCompleteLoadTableCallback?.Invoke();
    
    private void SetLoadTableCallback(FE.FirebaseRegisterSuccess evt) => _onCompleteLoadTableCallback = RequestCreateUserData;
    private void SetLoadTableCallback(FE.FirebaseLoginSuccess evt) => _onCompleteLoadTableCallback = RequestLoadUserData;
    
    private void OnCompleteLoadData(DataStructs.CompleteLoadDataEvent evt) => CheckChangeScene();
    
    
    private void RequestLoadTable()
    {
        EventHub.Instance.RaiseEvent(new FE.RequestLoadTableData());
    }
    private void RequestLoadUserData()
    {
        EventHub.Instance.RaiseEvent(new FE.RequestLoadUserData());
        EventHub.Instance.RaiseEvent(new FE.RequestLoadDeckData());
        EventHub.Instance.RaiseEvent(new FE.RequestLoadMatchmakingData());
    }
    private void RequestCreateUserData()
    {
        EventHub.Instance.RaiseEvent(new DE.RequestCreateNewDataEvent());
    }


    private void CheckChangeScene()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.UserNickName)) return;
        
        EventHub.Instance.RaiseEvent(new ChangeSceneStructs.RequestChangeSceneEvent("02.Main",-1,true));
    }
}