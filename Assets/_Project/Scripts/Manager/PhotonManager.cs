using _Project.Scripts.Extensions;
using _Project.Scripts.InGame;
using _Project.Scripts.Manager;
using Cysharp.Threading.Tasks;
using DonutClash.UI.GlobalUI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using PhotonTable = ExitGames.Client.Photon.Hashtable;

using US = _Project.Scripts.EventStructs.UIStructs;
using DS = _Project.Scripts.EventStructs.DataStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;
using EVT = _Project.Scripts.EventStructs.ChangeSceneStructs;
using IGS = _Project.Scripts.EventStructs.InGameStructs;
using FE = _Project.Scripts.EventStructs.FirebaseEvents;

public partial class PhotonManager : PunMonoSingleton<PhotonManager>
{
    private bool _isWaiting = false;
    
    #region Unity Event Method

    private void Start()
    {
        EventHub.Instance?.RegisterEvent<DS.CompleteLoadDataEvent>(OnCompleteInitializeData);
        EventHub.Instance?.RegisterEvent<PS.StartMatchMakingEvent>(OnStartMatchMaking);
        EventHub.Instance?.RegisterEvent<PS.StopMatchMakingEvent>(OnStopMatchMaking);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<DS.CompleteLoadDataEvent>(OnCompleteInitializeData);
        EventHub.Instance?.UnRegisterEvent<PS.StartMatchMakingEvent>(OnStartMatchMaking);
        EventHub.Instance?.UnRegisterEvent<PS.StopMatchMakingEvent>(OnStopMatchMaking);

        Disconnect();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        EventHub.Instance.RaiseEvent(new PS.OnChangeApplicationFocus(hasFocus));
    }

    #endregion Unity Event Method

    #region Event Wrapper Methods

    // pre-game and in-game flow
    private void OnCompleteInitializeData(DS.CompleteLoadDataEvent evt) => CompleteInitializeData();
    private void OnStartMatchMaking(PS.StartMatchMakingEvent evt) => StartMatchMaking();
    private void OnStopMatchMaking(PS.StopMatchMakingEvent evt) => StopMatchMaking(evt.withLeaveRoom);
    private void OnSceneLoaded(EVT.CompleteLoadSceneEvent evt) => SceneLoaded(evt.sceneIndex);
    private void RequestChangeBattleState(PS.ChangeBattleState evt) => ChangeBattleState(evt.state, evt.turnOwner);

    // RPC events for in-game logic
    private void RequestSelectDonutSpawned(PS.RequestSelectDonutSpawned evt) => SelectDonutSpawned(evt.selector, evt.donuts);
    private void RequestCompleteSelectDonut(PS.CompleteSelectDonut evt) => CompleteSelectDonut();
    private void RequestSpawnDonut(PS.RequestSpawnDonut evt) => SpawnDonut(evt.selector, evt.donut);
    private void RequestShotDonut(PS.ShotDonut evt) => ShotDonut(evt.uid, evt.force);

    // RPC events for in-game UI
    private void RequestSetUserProfile(PS.RequestSetUserProfile evt) => SetUserProfile();
    private void RequestChangeDonutList(PS.RequestChangeDonutList evt) => ChangeDonutList(evt.owner, evt.stagedDonuts, evt.unstagedDonuts);
    private void RequestChangeDeathCount(PS.RequestChangeDeathCount evt) => ChangeDeathCount(evt.owner, evt.deathCount);
    private void RequestChangeTurnTimer(PS.RequestChangeTurnTimer evt) => ChangeTurnTimer(evt.max, evt.current);
    private void RequestDonutCollisionEvent(PS.RequestDonutCollisionEvent evt) => DonutCollisionEvent(evt.point, evt.damage, evt.collider, evt.collidee, evt.type, evt.spdMod, evt.isCrit);
    
    // post-game
    private void OnGameEnd(PS.GameEndEvent evt) => GameEnd();
    private void ExceptionOpponentDisconnected(PS.OnOpponentDisconnected evt) => OpponentDisconnected();
    private void ExceptionUnexpectedDisconnect(PS.LeaveRoomEvent evt) => UnexpectedDisconnect();
    private void OnLeaveRoomWithGameEnd(PS.LeaveRoomEvent evt) => LeaveRoomWithGameEnd();
    private void RequestUnRegisterRoomEvent(PS.UnRegisterRoomEvent evt) => UnRegisterRoomEvent();
    private void RequestSendReward(PS.RequestReward evt) => SendReward(evt.winner, evt.deltaGold);
    
    // exception events for out focusing error
    private void OnChangeFocusInMain(PS.OnChangeApplicationFocus evt) => CheckIsPhotonConnected(evt.hasFocus);
    private void OnChangeFocusInGame(PS.OnChangeApplicationFocus evt) => CheckIsInRoom(evt.hasFocus);

    #endregion Event Wrapper Methods

    #region PhotonManager Methods

    private void Disconnect() => PhotonNetwork.Disconnect();

    private void CompleteInitializeData()
    {
        PhotonNetwork.SerializationRate = 20;
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = "replace nickname";

        PhotonTable cp = PhotonNetwork.LocalPlayer.CustomProperties;
        cp.TryAdd("uid", "replace uid");
        PhotonNetwork.LocalPlayer.SetCustomProperties(cp);

        PhotonNetwork.ConnectUsingSettings();

        Debug.Log($"포톤 서버 접속 시도");
    }

    private void StartMatchMaking()
    {
        if (CanStartMatchMaking(out GlobalPanelParam param))
        {
            _isWaiting = true;
            
            PhotonNetwork.LocalPlayer.NickName = DataManager.Instance?.UserNickName ?? "someone";
            RoomOptions roomOptions = new() { MaxPlayers = 2 };
            InvokeWhenReadyAsync(() => PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions)).Forget();
        }
    }

    private bool CanStartMatchMaking(out GlobalPanelParam param)
    {
        param = null;

        // 사용자의 현재 덱의 유효성 검사
        DeckData deck = DataManager.Instance.DeckData;
        List<DonutInstanceData> donuts = DataManager.Instance.Donuts;
        if (PhotonNetwork.IsConnectedAndReady == false || PhotonNetwork.InLobby == false)
            param = new OneButtonParam("경고", "게임 서버에 접속상태가 아닙니다.\n재시도 해주세요", "확인", StopMatchMaking);
        else if (deck == null)
            param = new OneButtonParam("경고", "덱 데이터가 없습니다.", "확인", StopMatchMaking);
        else if (deck.waitingDonuts == null || deck.waitingDonuts.Count == 0)
            param = new OneButtonParam("경고", "도넛이 배치되지 않았습니다.", "확인", StopMatchMaking);
        else if (deck.waitingDonuts.Count < 5)
            param = new OneButtonParam("경고", "덱의 도넛이 부족합니다.", "확인", StopMatchMaking);
        else if (deck.waitingDonuts.Any(i => i?.uid?.IsNullOrEmpty() ?? false))
            param = new OneButtonParam("경고", "유효하지 않는 도넛이 존재합니다.", "확인", StopMatchMaking);
        else if (deck.waitingDonuts.Any(i => !DataManager.Instance.TryGetDonutData(i?.origin ?? "", out DonutData _)))
            param = new OneButtonParam("경고", "존재하지 않는 도넛을 갖고있습니다.", "확인", StopMatchMaking);
        else if (deck.waitingDonuts.Any(i => donuts.Find(x => x.uid == i.uid) == null))
            param = new OneButtonParam("경고", "보유하지 않은 도넛이 배치되어 있습니다.", "확인", StopMatchMaking);
        else if (deck.baker == null || deck.baker.uid.IsNullOrEmpty())
            param = new OneButtonParam("경고", "마녀를 배치하지 않았습니다.", "확인", StopMatchMaking);
        else if (_isWaiting)
            param = new OneButtonParam("경고", "이미 통신중 입니다.", "확인");

        bool isAvailable = param == null;
        param ??= new OneButtonParam("매칭 중", "매칭을 취소하려면 버튼을 누르세요", "매칭 취소", OnCancelMatchMaking);
        
        EventHub.Instance?.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));

        return isAvailable;
    }
    
    private void OnCancelMatchMaking()
    {
        if (_isWaiting)
        {
            OneButtonParam oneButtonParam = new("매칭 중", "매칭을 취소하려면 버튼을 누르세요", "매칭 취소", OnCancelMatchMaking);
            EventHub.Instance?.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, oneButtonParam));
            
            OneButtonParam param = new("경고", "이미 통신중 입니다.", "확인");
            EventHub.Instance?.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
            
            return;
        }
        EventHub.Instance.RaiseEvent(new PS.StopMatchMakingEvent(true));
    }

    private void StopMatchMaking()
    {
        EventHub.Instance?.RaiseEvent(new PS.StopMatchMakingEvent(false));
    }

    private void StopMatchMaking(bool withLeaveRoom)
    {
        if (withLeaveRoom)
        {
            EventHub.Instance?.UnRegisterEvent<PS.LeaveRoomEvent>(ExceptionUnexpectedDisconnect);
            PhotonNetwork.LeaveRoom();
        }
    }

    private void SceneLoaded(int sceneIndex)
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonTable cp = PhotonNetwork.LocalPlayer.CustomProperties;
        cp["scene"] = sceneIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(cp);
    }

    private void UnRegisterRoomEvent()
    {
        // 정상적인 게임 종료 완료. 예외처리를 위한 이벤트 제거
        EventHub.Instance?.UnRegisterEvent<PS.OnOpponentDisconnected>(ExceptionOpponentDisconnected);
        EventHub.Instance?.UnRegisterEvent<PS.LeaveRoomEvent>(ExceptionUnexpectedDisconnect);

        EventHub.Instance?.RegisterEvent<PS.LeaveRoomEvent>(OnLeaveRoomWithGameEnd);
    }

    private void GameEnd()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void ExceptionGameEnd()
    {
        // 예외 감지
        EventHub.Instance?.UnRegisterEvent<PS.OnOpponentDisconnected>(ExceptionOpponentDisconnected);
        EventHub.Instance?.UnRegisterEvent<PS.LeaveRoomEvent>(ExceptionUnexpectedDisconnect);

        // TODO : 아래의 방을 떠나는 이벤트를 콜백으로 실행하도록 변경. 글로벌 유아이 팝업 활요하면 될듯
        // 예외처리로 인한 예기치 못한 게임 종료
        if (PhotonNetwork.InRoom)
        {
            EventHub.Instance?.RegisterEvent<PS.LeaveRoomEvent>(OnLeaveRoomWithGameEnd);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            LeaveRoomWithGameEnd();
        }
        
    }

    private void LeaveRoomWithGameEnd()
    {
        EventHub.Instance?.UnRegisterEvent<PS.LeaveRoomEvent>(OnLeaveRoomWithGameEnd);
        EventHub.Instance?.RaiseEvent(new EVT.RequestChangeSceneEvent(index: (int)SceneType.Main));
    }

    private void OpponentDisconnected()
    {
        Debug.Log($"상대편 탈주, 승리");

        GiveBakerExpReward(3);
        GiveGoldReward(500);
        EventHub.Instance.RaiseEvent(new FE.RequestLeaderboardUpdate(50, DataManager.Instance.UserNickName));
        
        ExceptionGameEnd();
    }

    private void UnexpectedDisconnect()
    {
        Debug.Log($"예기치 못한 연결 종료, 패배 처리");
        ExceptionGameEnd();
    }

    private void SetUserProfile()
    {
        if (PhotonNetwork.InRoom == false)
            return;
        
        PhotonTable rcp = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonNetwork.CurrentRoom.Players.ForEach(x =>
        {
            InGameOwner owner = x.Value.IsMasterClient ? InGameOwner.Left : InGameOwner.Right;
            DeckDataDto ddd = null;
            rcp.TryGetValue(owner.ToString(), out object value);
            if (value != null)
            {
                try
                {
                    ddd = value.ToString().DeSerializeObject<DeckDataDto>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"<color=red>[Photon Manager] JsonDeserialize Error {e.Message} </color>");
                }
            }
            EventHub.Instance?.RaiseEvent(new IGS.RequestSetUserProfile(x.Value.IsMasterClient ? InGameOwner.Left : InGameOwner.Right, x.Value.NickName, ddd?.baker?.origin));
        });
    }

    private void CheckIsPhotonConnected(bool hasFocus)
    {
        if (!PhotonNetwork.IsConnected)
        {

            OneButtonParam parm = new OneButtonParam("연결 불량", "게임 서버와 연결이 중단되었습니다.", "확인");
            EventHub.Instance.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, parm));
            PhotonNetwork.ConnectUsingSettings();
            EventHub.Instance.RaiseEvent(new EVT.RequestChangeSceneEvent(null, (int)SceneType.Main, true));
        }
    }

    private void CheckIsInRoom(bool hasFocus)
    {
        // 안드로이드를 위한 예외, 게임 중 비활성화시 포톤 룸 떠나기
        // 안드로이드 환경에서 백그라운드로 앱을 넘기게 되면 프로세스가 멈추게 됨으로 해당 예외 처리 추가
        #if UNITY_ANDROID
        
        if (!hasFocus && PhotonNetwork.InRoom)
        {
            StopMatchMaking();
            EventHub.Instance.RaiseEvent(new US.RequestCloseAllGlobalPanel());
            OneButtonParam parm = new OneButtonParam("연결 불량", "게임 서버와 연결이 중단되었습니다.", "확인");
            EventHub.Instance.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, parm));
            PhotonNetwork.LeaveRoom();
            return;
        }
        
        #endif
        
        if (!PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
            
            OneButtonParam parm = new OneButtonParam("연결 불량", "게임 서버와 연결이 중단되었습니다.", "확인");
            EventHub.Instance.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, parm));
            
            EventHub.Instance.RaiseEvent(new EVT.RequestChangeSceneEvent(null, (int)SceneType.Main, true));
        }
    }

    private async UniTask CheckPlayerLoad(int targetIndex)
    {
        CancellationTokenSource cts = new();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(10));

        try
        {
            while (cts.Token.IsCancellationRequested == false)
            {
                await UniTask.Delay(1000, cancellationToken: cts.Token);
                bool isReady = true;
                Player[] players = PhotonNetwork.CurrentRoom.Players.Values.ToArray();
                foreach (Player i in players)
                {
                    PhotonTable cp = i.CustomProperties;
                    cp.TryGetValue("scene", out object sceneIndex);
                    if (sceneIndex == null)
                        continue;
                    Debug.Log((int)sceneIndex);
                    isReady &= targetIndex == (int)sceneIndex;
                }

                if (isReady)
                    break;
            }

            Debug.Log("모든 플레이어 로딩 완료");
            PhotonTable rcp = PhotonNetwork.CurrentRoom.CustomProperties;
            DeckDataDto dto = rcp[nameof(InGameOwner.Left)].ToString().DeSerializeObject<DeckDataDto>();
            DeckData deck = new DeckData();
            deck.SetData(dto);
            deck.waitingDonuts.ForEach(x => x.CalcDonutStatus());
            EventHub.Instance.RaiseEvent(new IGS.SetDeckData(InGameOwner.Left, deck));
            dto = rcp[nameof(InGameOwner.Right)].ToString().DeSerializeObject<DeckDataDto>();
            deck = new DeckData();
            deck.SetData(dto);
            deck.waitingDonuts.ForEach(x => x.CalcDonutStatus());
            EventHub.Instance.RaiseEvent(new IGS.SetDeckData(InGameOwner.Right, deck));

            Debug.Log($"{rcp[$"{nameof(InGameOwner.Left)}"]}");
            Debug.Log($"{rcp[$"{nameof(InGameOwner.Right)}"]}");
            EventHub.Instance?.RaiseEvent(new PS.CompleteWaitingPlayer());
        }
        catch
        {
            Debug.Log("플레이어 연결 실패");
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
            EventHub.Instance?.RaiseEvent(new EVT.RequestChangeSceneEvent(index: (int)SceneType.Main, isDirect: true));
        }
    }
    
    private async UniTask InvokeWhenReadyAsync(Action callback)
    {
        CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(5));
        
        callback?.Invoke();
        while (cts.Token.IsCancellationRequested == false)
        {
            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.JoinedLobby:
                case ClientState.Authenticated:
                case ClientState.ConnectedToGameServer:
                case ClientState.Joined:
                case ClientState.Disconnected:
                case ClientState.ConnectedToMasterServer:
                case ClientState.ConnectedToNameServer:
                case ClientState.ConnectWithFallbackProtocol:
                    cts.Cancel();
                    break;
            }
            await UniTask.Yield();
        }
        _isWaiting = false;
    }

    private void GiveGoldReward(int deltaGold)
    {
        int gold = DataManager.Instance.UserGold;
        EventHub.Instance?.RaiseEvent(new DS.RequestSetGoldEvent(gold + deltaGold));
    }

    private void GiveBakerExpReward(int deltaExp)
    {
        List<BakerInstanceData> bakers = DataManager.Instance.Bakers;
        BakerInstanceData fieldBaker = DataManager.Instance.DeckData.baker;
        BakerInstanceData bakerData = bakers.Find(x => x.uid == fieldBaker.uid);
        bakerData.GetExp(deltaExp);
            
        EventHub.Instance?.RaiseEvent(new DS.RequestSetBakerListEvent(bakers));
    }

    private void RegisterInGameEvents()
    {
        EventHub.Instance?.RegisterEvent<EVT.CompleteLoadSceneEvent>(OnSceneLoaded);

        EventHub.Instance?.RegisterEvent<PS.ChangeBattleState>(RequestChangeBattleState);
        EventHub.Instance?.RegisterEvent<PS.RequestReward>(RequestSendReward);
        EventHub.Instance?.RegisterEvent<PS.UnRegisterRoomEvent>(RequestUnRegisterRoomEvent);
        EventHub.Instance?.RegisterEvent<PS.GameEndEvent>(OnGameEnd);
        // Request each client input
        EventHub.Instance?.RegisterEvent<PS.ShotDonut>(RequestShotDonut);
        EventHub.Instance?.RegisterEvent<PS.RequestSelectDonutSpawned>(RequestSelectDonutSpawned);
        EventHub.Instance?.RegisterEvent<PS.RequestSpawnDonut>(RequestSpawnDonut);

        // Event for UI synchronize
        EventHub.Instance?.RegisterEvent<PS.RequestChangeDeathCount>(RequestChangeDeathCount);
        EventHub.Instance?.RegisterEvent<PS.RequestChangeDonutList>(RequestChangeDonutList);
        EventHub.Instance?.RegisterEvent<PS.RequestChangeTurnTimer>(RequestChangeTurnTimer);
        EventHub.Instance?.RegisterEvent<PS.CompleteSelectDonut>(RequestCompleteSelectDonut);
        EventHub.Instance?.RegisterEvent<PS.RequestSetUserProfile>(RequestSetUserProfile);

        // Event for FX
        EventHub.Instance?.RegisterEvent<PS.RequestDonutCollisionEvent>(RequestDonutCollisionEvent);

        // Event for exception
        EventHub.Instance?.RegisterEvent<PS.OnOpponentDisconnected>(ExceptionOpponentDisconnected);
        EventHub.Instance?.RegisterEvent<PS.LeaveRoomEvent>(ExceptionUnexpectedDisconnect);
        
        EventHub.Instance?.UnRegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInMain);
        EventHub.Instance?.RegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInGame);
    }

    private void UnRegisterInGameEvents()
    {
        EventHub.Instance?.UnRegisterEvent<EVT.CompleteLoadSceneEvent>(OnSceneLoaded);

        EventHub.Instance?.UnRegisterEvent<PS.ChangeBattleState>(RequestChangeBattleState);
        EventHub.Instance?.UnRegisterEvent<PS.RequestReward>(RequestSendReward);
        EventHub.Instance?.UnRegisterEvent<PS.UnRegisterRoomEvent>(RequestUnRegisterRoomEvent);
        EventHub.Instance?.UnRegisterEvent<PS.GameEndEvent>(OnGameEnd);
        // Request each client input
        EventHub.Instance?.UnRegisterEvent<PS.ShotDonut>(RequestShotDonut);
        EventHub.Instance?.UnRegisterEvent<PS.RequestSelectDonutSpawned>(RequestSelectDonutSpawned);
        EventHub.Instance?.UnRegisterEvent<PS.RequestSpawnDonut>(RequestSpawnDonut);

        // Event for UI synchronize
        EventHub.Instance?.UnRegisterEvent<PS.RequestChangeDeathCount>(RequestChangeDeathCount);
        EventHub.Instance?.UnRegisterEvent<PS.RequestChangeDonutList>(RequestChangeDonutList);
        EventHub.Instance?.UnRegisterEvent<PS.RequestChangeTurnTimer>(RequestChangeTurnTimer);
        EventHub.Instance?.UnRegisterEvent<PS.CompleteSelectDonut>(RequestCompleteSelectDonut);
        EventHub.Instance?.UnRegisterEvent<PS.RequestSetUserProfile>(RequestSetUserProfile);

        // Event for FX
        EventHub.Instance?.UnRegisterEvent<PS.RequestDonutCollisionEvent>(RequestDonutCollisionEvent);

        // Event for exception
        EventHub.Instance?.UnRegisterEvent<PS.OnOpponentDisconnected>(ExceptionOpponentDisconnected);
        EventHub.Instance?.UnRegisterEvent<PS.LeaveRoomEvent>(ExceptionUnexpectedDisconnect);
        
        EventHub.Instance?.UnRegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInGame);
        EventHub.Instance?.RegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInMain);
    }

    #endregion PhotonManager Methods

    #region Photon Static Methods

    public static GameObject PunInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject go = PhotonNetwork.Instantiate(prefab.name, position, rotation);
        return go;
    }

    public static void PunDestroy(GameObject gameObject)
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public static bool CheckStartable()
    {
        bool valid = PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2;
        if (valid == false)
            EventHub.Instance?.RaiseEvent(new EVT.RequestChangeSceneEvent(index: (int)SceneType.Main));
        return valid;
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log($"마스터 서버에 연결 완료");

        EventHub.Instance?.UnRegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInGame);
        EventHub.Instance?.RegisterEvent<PS.OnChangeApplicationFocus>(OnChangeFocusInMain);
        
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"로비에 접속 완료");

        EventHub.Instance?.RaiseEvent(new PS.JoinedLobbyEvent());
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"방 생성");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 참가 : {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonTable cp = PhotonNetwork.LocalPlayer.CustomProperties;
        cp.TryAdd("scene", SceneController.GetSceneIndex());
        PhotonNetwork.LocalPlayer.SetCustomProperties(cp);

        PhotonTable rcp = PhotonNetwork.CurrentRoom.CustomProperties;
        string owner = (PhotonNetwork.IsMasterClient ? InGameOwner.Left : InGameOwner.Right).ToString();
        DeckDataDto dto = DeckDataDto.CurrentDto(DataManager.Instance.DeckData);
        string json = dto.SerializeObject();
        rcp.TryAdd(owner, json);
        PhotonNetwork.CurrentRoom.SetCustomProperties(rcp);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            EventHub.Instance?.RaiseEvent(new US.RequestCloseAllGlobalPanel());
        }

        RegisterInGameEvents();
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"방 탈출");
        EventHub.Instance?.RaiseEvent(new PS.LeaveRoomEvent());

        UnRegisterInGameEvents();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            EventHub.Instance?.RaiseEvent(new US.RequestCloseAllGlobalPanel());
            if (PhotonNetwork.IsMasterClient == false)
                return;
            photonView.RPC("SceneChangeRPC", RpcTarget.All, (int)SceneType.Stage);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        EventHub.Instance?.RaiseEvent(new PS.OnOpponentDisconnected());
    }

    #endregion Photon Callbacks
}