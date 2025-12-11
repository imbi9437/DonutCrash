using _Project.Scripts.Manager;
using Cysharp.Threading.Tasks;
using DonutClash.UI.GlobalUI;
using UnityEngine;
using UnityEngine.SceneManagement;

using EVT = _Project.Scripts.EventStructs.ChangeSceneStructs;
using US = _Project.Scripts.EventStructs.UIStructs;

public class SceneController : MonoSingleton<SceneController>
{
    private const string LoadingSceneName = "03.Loading";
    
    private const float DefaultDelayTime = 1f;
    
    private string _targetSceneName;
    private int _targetSceneIndex;
    private bool _isSceneLoading;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        EventHub.Instance.RegisterEvent<EVT.RequestChangeSceneEvent>(RequestChangeScene);
        EventHub.Instance.RegisterEvent<EVT.RequestChangeTargetSceneEvent>(RequestChangeToTarget);
        EventHub.Instance.RegisterEvent<EVT.CompleteLoadSceneEvent>(OnCompleteLoadScene);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<EVT.RequestChangeSceneEvent>(RequestChangeScene);
        EventHub.Instance?.UnRegisterEvent<EVT.RequestChangeTargetSceneEvent>(RequestChangeToTarget);
        EventHub.Instance?.UnRegisterEvent<EVT.CompleteLoadSceneEvent>(OnCompleteLoadScene);
    }


    #region Event Rapper Functions

    private void RequestChangeScene(EVT.RequestChangeSceneEvent evt) => StartChange(evt.sceneName, evt.index, evt.isDirect, evt.delay);
    private void RequestChangeToTarget(EVT.RequestChangeTargetSceneEvent evt) => StartChangeToTarget(evt.delay);
    private void OnCompleteLoadScene(EVT.CompleteLoadSceneEvent evt) => CompleteLoadScene(evt.sceneIndex);
    
    #endregion

    public static string GetSceneName() => SceneManager.GetActiveScene().name;
    public static int GetSceneIndex() => SceneManager.GetActiveScene().buildIndex;
    public static SceneType GetSceneType() => (SceneType)SceneManager.GetActiveScene().buildIndex;
    public static string GetTargetSceneName() => Instance._targetSceneName;
    public static int GetTargetSceneIndex() => Instance._targetSceneIndex;
    
    private void CompleteLoadScene(int sceneIndex)
    {
        if ((SceneType)sceneIndex == SceneType.Loading)
            return;
        
        _isSceneLoading = false;
        _targetSceneName = "";
        _targetSceneIndex = -1;
    }

    private void StartChange(string sceneName, int sceneIndex, bool isDirect, float delay)
    {
        if (_isSceneLoading)
        {
            Debug.LogWarning($"<color=yellow>[{nameof(SceneController)}] 이미 씬 전환중에 있습니다.</color>");
            // OneButtonParam param = new("오류", "이미 씬 전환중에 있습니다.");
            // EventHub.Instance?.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
            return;
        }
        
        if (CheckSceneValidation(sceneName, sceneIndex) == false)
            return;

        if (GetSceneIndex() == _targetSceneIndex)
        {
            Debug.LogWarning($"동일한 씬으로 이동하려고 합니다.");
            return;
        }
        
        _isSceneLoading = true;
        
        if (isDirect == false)
        {
            ChangeScene(LoadingSceneName, delay);
        }
        else
        {
            if (string.IsNullOrEmpty(sceneName)) ChangeScene(_targetSceneIndex, delay);
            else if (sceneIndex < 0) ChangeScene(_targetSceneName, delay);
        }
    }
    private void StartChangeToTarget(float delay)
    {
        if (string.IsNullOrEmpty(_targetSceneName) && _targetSceneIndex < 0)
        {
            Debug.LogError("이동할 씬의 이름 및 인덱스가 존재하지 않습니다.");
            return;
        }
        
        if (string.IsNullOrEmpty(_targetSceneName)) ChangeScene(_targetSceneIndex,delay);
        else if (_targetSceneIndex < 0) ChangeScene(_targetSceneName, delay);
    }
    

    private void ChangeScene(string sceneName, float delay = 0f)
    {
        if (delay <= 0f)
            LoadSceneAsyncWithOutDelay(sceneName).Forget();
        else LoadSceneAsync(sceneName, delay).Forget();
    }
    private void ChangeScene(int sceneIndex, float delay = 0f)
    {
        if (delay <= 0f)
            LoadSceneAsyncWithOutDelay(sceneIndex).Forget();
        else LoadSceneAsync(sceneIndex, delay).Forget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventHub.Instance.RaiseEvent(new EVT.CompleteLoadSceneEvent(scene.buildIndex));
    }

    private bool CheckSceneValidation(string sceneName, int sceneIndex)
    {
        if (string.IsNullOrEmpty(sceneName) && sceneIndex < 0)
        {
            Debug.LogError($"<color=red>[{nameof(SceneController)}] 이동할 씬의 이름 및 인덱스가 존재하지 않습니다.</color>");
            OneButtonParam param = new("오류", "이동할 씬의 이름 및 인덱스가 존재하지 않습니다.");
            EventHub.Instance?.RaiseEvent(new US.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
            return false;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneIndex))
        {
            _targetSceneIndex = sceneIndex;
            _targetSceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
        }
        else if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            _targetSceneName = sceneName;
            _targetSceneIndex = SceneManager.GetSceneByName(sceneName).buildIndex;
        }
        else
        {
            Debug.LogError($"존재하지 않는 씬으로 이동하려 합니다. {sceneName}, {sceneIndex}");
            EventHub.Instance?.RaiseEvent(new EVT.FailLoadSceneEvent($"존재하지 않는 씬으로 이동하려 합니다. {sceneName}, {sceneIndex}"));
            return false;
        }
        return true;
    }
    
    
    private async UniTask LoadSceneAsyncWithOutDelay(string sceneName)
    {
        EventHub.Instance.RaiseEvent(new EVT.StartLoadSceneEvent());
        
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.WaitUntilLoaded())
        {
            await UniTask.Yield(destroyCancellationToken);
            
            EventHub.Instance.RaiseEvent(new EVT.UpdateLoadSceneEvent(op.progress));
        }
        
        op.allowSceneActivation = true;
    }
    private async UniTask LoadSceneAsyncWithOutDelay(int sceneIndex)
    {
        EventHub.Instance.RaiseEvent(new EVT.StartLoadSceneEvent());
        
        var op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;
        
        while (op.WaitUntilLoaded())
        {
            await UniTask.Yield(destroyCancellationToken);
            
            EventHub.Instance.RaiseEvent(new EVT.UpdateLoadSceneEvent(op.progress));
        }
        
        op.allowSceneActivation = true;
    }
    

    private async UniTask LoadSceneAsync(string sceneName, float delay)
    {
        EventHub.Instance.RaiseEvent(new EVT.StartLoadSceneEvent());
        
        float t = Mathf.Max(delay, DefaultDelayTime);

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        while (op.WaitUntilLoaded() || elapsed < t)
        {
            await UniTask.Yield(destroyCancellationToken);
            
            EventHub.Instance.RaiseEvent(new EVT.UpdateLoadSceneEvent(op.progress));
            elapsed += Time.deltaTime;
        }
        
        op.allowSceneActivation = true;
    }
    private async UniTask LoadSceneAsync(int sceneIndex, float delay)
    {
        EventHub.Instance.RaiseEvent(new EVT.StartLoadSceneEvent());
        
        float t = Mathf.Max(delay, DefaultDelayTime);

        var op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        while (op.WaitUntilLoaded() || elapsed < t)
        {
            await UniTask.Yield(destroyCancellationToken);
            
            EventHub.Instance.RaiseEvent(new EVT.UpdateLoadSceneEvent(op.progress));
            elapsed += Time.deltaTime;
        }
        
        op.allowSceneActivation = true;
    }
}
