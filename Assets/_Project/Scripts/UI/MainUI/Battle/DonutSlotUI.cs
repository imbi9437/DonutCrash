using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonutSlotUI : DeckSlotUI
{
    private int deckIndex;
    [SerializeField] private Image frameImage; //바깥 프레임이미지
    [SerializeField] private Sprite donutFrame; //도넛프레임을 바꿀 sprite
    [SerializeField] private Sprite currentFrame; //기존 프레임 sprite

    private void OnEnable()
    {
        SetData();
        
        EventHub.Instance.RegisterEvent<OutGameStructs.ChangeDeckDonutSuccessEvent>(OnChangedEvent);
    }

    private void OnDisable()
    {
        EventHub.Instance?.UnRegisterEvent<OutGameStructs.ChangeDeckDonutSuccessEvent>(OnChangedEvent);
    }

    private void OnChangedEvent(OutGameStructs.ChangeDeckDonutSuccessEvent evt) => OnChangedDeckDonut(evt.index, evt.uid);
    
    public override void Initialize(int index = 0) => deckIndex = index;

    protected override void OnSelectButtonClick()
    {
        inventory.gameObject.SetActive(true);
        EventHub.Instance.RegisterEvent<UIStructs.RequestDonutInstanceData>(OnSelectedData);
        Debug.Log(deckIndex);
        EventHub.Instance.RaiseEvent(new UIStructs.OpenInventoryPanelEvent(InventoryButtonType.OnlyDonut));
    }
    protected override void OnDeleteButtonClick()
    {
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestChangeDeckDonutEvent(deckIndex, null));
        
        iconImage.sprite = null;
        iconImage.color = Color.clear;
        nameText.text = "";
        lvText.text = "";
        ResetFrame();
        RequestSaveDeck();
        SetSprite(nullSprite,iconImage);
        // deleteButton.gameObject.SetActive(false);
    }

    private void OnSelectedData(UIStructs.RequestDonutInstanceData evt)
    { 
        choiceButton.onClick.RemoveAllListeners(); //버튼 초기화
        choiceButton.onClick.AddListener(()=>
        {
            Debug.Log("Donutchoice Button Clicked");
            RequestChangeData(evt.donutinstance);
        });
    }

    private void SetData()
    {
        var instance = DataManager.Instance.DeckData.waitingDonuts[deckIndex];

        if (instance == null)
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
            nameText.text = "";
            lvText.text = "";
            // deleteButton.gameObject.SetActive(false);
            ResetFrame();
            SetSprite(nullSprite,iconImage);
            return;
        }

        DataManager.Instance.TryGetDonutData(instance.origin, out var origin);
        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => SetSprite(x, iconImage)).Forget();
        // deleteButton.gameObject.SetActive(true);
        SetFrame(origin);
        ShowLvText(instance.level);
    }

    private void SetSprite(Sprite sprite, Image image)
    {
        image.color = sprite ? Color.white : Color.clear;
        image.sprite = sprite;
    }

    private void RequestChangeData(DonutInstanceData data)
    {
        if (data == null)
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
            nameText.text = "";
            lvText.text = "";
            // deleteButton.gameObject.SetActive(false);
            ResetFrame();
            SetSprite(nullSprite,iconImage);
            return;
        }
        Debug.Log($"RequestChangeData  = {deckIndex}");
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestChangeDeckDonutEvent(deckIndex, data.uid));
        EventHub.Instance.UnRegisterEvent<UIStructs.RequestDonutInstanceData>(OnSelectedData);
        EventHub.Instance.RaiseEvent(new UIStructs.CloseInventoryPanelEvent());
        inventory.gameObject.SetActive(false);
    }

    private void OnChangedDeckDonut(int index, string uid)
    {
        if (deckIndex != index) return;
        
        bool hasInstance = DataManager.Instance.TryGetUserDonut(uid, out var instance);

        if (hasInstance == false)
        {
            lvText.text = "";
            ResetFrame();
            ShowIcon(null);
            // deleteButton.gameObject.SetActive(false);
            return;
        }
        
        DataManager.Instance.TryGetDonutData(instance.origin, out var origin);
        // deleteButton.gameObject.SetActive(true);
        SetFrame(origin);
        ShowLvText(instance.level);
        RequestSaveDeck();
        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, ShowIcon).Forget();
    }
    private void ResetFrame() //프레임 리셋 
    {
        frameImage.sprite = currentFrame;
        frameImage.color = Color.white;
    }

    private void SetFrame(DonutData donut)
    {
        frameImage.sprite = donutFrame;
        switch (donut.tier)
        {
            case DonutTier.Tier1:
                frameImage.color = GetColorCode("#00A331");
                break;
            case DonutTier.Tier2:
                frameImage.color = GetColorCode("#23009F");
                break;
            case DonutTier.Tier3:
                frameImage.color = GetColorCode("#A000A3");
                break;
            default:
                frameImage.sprite = currentFrame;
                Debug.Log("티어가 1~3티어가 아닙니다 ");
                break;
        }
    }
    
    private Color GetColorCode(string colorCode)
    {
        if (UnityEngine.ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            return color;
        }
    
        Debug.LogError($"HEX 코드 '{colorCode}' 변환에 실패했습니다.");
        return Color.white;
    }
}
