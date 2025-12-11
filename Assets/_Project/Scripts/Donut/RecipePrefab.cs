
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePrefab : MonoBehaviour
{

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private Image frameImage; //바깥 프레임이미지
    [SerializeField] private Sprite donutFrame; //도넛프레임을 바꿀 sprite
    [SerializeField] private Sprite currentFrame; //기존 프레임 sprit

    private DonutData donutData;


    public void DonutInitialize(string donutUid) //도넛슬롯버튼 초기화 
    {
        if (DataManager.Instance.TryGetDonutData(donutUid, out DonutData data))
            AddressableLoader.AssetLoadByPath<Sprite>(data.resourcePath, x => iconImage.sprite = x).Forget();


        nameText.text = $"{data.donutName}";
        SetData(data);
    }

    public void SetData(DonutData donut)
    {
        if (donut == null)
        {
            iconImage.gameObject.SetActive(false);
            return;
        }
        frameImage.sprite = donutFrame;
        donutData = donut;
        switch (donutData.tier)
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
                frameImage.color = Color.yellow;
                Debug.Log("티어가 1~3티어가 아닙니다 ");
                break;
        }
        iconImage.gameObject.SetActive(true);
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
