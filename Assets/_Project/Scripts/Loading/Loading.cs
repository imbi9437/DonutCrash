using _Project.Scripts.EventStructs;
using TMPro;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] private float loadingTime = 3f;
    [SerializeField] private ProgressBarController progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    private float _curLoadingTime = 0f;
    private float _lerp;
    
    private void Start()
    {
        EventHub.Instance.RegisterEvent<ChangeSceneStructs.UpdateLoadSceneEvent>(UpdateProgress);
        
        EventHub.Instance.RaiseEvent(new ChangeSceneStructs.RequestChangeTargetSceneEvent(loadingTime));
    }

    private void Update()
    {
        _curLoadingTime += Time.deltaTime;
    }


    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<ChangeSceneStructs.UpdateLoadSceneEvent>(UpdateProgress);
    }

    private void UpdateProgress(ChangeSceneStructs.UpdateLoadSceneEvent evt) => FillSlider(evt.progress);
    

    private void FillSlider(float progress)
    {
        _lerp = Mathf.InverseLerp(0, loadingTime, _curLoadingTime);
        
        float value = Mathf.Min(progress, _lerp);
        
        progressBar.Value = value;
        loadingText.text = $"{(value * 100f):F0} %";
    }
}
