using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentDeckInfoUI : MonoBehaviour
{
    [Serializable]
    public class DonutInfo
    {
        public Image icon;
        public TMP_Text lvText;
    }
    
    [SerializeField] private List<DonutInfo> donutSlots;
    
    [SerializeField] private Image bakerImage;

    [SerializeField] private TMP_Text bakerNameText;
    [SerializeField] private TMP_Text bakerLevelText;
    [SerializeField] private TMP_Text bakerDescText;

    private void OnEnable()
    {
        SetBakerInfo();
        SetDonutInfo();
    }

    private void SetBakerInfo()
    {
        var baker = DataManager.Instance.DeckData.baker;

        if (baker == null)
        {
            bakerNameText.text = "";
            bakerDescText.text = "";
            bakerLevelText.text = "";
            
            SetSprite(null, bakerImage);
            return;
        }
        
        DataManager.Instance.TryGetBakerData(baker.origin, out var origin);

        bakerNameText.text = origin.bakerName;
        bakerDescText.text = origin.bakerDescription;
        bakerLevelText.text = $"레벨 : {baker.level}";

        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => SetSprite(x,bakerImage)).Forget();
    }

    private void SetDonutInfo()
    {
        var donuts = DataManager.Instance.DeckData.waitingDonuts;

        for (int i = 0; i < donuts.Count; i++)
        {
            int index = i;
            var donut = donuts[i];

            if (donut == null)
            {
                donutSlots[index].lvText.text = "";
                SetSprite(null, donutSlots[index].icon);
                continue;
            }
            
            DataManager.Instance.TryGetDonutData(donut.origin, out var origin);
            AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => SetDonutSprite(x,donutSlots[index].icon)).Forget();
            donutSlots[index].lvText.text = $"Lv.{donut.level}";
        }
    }

    private void SetSprite(Sprite sprite, Image image)
    {
        image.color = sprite == false ? Color.clear : Color.white;
        image.sprite = sprite;
    }
    
    private void SetDonutSprite(Sprite sprite, Image image)
    {
        image.color = sprite == false ? Color.clear : Color.white;
        image.sprite = sprite;
    }
}
