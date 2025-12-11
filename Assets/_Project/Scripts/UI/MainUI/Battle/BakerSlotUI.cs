using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BakerSlotUI : DeckSlotUI
{
    [SerializeField] private TMP_Text bakerLevelText;
    [SerializeField] private TMP_Text bakerDescText;
    private void OnEnable()
    {
        SetData();
    }

    public override void Initialize(int index = 0)
    {
        SetData();
    }

    protected override void OnSelectButtonClick()
    {
        inventory.gameObject.SetActive(true);
        EventHub.Instance.RegisterEvent<UIStructs.RequestBakerInstanceData>(OnSelectedData);
        choiceButton.onClick.RemoveAllListeners(); //버튼 초기화
        EventHub.Instance.RaiseEvent(new UIStructs.OpenInventoryPanelEvent(InventoryButtonType.OnlyBaker));
        
    }
    protected override void OnDeleteButtonClick()
    {
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestChangeDeckBakerEvent(null));
        
        iconImage.sprite = nullSprite;
        nameText.text = "";
        bakerLevelText.text = "";
        bakerDescText.text = "";
        // deleteButton.gameObject.SetActive(false);
        SetSprite(nullSprite,iconImage);
        RequestSaveDeck();
    }

    private void OnSelectedData(UIStructs.RequestBakerInstanceData evt)
    {
        choiceButton.onClick.AddListener(()=>
        {
            Debug.Log("Bakerchoice Button Clicked");
            RequestChangeData(evt.bakerinstance);
        });
    }

    private void SetData()
    {
        var instance = DataManager.Instance.DeckData.baker;
        if (instance == null)
        {
            iconImage.sprite = nullSprite;
            nameText.text = "";
            bakerLevelText.text = "";
            bakerDescText.text = "";
            // deleteButton.gameObject.SetActive(false);
            SetSprite(nullSprite,iconImage);
            return;
        }
        
        DataManager.Instance.TryGetBakerData(instance.origin, out var origin);

        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => SetSprite(x, iconImage)).Forget();
        nameText.text = origin.bakerName;
        bakerLevelText.text = instance.level.ToString();
        bakerDescText.text = origin.bakerDescription;
        // deleteButton.gameObject.SetActive(true);
    }

    private void SetSprite(Sprite sprite, Image image)
    {
        image.color = sprite ? Color.white : Color.clear;
        image.sprite = sprite;
    }

    private void RequestChangeData(BakerInstanceData data)
    {
        if (data == null) //데이터가 없을때는 delect버튼 비활성화
        {
            iconImage.sprite = nullSprite;
            nameText.text = "";
            bakerLevelText.text = "";
            bakerDescText.text = "";
            // deleteButton.gameObject.SetActive(false);
            SetSprite(nullSprite,iconImage);
            return;
        }
        
        DataManager.Instance.TryGetBakerData(data.origin, out var origin);
        
        
        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => SetSprite(x, iconImage)).Forget();
        nameText.text = origin.bakerName;
        bakerLevelText.text = data.level.ToString();
        bakerDescText.text = origin.bakerDescription;
        // deleteButton.gameObject.SetActive(true);
        
        EventHub.Instance.UnRegisterEvent<UIStructs.RequestBakerInstanceData>(OnSelectedData);
        
        EventHub.Instance.RaiseEvent(new OutGameStructs.RequestChangeDeckBakerEvent(data.uid));
        EventHub.Instance.RaiseEvent(new UIStructs.CloseInventoryPanelEvent());
        RequestSaveDeck();
        inventory.gameObject.SetActive(false);
    }
    
    
}
