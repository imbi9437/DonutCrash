using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DeckSlotUI : MonoBehaviour
{
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text lvText; 
    [SerializeField] protected Sprite nullSprite;
    
    [SerializeField] protected Button selectButton;
    [SerializeField] protected Button choiceButton;
    [SerializeField] protected Button deleteButton;

    [SerializeField] protected InventoryController inventory;

    protected virtual void Start()
    {
        selectButton.onClick.AddListener(OnSelectButtonClick);
        deleteButton.onClick.AddListener(OnDeleteButtonClick);
    }

    protected virtual void OnDestroy()
    {
        selectButton.onClick.RemoveListener(OnSelectButtonClick);
        deleteButton.onClick.RemoveListener(OnDeleteButtonClick);
    }

    public abstract void Initialize(int index = 0);
    protected abstract void OnSelectButtonClick();
    protected abstract void OnDeleteButtonClick();

    protected void ShowIcon(Sprite sprite)
    {
        iconImage.color = sprite ? Color.white : Color.clear;
        iconImage.sprite = sprite;
    }
    protected void ShowNameText(string text) => nameText.text = text;

    protected void ShowLvText(int lv) => lvText.text = $"Lv. {lv}";
    
    protected void RequestSaveDeck()
    {
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestSaveCurrentDeckEvent());
    }

}
