using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// UI에서 사용할 최상위 패널 객체 각종 패널의 경우 해당 객체 상속받아서 사용
/// PanelType의 경우 열거형(enum)을 int로 캐스팅 할 것
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIPanel : MonoBehaviour
{
    public abstract int PanelType { get; } 
    
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float animationTime = 0.3f;
    [SerializeField] protected Ease animationEase;
    
    protected Sequence sequence;

    public abstract void Initialize(UIController controller);
    public abstract void Show();
    public abstract void Hide();

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
