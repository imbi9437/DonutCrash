using DG.Tweening;
using System;
using UnityEngine;

public class LoadingSpinnerController : MonoBehaviour
{
    private RectTransform _rect;
    private Sequence _sequence;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _sequence = DOTween.Sequence();
        _sequence.Append(_rect.DORotate(new Vector3(0, 0, 270), .25f).SetEase(Ease.Linear));
        _sequence.Append(_rect.DORotate(new Vector3(0, 0, 180), .25f).SetEase(Ease.Linear));
        _sequence.Append(_rect.DORotate(new Vector3(0, 0, 90), .25f).SetEase(Ease.Linear));
        _sequence.Append(_rect.DORotate(new Vector3(0, 0, 0), .25f).SetEase(Ease.Linear));
        _sequence.SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        _sequence.Kill();
    }
}
