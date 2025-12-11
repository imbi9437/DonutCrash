using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using GPE = _Project.Scripts.EventStructs.GooglePlayGamesStructs;

public partial class FirebaseManager
{
    private FirebaseAuth Auth { get; set; }
    
    public bool IsAnonymous => Auth.CurrentUser?.IsAnonymous ?? false;

    private void RegisterFirebaseAuthEvent()
    {
        //================ Auth SignIn ================//
        EventHub.Instance.RegisterEvent<FE.RequestSignInWithEmailEvent>(OnRequestSignInWithEmailEvent);
        EventHub.Instance.RegisterEvent<FE.RequestSignInWithGpgsEvent>(OnRequestSignInWithGpgsEvent);
        
        //=============== Auth SignUp ================//
        EventHub.Instance?.RegisterEvent<FE.RequestCreateGuestEvent>(OnRequestCreateGuestEvent);
        EventHub.Instance?.RegisterEvent<FE.RequestCreateEmailEvent>(OnRequestCreateEmailEvent);
        
        //=============== Auth SignOut ================//
        EventHub.Instance?.RegisterEvent<FE.RequestLogoutEvent>(OnRequestLogoutEvent);
        
        //=============== Auth SwitchCredential ================//
        EventHub.Instance?.RegisterEvent<FE.RequestSwitchCredentialEvent>(OnRequestSwitchCredentialEvent);
        
    }
    private void UnRegisterFirebaseAuthEvent()
    {
        //================ Auth SignIn ================//
        EventHub.Instance?.UnRegisterEvent<FE.RequestSignInWithEmailEvent>(OnRequestSignInWithEmailEvent);
        EventHub.Instance?.UnRegisterEvent<FE.RequestSignInWithGpgsEvent>(OnRequestSignInWithGpgsEvent);
        
        //=============== Auth SignUp ================//
        EventHub.Instance?.UnRegisterEvent<FE.RequestCreateGuestEvent>(OnRequestCreateGuestEvent);
        EventHub.Instance?.UnRegisterEvent<FE.RequestCreateEmailEvent>(OnRequestCreateEmailEvent);

        //=============== Auth SignOut ================//
        EventHub.Instance?.UnRegisterEvent<FE.RequestLogoutEvent>(OnRequestLogoutEvent);
        
        //=============== Auth SwitchCredential ================//
        EventHub.Instance?.UnRegisterEvent<FE.RequestSwitchCredentialEvent>(OnRequestSwitchCredentialEvent);
    }

    
    #region Event Rapper Methods

    private void OnRequestSignInWithEmailEvent(FE.RequestSignInWithEmailEvent evt) => SignInWithEmailAndPasswordAsync(evt.email, evt.password).Forget();
    private void OnRequestSignInWithGpgsEvent(FE.RequestSignInWithGpgsEvent evt) => SignInWithGpgsSequence();
    private void OnRequestCreateGuestEvent(FE.RequestCreateGuestEvent evt) => CreateAccountWithAnonymousAsync().Forget();
    
    private void OnRequestCreateEmailEvent(FE.RequestCreateEmailEvent evt) => CreateAccountWithEmailAndPasswordAsync(evt.email, evt.password, evt.confirmPassword).Forget();
    
    
    private void OnRequestLogoutEvent(FE.RequestLogoutEvent evt) => SignOut();
    
    private void OnRequestSwitchCredentialEvent(FE.RequestSwitchCredentialEvent evt) => SwitchGpgsCredentialSequence();
    
    #endregion
    
    #region SignIn Methods
    
    /// <summary> 자체 Email, Password 로그인 </summary>
    private async UniTask SignInWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, password);
            
            Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase SignInWithEmailAndPasswordAsync Success</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseLoginSuccess(result.User.UserId));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[{nameof(FirebaseManager)}] Firebase SignInWithEmailAndPasswordAsync Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseLoginFailed(e.Message));
        }
    }

    /// <summary> Google Play Games 로그인 </summary>
    private void SignInWithGpgsSequence()
    {
        Debug.Log($"<color=yellow>[{nameof(FirebaseManager)}] SignInWithGpgsSequence Start</color>");
        
        EventHub.Instance.RegisterEvent<GPE.AuthSuccessEvent>(CompleteGpgsAuth);
        EventHub.Instance.RaiseEvent(new GPE.RequestAuthEvent());

        void CompleteGpgsAuth(GPE.AuthSuccessEvent evt)
        {
            Action<string> callback = token=> SignInWithGpgsAsync(token).Forget();
            EventHub.Instance.RaiseEvent(new GPE.RequestServerSideAccess(callback));
            EventHub.Instance.UnRegisterEvent<GPE.AuthSuccessEvent>(CompleteGpgsAuth);
        }
    }
    private async UniTask SignInWithGpgsAsync(string token)
    {
        try
        {
            var credential = PlayGamesAuthProvider.GetCredential(token);
            var result = await Auth.SignInWithCredentialAsync(credential);
            
            Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase SignInWithGpgsAsync Success</color>");
            
            var diff = (long)result.Metadata.LastSignInTimestamp - (long)result.Metadata.CreationTimestamp;
            
            if (0 <= diff && diff <= 1000)
                EventHub.Instance.RaiseEvent(new FE.FirebaseRegisterSuccess(result.UserId));
            else
                EventHub.Instance.RaiseEvent(new FE.FirebaseLoginSuccess(result.UserId));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[{nameof(FirebaseManager)}] Firebase SignInWithGpgsAsync Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseLoginFailed(e.Message));
        }
    }
    
    
    #endregion
    
    #region Create Account Methods
    
    private async UniTask CreateAccountWithAnonymousAsync()
    {
        try
        {
            var result = await Auth.SignInAnonymouslyAsync();
            
            Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase SignInWithAnonymousAsync Success</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseRegisterSuccess(result.User.UserId));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[{nameof(FirebaseManager)}] Firebase SignInWithAnonymousAsync Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseRegisterFailed(e.Message));
        }
    }

    private async UniTask CreateAccountWithEmailAndPasswordAsync(string email, string password, string confirmPassword)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        
            Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase CreateAccountWithEmailAndPasswordAsync Success</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseRegisterSuccess(result.User.UserId));
        }
        catch (Exception e)
        {
            Debug.LogError($"[{nameof(FirebaseManager)}] Firebase CreateAccountWithEmailAndPasswordAsync Failed : {e.Message})]");
            EventHub.Instance.RaiseEvent(new FE.FirebaseRegisterFailed(e.Message));       
        }
    }
    
    #endregion
    
    #region SignOut Methods

    private void SignOut()
    {
        Auth.SignOut();
        
        EventHub.Instance.RaiseEvent(new FE.FirebaseLogoutEvent());
    }
    
    #endregion
    
    #region Switch Crediential Methods

    private void SwitchCredential(string providerId)
    {
        if (providerId == PlayGamesAuthProvider.ProviderId) SwitchGpgsCredentialSequence();
    }
    
    private void SwitchGpgsCredentialSequence()
    {
        if (Auth.CurrentUser == null || Auth.CurrentUser.IsAnonymous == false) return;
        
        EventHub.Instance.RegisterEvent<GPE.AuthSuccessEvent>(AuthCallback);
        EventHub.Instance.RaiseEvent(new GPE.RequestAuthEvent());

        void AuthCallback(GPE.AuthSuccessEvent evt)
        {
            Action<string> callback = token => SwitchCredentialToGpgs(token).Forget();
            EventHub.Instance.UnRegisterEvent<GPE.AuthSuccessEvent>(AuthCallback);
            EventHub.Instance.RaiseEvent(new GPE.RequestServerSideAccess(callback));
        }
    }
    
    private async UniTask SwitchCredentialToGpgs(string token)
    {
        if (Auth.CurrentUser == null || Auth.CurrentUser.IsAnonymous == false) return;
        
        Debug.Log($"<color=yellow>[{nameof(FirebaseManager)}] SwitchCredentialToGpgs Start</color>");
        
        try
        {
            var credential = PlayGamesAuthProvider.GetCredential(token);
            await Auth.CurrentUser.LinkWithCredentialAsync(credential);

            var profile = new UserProfile() { DisplayName = Social.localUser.userName };
            await Auth.CurrentUser.UpdateUserProfileAsync(profile);
            
            Debug.Log($"<color=green>[{nameof(FirebaseManager)}] Firebase SwitchCredentialToGpgs Success</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseSwitchCredentialSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[{nameof(FirebaseManager)}] Firebase SwitchCredentialToGpgs Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.FirebaseSwitchCredentialFailed(e.Message));
        }
    }
    
    #endregion
}
