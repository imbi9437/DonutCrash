using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DonutClash.UI.GlobalUI
{
    /// <summary> 모든 씬에서 사용할 UI 코드 </summary>
    public abstract class GlobalPanel : MonoBehaviour
    {
        public abstract GlobalPanelType GlobalPanelType { get; }
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float animationTime = 0.5f;
        [SerializeField] protected Ease animationEase;

        protected Sequence _sequence;

        public abstract void Initialize();

        public abstract void Show(GlobalPanelParam param);

        public abstract void Hide();
    }
}