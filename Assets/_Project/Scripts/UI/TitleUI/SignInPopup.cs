using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SignInPopup : TitlePopup
{
    public override int PanelType => (int)TitlePanelType.SignIn;
    
    [SerializeField] private Button guestButton;
    [SerializeField] private Button playGamesButton;

    #region Unity Message Events
    
    private void Start()
    {
        guestButton.onClick.AddListener(OnGuestButtonClick);
        playGamesButton.onClick.AddListener(OnSignInButtonClick);
        
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseLoginFailed>(OnFailedGuestSignIn);
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseRegisterFailed>(OnFailedGuestRegister);
    }

    private void OnDestroy()
    {
        guestButton.onClick.RemoveListener(OnGuestButtonClick);
        playGamesButton.onClick.RemoveListener(OnSignInButtonClick);
        
        EventHub.Instance?.UnRegisterEvent<FirebaseEvents.FirebaseLoginFailed>(OnFailedGuestSignIn);
        EventHub.Instance?.UnRegisterEvent<FirebaseEvents.FirebaseRegisterFailed>(OnFailedGuestRegister);
    }
    
    #endregion

    private void OnFailedGuestSignIn(FirebaseEvents.FirebaseLoginFailed evt) => OnFailedSignIn();
    private void OnFailedGuestRegister(FirebaseEvents.FirebaseRegisterFailed evt) => OnFailedRegister();
    
    
    #region Button Events Methods
    
    private void OnGuestButtonClick()
    {
        guestButton.interactable = false;
        playGamesButton.interactable = false;
        
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestCreateGuestEvent());
    }
    private void OnSignInButtonClick()
    {
        guestButton.interactable = false;
        playGamesButton.interactable = false;
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestSignInWithGpgsEvent());
        #elif UNITY_EDITOR
        var param = new OneButtonParam("<color=red>경고</color>", "해당 버튼을 통한 로그인 혹은 회원가입의 경우 안드로이드에서만 동작합니다.");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup,param));
        #endif
    }

    private void OnFailedSignIn()
    {
        guestButton.interactable = true;
        playGamesButton.interactable = true;

        var param = new OneButtonParam("로그인 실패", "로그인에 실패하셨습니다.", "확인");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup,param));
    }

    private void OnFailedRegister()
    {
        guestButton.interactable = true;
        playGamesButton.interactable = true;
        
        var param = new OneButtonParam("로그인 실패", "로그인에 실패하셨습니다.", "확인");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup,param));
    }
    
    #endregion
}
