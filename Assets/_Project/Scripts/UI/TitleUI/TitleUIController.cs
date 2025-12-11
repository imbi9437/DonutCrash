using _Project.Scripts.EventStructs;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TitlePanelType
{
    SignIn,
    SignUp,
    SetNickname,
}

public class TitleUIController : UIController
{
    private void Start()
    {
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseInitSuccess>(OnCompleteFirebaseInit);
        
        EventHub.Instance.RegisterEvent<DataStructs.CompleteCreateNewDataEvent>(OnCompleteCreateData);
        EventHub.Instance.RegisterEvent<DataStructs.CompleteLoadDataEvent>(OnCompleteLoadData);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<FirebaseEvents.FirebaseInitSuccess>(OnCompleteFirebaseInit);
        
        EventHub.Instance?.UnRegisterEvent<DataStructs.CompleteCreateNewDataEvent>(OnCompleteCreateData);
        
        EventHub.Instance?.UnRegisterEvent<DataStructs.CompleteLoadDataEvent>(OnCompleteLoadData);
    }
    
    #region Event Rapper Methods

    private void OnCompleteFirebaseInit(FirebaseEvents.FirebaseInitSuccess evt) => CheckAuth(evt.isLogin);
    
    private void OnCompleteCreateData(DataStructs.CompleteCreateNewDataEvent evt) => ChangePanel(TitlePanelType.SetNickname);

    private void OnCompleteLoadData(DataStructs.CompleteLoadDataEvent evt) => CheckNicknamePanelOpen();
    
    #endregion
    
    
    private void OpenPanel(TitlePanelType type) => OpenPanel((int)type);
    private void ClosePanel(TitlePanelType type) => ClosePanel((int)type);
    
    public void ChangePanel(TitlePanelType type)
    {
        if (currentPanel.PanelType == (int)type) return;
        
        ClosePanel(currentPanel.PanelType);
        OpenPanel(type);
    }

    private void CheckAuth(bool isLogin)
    {
        if (isLogin) return;
        
        OpenPanel(TitlePanelType.SignIn);
    }

    private void CheckNicknamePanelOpen()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.UserNickName) == false) return;
        
        OpenPanel(TitlePanelType.SetNickname);
    }
    
    
    protected override IEnumerable GetChildPanel()
    {
        return GetComponentsInChildren<TitlePopup>(true);
    }
}
