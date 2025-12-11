using Cysharp.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;

public partial class FirebaseManager
{
    private FirebaseDatabase Database { get; set; }

    private DatabaseReference _rootRef;
    private DatabaseReference _leaderboardRef;
    private DatabaseReference _total;
    private DatabaseReference _daily;

    private void RegisterRealtimeEvent()
    {
        EventHub.Instance.RegisterEvent<FE.RequestLeaderboardUpdate>(OnRequestLeaderboardUpdate);
        
        EventHub.Instance.RegisterEvent<FE.RequestTotalTopRanking>(OnRequestTotalTopRanking);
        EventHub.Instance.RegisterEvent<FE.RequestDailyRanking>(OnRequestDailyRanking);
        EventHub.Instance.RegisterEvent<FE.RequestTotalRanking>(OnRequestTotalRanking);
    }

    private void UnRegisterRealtimeEvent()
    {
        EventHub.Instance?.UnRegisterEvent<FE.RequestLeaderboardUpdate>(OnRequestLeaderboardUpdate);
        
        EventHub.Instance?.UnRegisterEvent<FE.RequestTotalTopRanking>(OnRequestTotalTopRanking);
        EventHub.Instance?.UnRegisterEvent<FE.RequestDailyRanking>(OnRequestDailyRanking);
        EventHub.Instance?.UnRegisterEvent<FE.RequestTotalRanking>(OnRequestTotalRanking);
    }
    
    private void CacheRealtimeField()
    {
        _rootRef = Database.RootReference;
        _leaderboardRef = _rootRef.Child("leaderboard");
        
        _total = _leaderboardRef.Child("total");
        _daily = _leaderboardRef.Child("daily");
    }

    
    private void OnRequestLeaderboardUpdate(FE.RequestLeaderboardUpdate evt) => UpdateLeaderboard(evt.deltaScore, evt.nickName).Forget();
    
    private void OnRequestTotalTopRanking(FE.RequestTotalTopRanking evt) => LoadTotalTopRanking(evt.topCount).Forget();
    private void OnRequestDailyRanking(FE.RequestDailyRanking evt) => LoadDailyRanking(evt.nickName, evt.count).Forget();
    private void OnRequestTotalRanking(FE.RequestTotalRanking evt) => LoadTotalRanking(evt.nickName, evt.count).Forget();
    

    private async UniTask UpdateLeaderboard(long deltaScore, string nickName)
    {
        if (Auth.CurrentUser == null) return;
        
        string date = CustomExtensions.ConvertTimeToKr("yyyy-MM-dd");

        try
        {
            CancellationTokenSource cts = new ();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var totalEntry = await GetTotalLeaderboard(nickName).AttachExternalCancellation(cts.Token);
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var dailyEntry = await GetDailyLeaderboard(date, nickName).AttachExternalCancellation(cts.Token);
            
            totalEntry.score += deltaScore;
            dailyEntry.score += deltaScore;
            
            await UploadTotalLeaderboard(totalEntry);
            await UploadDailyLeaderboard(date, dailyEntry);
            
            EventHub.Instance.RaiseEvent(new FE.LeaderboardUpdateSuccess());
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Realtime Database] UpdateLeaderboard Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LeaderboardUpdateFailed(e.Message));
        }
    }
    private async UniTask<LeaderboardEntry> GetDailyLeaderboard(string date, string nickName)
    {
        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");

            string uid = Auth.CurrentUser.UserId;
            
            LeaderboardEntry entry;
            var snapshot = await _daily.Child($"{date}/{uid}").GetValueAsync();

            if (snapshot.Exists) entry = JsonConvert.DeserializeObject<LeaderboardEntry>(snapshot.GetRawJsonValue());
            else entry = new LeaderboardEntry(uid, nickName);
            
            return entry;
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"<color=red>[Realtime Database] GetDailyLeaderboardEntry Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask<LeaderboardEntry> GetTotalLeaderboard(string nickname)
    {
        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");

            string uid = Auth.CurrentUser.UserId;
            
            var snapshot = await _total.Child(uid).GetValueAsync();
            
            LeaderboardEntry entry;
            
            if (snapshot.Exists) entry = JsonConvert.DeserializeObject<LeaderboardEntry>(snapshot.GetRawJsonValue());
            else entry = new LeaderboardEntry(uid, nickname);
            
            return entry;
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"<color=red>[Realtime Database] GetTotalLeaderboardEntry Failed : {e.Message}</color>");
            throw;
        }
    }

    private async UniTask UploadTotalLeaderboard(LeaderboardEntry entry)
    {
        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");
            
            string json = JsonConvert.SerializeObject(entry);
            await _total.Child(entry.uid).SetRawJsonValueAsync(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Realtime Database] UploadTotalLeaderboard Failed : {e.Message}</color>");
            throw;
        }
    }
    private async UniTask UploadDailyLeaderboard(string date, LeaderboardEntry entry)
    {
        try
        {
            if (Auth.CurrentUser == null) throw new Exception("Auth CurrentUser is null");
            
            string json = JsonConvert.SerializeObject(entry);
            await _daily.Child($"{date}/{entry.uid}").SetRawJsonValueAsync(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Realtime Database] UploadDailyLeaderboard Failed : {e.Message}</color>");
            throw;
        }
    }

    
    private async UniTask LoadTotalTopRanking(int count)
    {
        if (Auth.CurrentUser == null) return;

        try
        {
            List<LeaderboardEntry> rank = new(count);
            
            var snapshot = await _total.OrderByChild("score").LimitToLast(count).GetValueAsync();
            foreach (var child in snapshot.Children)
            {
                var entry = JsonConvert.DeserializeObject<LeaderboardEntry>(child.GetRawJsonValue());
                rank.Insert(0, entry);
            }
            
            Debug.Log("<color=green>[Realtime Database] LoadTotalTopRanking Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTotalTopRankingSuccess(rank));
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[Realtime Database] LoadTotalTopRanking Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTotalTopRankingFailed(e.Message));
        }
    }

    private async UniTask LoadDailyRanking(string nickName, int count)
    {
        if (Auth.CurrentUser == null) return;
        
        Debug.Log($"[Realtime Database] LoadDailyRanking");
        string date = CustomExtensions.ConvertTimeToKr("yyyy-MM-dd");
        try
        {
            CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var dailyEntry = await GetDailyLeaderboard(date, nickName).AttachExternalCancellation(cts.Token);
            int lower = count / 2;
            int upper = count - lower;
            List<LeaderboardEntry> rank = new();
            
            var snapshotLower = await _daily.Child(date).OrderByChild("score").EndAt(dailyEntry.score - 1).LimitToLast(lower).GetValueAsync();
            foreach (var i in snapshotLower.Children)
            {
                var entry = JsonConvert.DeserializeObject<LeaderboardEntry>(i.GetRawJsonValue());
                rank.Insert(0, entry);
            }
            
            rank.Insert(0, dailyEntry);
            
            var snapshotUpper = await _daily.Child(date).OrderByChild("score").StartAt(dailyEntry.score + 1).LimitToFirst(upper).GetValueAsync();
            foreach (var i in snapshotUpper.Children)
            {
                var entry = JsonConvert.DeserializeObject<LeaderboardEntry>(i.GetRawJsonValue());
                rank.Insert(0, entry);
            }
            
            Debug.Log("<color=green>[Realtime Database] LoadDailyRanking Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDailyRankingSuccess(rank));
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"<color=red>[Realtime Database] LoadDailyRanking Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadDailyRankingFailed(e.Message));
        }
    }

    private async UniTask LoadTotalRanking(string nickName, int count)
    {
        if (Auth.CurrentUser == null) return;
        
        Debug.Log($"[Realtime Database] LoadTotalRanking");
        
        try
        {
            CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var dailyEntry = await GetTotalLeaderboard(nickName).AttachExternalCancellation(cts.Token);
            
            int lower = count / 2;
            int upper = count - lower;
            List<LeaderboardEntry> rank = new();
            
            var snapshotLower = await _total.OrderByChild("score").EndAt(dailyEntry.score - 1).LimitToLast(lower).GetValueAsync();
            foreach (var i in snapshotLower.Children)
            {
                var entry = JsonConvert.DeserializeObject<LeaderboardEntry>(i.GetRawJsonValue());
                rank.Insert(0, entry);
            }
            
            rank.Insert(0, dailyEntry);
            
            var snapshotUpper = await _total.OrderByChild("score").StartAt(dailyEntry.score + 1).LimitToFirst(upper).GetValueAsync();
            foreach (var i in snapshotUpper.Children)
            {
                var entry = JsonConvert.DeserializeObject<LeaderboardEntry>(i.GetRawJsonValue());
                rank.Insert(0, entry);
            }
            
            Debug.Log("<color=green>[Realtime Database] LoadTotalRanking Success</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTotalRankingSuccess(rank));
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"<color=red>[Realtime Database] LoadTotalRanking Failed : {e.Message}</color>");
            EventHub.Instance.RaiseEvent(new FE.LoadTotalRankingFailed(e.Message));
        }
    }
}
