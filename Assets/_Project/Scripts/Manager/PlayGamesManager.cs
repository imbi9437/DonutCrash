using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GPGSEvent = _Project.Scripts.EventStructs.GooglePlayGamesStructs;

public class PlayGamesManager : MonoSingleton<PlayGamesManager>
{
    private void Start()
    {
        PlayGamesPlatform.Activate();
        
        EventHub.Instance.RegisterEvent<GPGSEvent.RequestAuthEvent>(OnRequestAuth);
        EventHub.Instance.RegisterEvent<GPGSEvent.RequestServerSideAccess>(OnRequestAccessServerSide);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<GPGSEvent.RequestAuthEvent>(OnRequestAuth);
        EventHub.Instance?.UnRegisterEvent<GPGSEvent.RequestServerSideAccess>(OnRequestAccessServerSide);
    }


    private void OnRequestAuth(GPGSEvent.RequestAuthEvent e) => Authenticate();
    private void OnRequestAccessServerSide(GPGSEvent.RequestServerSideAccess evt) => AccessInServerSide(evt.callback);
    
    
    private void Authenticate()
    {
        Debug.Log("[PlayGamesManager] Start Authenticate");
        
        PlayGamesPlatform.Instance.Authenticate(CompleteAuthentication);
    }
    
    private void CompleteAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("<color=green>[PlayGamesManager] Authenticate Success</color>");
            
            EventHub.Instance.RaiseEvent(new GPGSEvent.AuthSuccessEvent());
        }
        else
        {
            Debug.Log($"<color=red>[PlayGamesManager] Authenticate Failed : {status}</color>");
            
            EventHub.Instance.RaiseEvent(new GPGSEvent.AuthFailedEvent());
        }
    }

    private void AccessInServerSide(Action<string> callback)
    {
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, callback);
    }
}
