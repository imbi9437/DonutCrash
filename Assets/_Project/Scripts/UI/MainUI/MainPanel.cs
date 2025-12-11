using _Project.Scripts.UI.MainUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DonutClash.UI.Main
{
    /// <summary>
    /// 메인 씬에서 사용할 UI패널 코드 
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MainPanel : UIPanel
    {
        protected MainUIController MainUIController;

        public override void Initialize(UIController controller)
        {
            MainUIController = controller as MainUIController;
            
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            transform.localScale = Vector3.zero;
        }

        public override void Show()
        {
            if (sequence.IsActive()) sequence.Kill();
            sequence = DOTween.Sequence();
            
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            
            var fadeTween = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
            var scaleTween = transform.DOScale(1, animationTime).SetEase(animationEase);
        
            sequence.Join(fadeTween);
            sequence.Join(scaleTween);
        }

        public override void Hide()
        {
            if (sequence.IsActive()) sequence.Kill();
            sequence = DOTween.Sequence();
            
            canvasGroup.interactable = false;
            
            var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
            var scaleTween = transform.DOScale(0, animationTime).SetEase(animationEase);
            
            sequence.Join(fadeTween);
            sequence.Join(scaleTween);
            
            sequence.onComplete = () => gameObject.SetActive(false);
        }
    }
}
