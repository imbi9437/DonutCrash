using _Project.Scripts.EventStructs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using DE = _Project.Scripts.EventStructs.DataStructs;

public partial class DataManager
{
    private void RegisterSetterEvent()
    {
        //UserData
        EventHub.Instance?.RegisterEvent<DE.RequestSetNickNameEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetProfileImageEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetEnergyEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetGoldEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetCashEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetRecipePiecesEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetPerfectRecipeEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetDonutListEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetBakerListEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetIngredientEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetUnlockRecipeEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetTutorialEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetPurchaseInfoEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetTrayEvent>(OnRequestDataSetEvent);
        
        //DeckData
        EventHub.Instance?.RegisterEvent<DE.RequestSetDeckDonutEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetDeckBakerEvent>(OnRequestDataSetEvent);
        
        //MatchMakingData
        EventHub.Instance?.RegisterEvent<DE.RequestSetMMREvent>(OnRequestDataSetEvent);
        EventHub.Instance?.RegisterEvent<DE.RequestSetOnlineStateEvent>(OnRequestDataSetEvent);
    }
    private void UnRegisterSetterEvent()
    {
        //UserData
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetNickNameEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetProfileImageEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetEnergyEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetGoldEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetCashEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetRecipePiecesEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetPerfectRecipeEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetDonutListEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetBakerListEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetIngredientEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetUnlockRecipeEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetTutorialEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetPurchaseInfoEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetTrayEvent>(OnRequestDataSetEvent);
        
        //DeckData
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetDeckDonutEvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetDeckBakerEvent>(OnRequestDataSetEvent);
        
        //MatchMakingData
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetMMREvent>(OnRequestDataSetEvent);
        EventHub.Instance?.UnRegisterEvent<DE.RequestSetOnlineStateEvent>(OnRequestDataSetEvent);
    }
    
    #region Event Rapper Functions

    //UserData Rapper
    private void OnRequestDataSetEvent(DE.RequestSetNickNameEvent evt) => SetUserNickName(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetProfileImageEvent evt) => SetUserProfileImage(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetEnergyEvent evt) => SetUserEnergy(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetGoldEvent evt) => SetUserGold(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetCashEvent evt) => SetUserCash(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetRecipePiecesEvent evt) => SetRecipePieces(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetPerfectRecipeEvent evt) => SetPerfectRecipes(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetDonutListEvent evt) => SetUserDonut(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetBakerListEvent evt) => SetUserBaker(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetIngredientEvent evt) => SetIngredient(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetUnlockRecipeEvent evt) => SetUnlockRecipe(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetTutorialEvent evt) => SetTutorialIndex(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetPurchaseInfoEvent evt) => SetPurchaseInfo(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetTrayEvent evt) => SetTray(evt.newValue);
    
    //DeckData Rapper
    private void OnRequestDataSetEvent(DE.RequestSetDeckDonutEvent evt) => SetDeckDonut(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetDeckBakerEvent evt) => SetDeckBaker(evt.newValue);
    
    //MatchMakingData Rapper
    private void OnRequestDataSetEvent(DE.RequestSetMMREvent evt) => SetMMR(evt.newValue);
    private void OnRequestDataSetEvent(DE.RequestSetOnlineStateEvent evt) => SetOnlineState(evt.newValue);
    
    #endregion
    
    #region UserData Setter

    private void SetUserNickName(string nickName)
    {
        bool isDirty = localUserData.nickname != nickName;
        
        localUserData.nickname = nickName;
        EventHub.Instance.RaiseEvent(new FE.RequestSaveUserData(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserNicknameEvent());
    }

    private void SetUserProfileImage(string profileBaker)
    {
        bool isDirty = localUserData.profileBaker != profileBaker;

        localUserData.profileBaker = profileBaker;
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserProfileImageEvent());
    }
    
    private void SetUserEnergy(int changeValue)
    {
        bool isDirty = localUserData.energy != changeValue;
        
        localUserData.energy = changeValue;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserEnergyEvent());
    }

    private void SetUserGold(int changeValue)
    {
        bool isDirty = localUserData.gold != changeValue;
        
        localUserData.gold = changeValue;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserGoldEvent());
    }

    private void SetUserCash(int changeValue)
    {
        bool isDirty = localUserData.cash != changeValue;
        
        localUserData.cash = changeValue;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserCashEvent());
    }

    private void SetRecipePieces(int changeValue)
    {
        bool isDirty = localUserData.recipePieces != changeValue;
        
        localUserData.recipePieces = changeValue;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserRecipePiecesEvent());
    }

    private void SetPerfectRecipes(int changeValue)
    {
        bool isDirty = localUserData.perfectRecipes != changeValue;
        
        localUserData.perfectRecipes = changeValue;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        if (isDirty) EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserPerfectRecipeEvent());
    }

    private void SetUserDonut(List<DonutInstanceData> donutList)
    {
        localUserData.donuts = donutList.ToList();
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserDonutEvent());
    }

    private void SetUserBaker(List<BakerInstanceData> bakerList)
    {
        localUserData.bakers = bakerList.ToList();
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserBakerEvent());
    }

    private void SetIngredient(Dictionary<string, int> ingredient)
    {
        localUserData.ingredients = ingredient.ToDictionary(x => x.Key, x => x.Value);
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserIngredientEvent());
    }

    private void SetUnlockRecipe(List<string> recipeList)
    {
        localUserData.unlockedRecipes = recipeList.ToList();
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserUnlockRecipeEvent());
    }

    private void SetTutorialIndex(int value)
    {
        localUserData.tutorialIndex = value;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserTutorialEvent());
    }

    private void SetPurchaseInfo(Dictionary<string, int> purchaseInfo)
    {
        localUserData.purchaseInfo = purchaseInfo.ToDictionary(x => x.Key, x => x.Value);
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserPurchaseInfoEvent());
    }

    private void SetTray(TrayData data)
    {
        localUserData.trayData = TrayData.CopyTo(data);
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localUserData));
        
        EventHub.Instance.RaiseEvent(new DE.BroadcastSetUserTrayEvent());
    }
    
    #endregion
    
    #region DeckData Setter

    private void SetDeckDonut(List<DonutInstanceData> donutList)
    {
        localDeckData.waitingDonuts = donutList.ToList();
        EventHub.Instance.RaiseEvent(new FE.RequestSaveDeckData(localDeckData));
    }
    private void SetDeckBaker(BakerInstanceData baker)
    {
        localDeckData.baker = BakerInstanceData.CopyTo(baker);
        EventHub.Instance.RaiseEvent(new FE.RequestSaveDeckData(localDeckData));
    }
    
    #endregion

    #region MatchMakingData Setter

    private void SetMMR(int value)
    {
        localMatchData.mmr = value;
        EventHub.Instance.RaiseEvent(new FE.RequestSaveMatchmakingData(localMatchData));
    }
    private void SetOnlineState(bool isOnline)
    {
        localMatchData.isOnline = isOnline;
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestDataUpload(localMatchData));
    }

    #endregion
}
