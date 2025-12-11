using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class TurnPopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup hostCanvasGroup;
    [SerializeField] private CanvasGroup guestCanvasGorup;

    [SerializeField] private TextMeshProUGUI hostTurnStartPopupText;
    [SerializeField] private TextMeshProUGUI guestTurnStartPopupText;

    [SerializeField] private float popupDuration = 1f;
    [Space]
    [Header("얼마나 빠르게 사라질 것이냐")]
    [SerializeField] private float fadeDuration = 0.3f;

    private void Start()
    {
        
        EventHub.Instance?.RegisterEvent<IGS.ShowTurnStartPopup>(OnShowTurnStartPopup);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.ShowTurnStartPopup>(OnShowTurnStartPopup);
    }
    
    private void OnShowTurnStartPopup(IGS.ShowTurnStartPopup evt)
    {
        CanvasGroup targetGroup;
        TextMeshProUGUI targetText;

        if(evt.owner == _Project.Scripts.InGame.InGameOwner.Left)
        {
            targetText = hostTurnStartPopupText;
            targetGroup = hostCanvasGroup;
        }
        else
        {
            targetText = guestTurnStartPopupText;
            targetGroup = guestCanvasGorup;
        }
        
        targetText.text = $"{evt.nickname}의 차례입니다!";
        targetGroup.gameObject.SetActive(true);

        FadeCanvasGroup(targetGroup, 1f, fadeDuration).Forget();

        ShowTurnStartPopupAsync(targetGroup).Forget();
    }

    private async UniTaskVoid ShowTurnStartPopupAsync(CanvasGroup targetGroup)
    {
        float effectiveDuration = Mathf.Max(0, popupDuration - fadeDuration);

        await UniTask.Delay((int)(effectiveDuration * 1000));

        await FadeCanvasGroup(targetGroup, 0f, fadeDuration);
    }
    private async UniTask FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            canvasGroup.alpha = alpha;
            await UniTask.Yield();
        }
        canvasGroup.alpha = targetAlpha;

        if(targetAlpha > 0.0f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

}
