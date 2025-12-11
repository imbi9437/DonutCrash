using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLobbyPanel : BattleSubPannel
{
    public override int PanelType => (int)BattleSubPanelType.Lobby;
    
    
    // [SerializeField] private CurrentDeckInfoUI currentDeckInfoUI;

    [SerializeField] private Button leaderBoardButton;
    [SerializeField] private Button battleStartButton;
    [SerializeField] private LeaderBoardListPanel leaderBoardListPanel;
    
    [SerializeField] private Button backButton;
    
    [SerializeField] private List<DonutSlotUI> donutSlots;
    [SerializeField] private BakerSlotUI bakerSlot;

    [SerializeField] private Button saveButton;

    private void Start()
    {
        leaderBoardButton.onClick.AddListener(OnClickLeaderBoardButton);
        battleStartButton.onClick.AddListener(OnClickBattleStart);
        
        backButton.onClick.AddListener(OnBackButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        
        EventHub.Instance.RegisterEvent<PhotonStructs.StopMatchMakingEvent>(OnStopMatchMaking);
    }

    private void OnDestroy()
    {
        leaderBoardButton.onClick.RemoveListener(OnClickLeaderBoardButton);
        battleStartButton.onClick.RemoveListener(OnClickBattleStart);
        
        backButton.onClick.RemoveListener(OnBackButtonClick);
        saveButton.onClick.RemoveListener(OnSaveButtonClick);
        EventHub.Instance?.UnRegisterEvent<PhotonStructs.StopMatchMakingEvent>(OnStopMatchMaking);
    }

    private void OnStopMatchMaking(PhotonStructs.StopMatchMakingEvent evt) => StopMatchMaking();
    
    private void OnClickChangeDeck()
    {
        BattleSubPanelUIController.ChangePanel(BattleSubPanelType.Deck);
    }
    private void OnClickBattleStart()
    {
        battleStartButton.interactable = false;
        EventHub.Instance?.RaiseEvent(new PhotonStructs.StartMatchMakingEvent());
    }

    private void OnClickLeaderBoardButton()
    {
        leaderBoardListPanel.gameObject.SetActive(true);
    }

    private void StopMatchMaking()
    {
        battleStartButton.interactable = true;
    }
    
    public override void Initialize(UIController controller)
    {
        base.Initialize(controller);
        
        for (int i = 0; i < donutSlots.Count; i++)
        {
            int index = i;
            donutSlots[i].Initialize(index);
        }
        
        bakerSlot.Initialize();
    }

    private void OnBackButtonClick()
    {
        BattleSubPanelUIController.ChangePanel(BattleSubPanelType.Lobby);
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestNotSaveDeckEvent());
    }
    private void OnSaveButtonClick()
    {
        var param = new TwoButtonParam("저장", "저장하시겠습니까?", "예", "아니요");
        param.confirm = RequestSaveDeck;
        
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
    }

    private void RequestSaveDeck()
    {
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestSaveCurrentDeckEvent());
        BattleSubPanelUIController.ChangePanel(BattleSubPanelType.Lobby);
    }
}