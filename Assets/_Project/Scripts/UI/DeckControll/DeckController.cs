using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DE = _Project.Scripts.EventStructs.DataStructs;
using OGS = _Project.Scripts.EventStructs.OutGameStructs;

public class DeckController : MonoBehaviour
{
    private DeckData userDeck;
    private List<DonutInstanceData> userDonuts;
    private List<BakerInstanceData> userBakers;

    private bool deckDonutDirty;
    private bool deckBakerDirty;
    
    private bool userDonutDirty;
    private bool userBakerDirty;

    #region Unity Message Events
    
    private void Start()
    {
        InitData();
        
        EventHub.Instance?.RegisterEvent<DE.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance?.RegisterEvent<DE.BroadcastSetUserBakerEvent>(OnBroadcast);
        
        EventHub.Instance?.RegisterEvent<OGS.RequestChangeDeckDonutEvent>(OnRequestChange);
        EventHub.Instance?.RegisterEvent<OGS.RequestChangeDeckBakerEvent>(OnRequestChange);
        EventHub.Instance?.RegisterEvent<OGS.RequestSaveCurrentDeckEvent>(OnRequestSave);
        EventHub.Instance?.RegisterEvent<OGS.RequestNotSaveDeckEvent>(OnRequestResetDeck);
    }
    
    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DE.BroadcastSetUserDonutEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DE.BroadcastSetUserBakerEvent>(OnBroadcast);
        
        EventHub.Instance?.UnRegisterEvent<OGS.RequestChangeDeckDonutEvent>(OnRequestChange);
        EventHub.Instance?.UnRegisterEvent<OGS.RequestChangeDeckBakerEvent>(OnRequestChange);
        EventHub.Instance?.UnRegisterEvent<OGS.RequestSaveCurrentDeckEvent>(OnRequestSave);
        EventHub.Instance?.UnRegisterEvent<OGS.RequestNotSaveDeckEvent>(OnRequestResetDeck);
    }

    #endregion


    #region Event Rapper Methods

    private void OnBroadcast(DE.BroadcastSetUserDonutEvent evt) => SetDonutData();
    private void OnBroadcast(DE.BroadcastSetUserBakerEvent evt) => SetBakerData();
    
    
    private void OnRequestChange(OGS.RequestChangeDeckDonutEvent evt) => SetDeckDonut(evt.index, evt.uid);
    private void OnRequestChange(OGS.RequestChangeDeckBakerEvent evt) => SetDeckBaker(evt.uid);
    private void OnRequestSave(OGS.RequestSaveCurrentDeckEvent evt) => SaveDeckData();
    
    #endregion
    
    private void SetDeckData() => userDeck = DataManager.Instance.DeckData;
    private void SetDonutData() => userDonuts = DataManager.Instance.Donuts;
    private void SetBakerData() => userBakers = DataManager.Instance.Bakers;

    /// <summary> 데이터 초기화 </summary>
    private void InitData()
    {
        SetDeckData();
        SetDonutData();
        SetBakerData();
    }

    /// <summary> 덱 도넛 설정 </summary>
    private void SetDeckDonut(int index, string uid)
    {
        if (index >= 5) return;

        if (string.IsNullOrEmpty(uid)) RemoveDeckDonut(index);
        else ChangeDeckDonut(index, uid);
    }

    /// <summary> 덱 제빵사 설정 </summary>
    private void SetDeckBaker(string uid)
    {
        if (string.IsNullOrEmpty(uid)) RemoveDeckBaker();
        else ChangeDeckBaker(uid);
    }
    
    /// <summary> 덱 도넛 변경 </summary>
    private void ChangeDeckDonut(int index, string uid)
    {
        Debug.Log($"ChangeDeckDonut index = {index}, uid = {uid}");
        bool canChange = CanChangeDonut(uid);
        if (canChange == false) return;
        
        string prevUid = userDeck.waitingDonuts[index]?.uid;
        if (string.IsNullOrEmpty(prevUid) == false)
        {
            var prevData = userDonuts.Find(d => d.uid == prevUid);
            prevData.isLock = false;
        }
        
        var newData = userDonuts.Find(d => d.uid == uid);
        newData.isLock = true;
        
        userDeck.waitingDonuts[index] = DonutInstanceData.CopyTo(newData);
        
        userDonutDirty = true;
        deckDonutDirty = true;
        
        EventHub.Instance.RaiseEvent(new OGS.ChangeDeckDonutSuccessEvent(index, uid));
    }
    
    /// <summary> 덱 도넛 제거</summary>
    private void RemoveDeckDonut(int index)
    {
        if (userDeck.waitingDonuts[index] != null)
        {
            var prevData = userDonuts.Find(d => d.uid == userDeck.waitingDonuts[index].uid);
            if (prevData != null)
                prevData.isLock = false;
            userDonutDirty = true;
        }
        
        userDeck.waitingDonuts[index] = new DonutInstanceData();
        
        deckDonutDirty = true;
        
        EventHub.Instance.RaiseEvent(new OGS.ChangeDeckDonutSuccessEvent(index, ""));
    }

    /// <summary> 덱 제빵사 변경 </summary>
    private void ChangeDeckBaker(string uid)
    {
        bool canChange = CanChangeBaker(uid);
        if (canChange == false) return;

        var selectBaker = userBakers.First(b => b.uid == uid);
        userDeck.baker = BakerInstanceData.CopyTo(selectBaker);
        deckBakerDirty = true;
    }
    /// <summary> 덱 제빵사 제거 </summary>
    private void RemoveDeckBaker()
    {
        userDeck.baker = new BakerInstanceData();
        deckBakerDirty = true;
    }

    /// <summary> 덱 저장 </summary>
    private void SaveDeckData()
    {
        if (deckDonutDirty) EventHub.Instance.RaiseEvent(new DE.RequestSetDeckDonutEvent(userDeck.waitingDonuts));
        if (deckBakerDirty) EventHub.Instance.RaiseEvent(new DE.RequestSetDeckBakerEvent(userDeck.baker));
        
        if (userDonutDirty) EventHub.Instance.RaiseEvent(new DE.RequestSetDonutListEvent(userDonuts));
        if (userBakerDirty) EventHub.Instance.RaiseEvent(new DE.RequestSetBakerListEvent(userBakers));
        
        ResetDirty();
    }

    /// <summary> 해당 도넛으로 변경 가능한지 체크 </summary>
    private bool CanChangeDonut(string uid)
    {
        if (userDonuts.Exists(d => d.uid == uid) == false) return false;

        foreach (var data in userDeck.waitingDonuts)
        {
            if (data == null) continue;
            if (data.uid == uid)
            {
                var param = new OneButtonParam("알림", "이미 설정된 도넛입니다.", "돌아간다");
                EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
                return false;
            }
        }
        
        return true;
    }

    /// <summary> 해당 제빵사로 변경 가능한지 체크 </summary>
    private bool CanChangeBaker(string uid)
    {
        if (userBakers.Exists(b => b.uid == uid) == false) return false;
        if (DataManager.Instance.DeckData.baker != null && userDeck.baker.uid == uid) return false;

        return true;
    }

    /// <summary> 변경 기록 초기화 </summary>
    private void ResetDirty()
    {
        deckDonutDirty = false;
        deckBakerDirty = false;
        
        userBakerDirty = false;
        userDonutDirty = false;
    }
    /// <summary> 덱을 저장하지 않고 뒤로갔을때 초기화</summary>
    private void OnRequestResetDeck(OGS.RequestNotSaveDeckEvent evt)
    {
        ResetDirty();
        InitData(); //덱초기화
    }
}