using DG.Tweening;
using DonutClash.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;


public abstract class ShopCategoryPanel : UIPanel
{
    [SerializeField] protected MerchandiseButton buttonPrefab;
    [SerializeField] protected Transform contentParent;
    [SerializeField] protected Transform inActiveParent;

    protected ObjectPool<MerchandiseButton> buttonPool;
    protected List<MerchandiseButton> buttonList = new();

    protected void OnDisable()
    {
        HideAllMerchandiseButton();
    }

    public override void Initialize(UIController controller)
    {
        buttonPool = new ObjectPool<MerchandiseButton>(OnCreate, OnGet, OnRelease);
        
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        
        transform.localScale = Vector3.zero;
    }

    public override void Show()
    {
        if (sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        
        gameObject.SetActive(true);
        
        var fadeTween = canvasGroup.DOFade(1, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(1, animationTime).SetEase(animationEase);
        
        sequence.Join(fadeTween);
        sequence.Join(scaleTween);
        
        sequence.onComplete = () => canvasGroup.interactable = true;
    }
    public override void Hide()
    {
        if(sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        
        canvasGroup.interactable = false;
        
        var fadeTween = canvasGroup.DOFade(0, animationTime).SetEase(animationEase);
        var scaleTween = transform.DOScale(0, animationTime).SetEase(animationEase);
        
        sequence.Join(fadeTween);
        sequence.Join(scaleTween);
        
        sequence.onComplete = () => gameObject.SetActive(false);
    }

    protected void ShowMerchandiseButton(MerchandiseType type)
    {
        var merchandiseList = DataManager.Instance.GetMerchandiseTable().Where(m => m.merchandiseType == type)
            .OrderBy(d => d.uid);

        foreach (var data in merchandiseList)
        {
            var button = buttonPool.Get();
            button.Initialize(data.uid);
        }
    }
    protected void HideAllMerchandiseButton()
    {
        foreach (MerchandiseButton button in buttonList)
        {
            buttonPool.Release(button);
        }
        
        buttonList.Clear();
    }
    
    #region ObjectPool Methods

    private MerchandiseButton OnCreate() => Instantiate(buttonPrefab, contentParent);
    
    private void OnGet(MerchandiseButton button)
    {
         button.transform.SetParent(contentParent);
         button.transform.localScale = Vector3.one;
         buttonList.Add(button);
    }
    
    private void OnRelease(MerchandiseButton button)
    {
        button.transform.SetParent(inActiveParent);
        button.transform.localScale = Vector3.one;
    }

    #endregion

    
}