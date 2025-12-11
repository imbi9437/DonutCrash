using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class DialogPanel : MonoBehaviour
    {
        [Header("Icon")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI iconText;
    
        [Header("Content")]
        [SerializeField] private TextMeshProUGUI contentText;
    
        [Header("End")]
        [SerializeField] private RectTransform endIcon;
    
        private int _typeDelay;
    
        private Sequence _endIconSequence;
        private Action _endAction;
        private CancellationTokenSource _cts;

        private void OnDisable()
        {
            _cts?.Cancel();
            _endIconSequence?.Kill(true);
        }

        /// <summary>
        /// 다이얼 로그 패널을 초기화 하는 메서드입니다.
        /// 전달 받은 파라미터로 각 UI의 값을 할당하며 콘텐츠 텍스트의 지연 재생을 시작합니다.
        /// </summary>
        /// <param name="iconSprite">아이콘의 스프라이트</param>
        /// <param name="iconStr">아이콘 옆 타이틀의 텍스트</param>
        /// <param name="contentStr">지연 재생될 본문의 텍스트</param>
        /// <param name="typeDelay">자간 지연 재생의 간격, 단위 ms</param>
        /// <param name="endAction">다이얼 로그 닫기시 실행될 콜백</param>
        public void Initialize(Sprite iconSprite, string iconStr, string contentStr, int typeDelay, Action endAction = null)
        {
            iconImage.sprite = iconSprite;
            iconText.text = iconStr;
            contentText.text = contentStr;
            contentText.maxVisibleCharacters = 0;
            contentText.ForceMeshUpdate();
            endIcon.gameObject.SetActive(false);
            _typeDelay = typeDelay;

            _endAction = OnEndDialog;
            _endAction += endAction;
        
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
        
            StreamTextAsync(_cts.Token).Forget();
        }

        /// <summary>
        /// 텍스트 지연 재생을 생략하는 메서드
        /// </summary>
        public void SkipStreamText()
        {
            _cts?.Cancel();
            OnEndStreamText();
        }
        
        /// <summary>
        /// 다이얼 로그 패널을 닫고 초기화 메서드를 통해 전달받은 종료 콜백을 실행
        /// </summary>
        public void EndDialog() => _endAction?.Invoke();

        /// <summary>
        /// 텍스트 지연 재생 종료시 실행되는 메서드
        /// </summary>
        private void OnEndStreamText()
        {
            contentText.maxVisibleCharacters = contentText.textInfo.characterCount;
            endIcon.gameObject.SetActive(true);

            _endIconSequence?.Kill(true);
            _endIconSequence = DOTween.Sequence();
            _endIconSequence.Append(endIcon.DOAnchorPosY(16, .5f).SetEase(Ease.OutCubic));
            _endIconSequence.Append(endIcon.DOAnchorPosY(0, 1f).SetEase(Ease.InCubic));
            _endIconSequence.SetLoops(-1, LoopType.Restart);
        }
        
        private void OnEndDialog()
        {
            _cts?.Cancel();
            _endIconSequence?.Kill(true);
            Destroy(gameObject);
        }

        private async UniTaskVoid StreamTextAsync(CancellationToken ct)
        {
            for (int i = 0; i < contentText.textInfo.characterCount; i++)
            {
                contentText.maxVisibleCharacters++;
                await UniTask.Delay(_typeDelay, cancellationToken: ct);
            }
        
            OnEndStreamText();
        }
    }
}