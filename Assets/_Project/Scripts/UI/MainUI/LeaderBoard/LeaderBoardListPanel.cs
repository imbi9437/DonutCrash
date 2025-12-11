using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FE = _Project.Scripts.EventStructs.FirebaseEvents;

public class LeaderBoardListPanel : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    
    [Space(20)]
    [Header("탭")]
    [SerializeField] private Button dailyButton;
    [SerializeField] private Button totalButton;
    [SerializeField] private Button totalTopButton;
    
    [SerializeField] private Sprite deselected;
    [SerializeField] private Sprite selected;
    
    [Space(20)]
    [Header("콘텐츠")]
    [SerializeField] private TextMeshProUGUI info;
    [SerializeField] private RectTransform content;

    [SerializeField] private LeaderBoardUIEntry entryPrefab;

    private void OnEnable()
    {
        EventHub.Instance.RegisterEvent<FE.LoadDailyRankingSuccess>(OnLoadDailyRanking);
        EventHub.Instance.RegisterEvent<FE.LoadTotalRankingSuccess>(OnLoadTotalRanking);
        EventHub.Instance.RegisterEvent<FE.LoadTotalTopRankingSuccess>(OnLoadTotalTopRanking);
        
        EventHub.Instance.RegisterEvent<FE.RequestDailyRanking>(OnRequestDailyRanking);
        EventHub.Instance.RegisterEvent<FE.RequestTotalRanking>(OnRequestTotalRanking);
        EventHub.Instance.RegisterEvent<FE.RequestTotalTopRanking>(OnRequestTotalTopRanking);
        
        EventHub.Instance.RegisterEvent<FE.LoadDailyRankingFailed>(OnLoadDailyRankingFailed);
        EventHub.Instance.RegisterEvent<FE.LoadTotalRankingFailed>(OnLoadTotalRankingFailed);
        EventHub.Instance.RegisterEvent<FE.LoadTotalTopRankingFailed>(OnLoadTotalTopRankingFailed);
        
        closeButton.onClick.RemoveAllListeners();
        
        dailyButton.onClick.RemoveAllListeners();
        totalButton.onClick.RemoveAllListeners();
        totalTopButton.onClick.RemoveAllListeners();
        
        closeButton.onClick.AddListener(OnClickCloseButton);
        
        dailyButton.onClick.AddListener(OnClickDailyBtn);
        totalButton.onClick.AddListener(OnClickTotalBtn);
        totalTopButton.onClick.AddListener(OnClickTotalTopBtn);
        
        OnClickDailyBtn();
    }

    private void OnDisable()
    {
        EventHub.Instance?.UnRegisterEvent<FE.LoadDailyRankingSuccess>(OnLoadDailyRanking);
        EventHub.Instance?.UnRegisterEvent<FE.LoadTotalRankingSuccess>(OnLoadTotalRanking);
        EventHub.Instance?.UnRegisterEvent<FE.LoadTotalTopRankingSuccess>(OnLoadTotalTopRanking);
        
        EventHub.Instance?.UnRegisterEvent<FE.RequestDailyRanking>(OnRequestDailyRanking);
        EventHub.Instance?.UnRegisterEvent<FE.RequestTotalRanking>(OnRequestTotalRanking);
        EventHub.Instance?.UnRegisterEvent<FE.RequestTotalTopRanking>(OnRequestTotalTopRanking);
        
        EventHub.Instance?.UnRegisterEvent<FE.LoadDailyRankingFailed>(OnLoadDailyRankingFailed);
        EventHub.Instance?.UnRegisterEvent<FE.LoadTotalRankingFailed>(OnLoadTotalRankingFailed);
        EventHub.Instance?.UnRegisterEvent<FE.LoadTotalTopRankingFailed>(OnLoadTotalTopRankingFailed);
        
        closeButton.onClick.RemoveAllListeners();
        
        dailyButton.onClick.RemoveAllListeners();
        totalButton.onClick.RemoveAllListeners();
        totalTopButton.onClick.RemoveAllListeners();
    }

    private void OnLoadDailyRanking(FE.LoadDailyRankingSuccess evt) => InitializeContent(0, evt.ranking);
    private void OnLoadTotalRanking(FE.LoadTotalRankingSuccess evt) => InitializeContent(1, evt.ranking);
    private void OnLoadTotalTopRanking(FE.LoadTotalTopRankingSuccess evt) => InitializeContent(2, evt.ranking);

    private void OnRequestDailyRanking(FE.RequestDailyRanking evt) => InitializeContent(-1, null);
    private void OnRequestTotalRanking(FE.RequestTotalRanking evt) => InitializeContent(-1, null);
    private void OnRequestTotalTopRanking(FE.RequestTotalTopRanking evt) => InitializeContent(-1, null);
    
    private void OnLoadDailyRankingFailed(FE.LoadDailyRankingFailed evt) => InitializeContent(-2, null);
    private void OnLoadTotalRankingFailed(FE.LoadTotalRankingFailed evt) => InitializeContent(-2, null);
    private void OnLoadTotalTopRankingFailed(FE.LoadTotalTopRankingFailed evt) => InitializeContent(-2, null);

    private void InitializeContent(int infoIndex, List<LeaderboardEntry> rank)
    {
        info.text = infoIndex switch
        {
            -2 => "통신 오류",
            -1 => "통신 중",
            0 => "24시간 동안의 MMR 변화량을 비교합니다. (매일 자정 초기화)",
            1 => "지금까지의 누적 MMR을 비교합니다.",
            2 => "서버 내 1위 부터 100위 까지 전체 랭킹을 표시합니다.",
            _ => ""
        };
        
        foreach (LeaderBoardUIEntry i in content.GetComponentsInChildren<LeaderBoardUIEntry>())
        {
            Destroy(i.gameObject);
        }

        rank?.ForEach(x =>
        {
            LeaderBoardUIEntry lbue = Instantiate(entryPrefab, content);
            lbue.Setup(x);
        });
    }
    
    private void OnClickDailyBtn()
    {
        EventHub.Instance.RaiseEvent(new FE.RequestDailyRanking(DataManager.Instance.UserNickName, 10));
        dailyButton.image.sprite = selected;
        totalButton.image.sprite = deselected;
        totalTopButton.image.sprite = deselected;
    }

    private void OnClickTotalBtn()
    {
        EventHub.Instance.RaiseEvent(new FE.RequestTotalRanking(DataManager.Instance.UserNickName, 10));
        dailyButton.image.sprite = deselected;
        totalButton.image.sprite = selected;
        totalTopButton.image.sprite = deselected;
    }

    private void OnClickTotalTopBtn()
    {
        EventHub.Instance.RaiseEvent(new FE.RequestTotalTopRanking(100));
        dailyButton.image.sprite = deselected;
        totalButton.image.sprite = deselected;
        totalTopButton.image.sprite = selected;
    }

    private void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
