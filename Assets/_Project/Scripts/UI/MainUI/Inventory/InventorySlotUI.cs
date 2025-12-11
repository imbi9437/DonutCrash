using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private int inventoryIndex;
    [SerializeField] private TMP_Text lvText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text stackText;
    [SerializeField] private GameObject useDonutCheck;
    
    [SerializeField] private Image frameImage; //바깥 프레임이미지
    [SerializeField] private Sprite donutFrame; //도넛프레임을 바꿀 sprite
    [SerializeField] private Sprite currentFrame; //기존 프레임 sprite
    
    
    private DonutData donutData;
    private BakerData bakerData;
    private IngredientData ingredientData;

    private bool isUsing = false;
    private bool isClick = false;
    private float alphaValue = 0.4f;

    private string currentDataID;
    private void OnEnable()
    {
        EventHub.Instance?.RegisterEvent<US.SelectInventorySlotEvent>(OnSlotSelect);
        EventHub.Instance?.RegisterEvent<US.CloseInventoryPanelEvent>(ResetImageAlpha);
        EventHub.Instance?.RegisterEvent<US.ChangeInventoryCategoryEvent>(ResetImageAlpha);

        currentDataID = null;
    }

    private void OnDisable()
    {
        EventHub.Instance?.UnRegisterEvent<US.SelectInventorySlotEvent>(OnSlotSelect);
        EventHub.Instance?.UnRegisterEvent<US.CloseInventoryPanelEvent>(ResetImageAlpha);
        EventHub.Instance?.UnRegisterEvent<US.ChangeInventoryCategoryEvent>(ResetImageAlpha);
        
    }

    public void DonutInitialize(DonutInstanceData donut) //도넛슬롯버튼 초기화 
    {
        RemoveListener();
        ResetImage();
        iconImage.sprite = null;
        iconImage.enabled = false;
        currentDataID = donut.origin;
        if (DataManager.Instance.TryGetDonutData(donut.origin, out DonutData data))
            AddressableLoader.AssetLoadByPath<Sprite>(data.resourcePath, x =>
            {
                if (currentDataID != donut.origin) return; 
                iconImage.sprite = x;
                iconImage.enabled = true;
            }).Forget();
        selectButton.onClick.AddListener(() =>
        {
            EventHub.Instance?.RaiseEvent(new US.SelectInventorySlotEvent(this));
            EventHub.Instance?.RaiseEvent(new US.RequestDonutInstanceData(donut));
        });
    }

    public void BakerInitialize(BakerInstanceData baker) //마녀슬롯버튼 초기화
    {
        RemoveListener();
        ResetImage();
        iconImage.sprite = null;
        iconImage.enabled = false;
        currentDataID = baker.origin;
        selectButton.onClick.AddListener(() =>
        {
            EventHub.Instance?.RaiseEvent(new US.SelectInventorySlotEvent(this));
            EventHub.Instance?.RaiseEvent(new US.RequestBakerInstanceData(baker));
        });
        if (DataManager.Instance.TryGetBakerData(baker.origin, out BakerData data))
            AddressableLoader.AssetLoadByPath<Sprite>(data.resourcePath, x =>
            {
                if (currentDataID != baker.origin) return; 
                iconImage.enabled = true;
                iconImage.sprite = x;
            }).Forget();
    }
    public void DonutUsing()
    {
        useDonutCheck.SetActive(true);
    }

    public void DonutNotUsing()
    {
        useDonutCheck.SetActive(false);
    }

    public void IngredientInitialize(string ingredientUid, int stack)
    {
        RemoveListener();
        ResetImage();
        iconImage.sprite = null;
        iconImage.enabled = false;
        currentDataID = ingredientUid;
        selectButton.onClick.AddListener(() =>
        {
            EventHub.Instance?.RaiseEvent(new US.SelectInventorySlotEvent(this));
            EventHub.Instance?.RaiseEvent(new US.RequestIngredientData(ingredientUid, stack));
        });

        if (DataManager.Instance.TryGetIngredientData(ingredientUid, out IngredientData data))
            AddressableLoader.AssetLoadByPath<Sprite>(data.resourcePath, x =>
            {
                if(currentDataID != ingredientUid) return;
                iconImage.enabled = true;
                iconImage.sprite = x;
            }).Forget();
    }

    private void RemoveListener() //리스너 초기화용  및 프레임 초기화용 
    {
        selectButton.onClick.RemoveAllListeners();
    }


    public void SetData(DonutInstanceData donut)
    {
        if (donut == null)
        {
            lvText.text = "";
            stackText.text = "";
            return;
        }
        frameImage.sprite = donutFrame;
        stackText.text = "";
        lvText.text = $"LV : {donut.level}";
        DataManager.Instance?.TryGetDonutData(donut.origin, out donutData);
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
                frameImage.sprite = currentFrame;
                Debug.Log("티어가 1~3티어가 아닙니다 ");
                break;
        }
    }

    // Baker 데이터 바인딩 (오버로딩)
    public void SetData(BakerInstanceData baker)
    {
        if (baker == null)
        {
            lvText.text = "";
            stackText.text = "";
            return;
        }
        ResetFrame();
        DataManager.Instance?.TryGetBakerData(baker.origin, out bakerData);
        stackText.text = "";
        lvText.text = $"LV : {baker.level}";
    }

    public void SetData(string ingredientUid, int stack)
    {
        if (string.IsNullOrEmpty(ingredientUid))
        {
            lvText.text = "";
            stackText.text = "";
            return;
        }
        ResetFrame();
        DataManager.Instance?.TryGetIngredientData(ingredientUid, out ingredientData);
        lvText.text = "";
        stackText.text = $"{stack}";
    }

    public void ResetFrame() //프레임 리셋 
    {
        frameImage.sprite = currentFrame;
        frameImage.color = Color.white;
    }
    
    private void OnSlotSelect(US.SelectInventorySlotEvent evt)
    {
        if (evt.selectSlot != this)
        {
            SelectSlotAlphaChange(false); // 자신이 아니라면, 선택 해제 상태로 변경합니다.
        }
        else
        {
            SelectSlotAlphaChange(true);
        }
    }

    private void ResetImageAlpha(US.CloseInventoryPanelEvent evt) => ResetImage();
    private void ResetImageAlpha(US.ChangeInventoryCategoryEvent evt) => ResetImage();
    private void SelectSlotAlphaChange(bool isSelect) //선택한 슬롯아이콘 이미지 alpha변경 
    {
        float alpha = isSelect ? alphaValue : 1f; 
        Color temp = iconImage.color;
        temp.a = alpha;
        iconImage.color = temp;
    }

    private void ResetImage()
    {
        Color temp = iconImage.color;
        temp.a = 1f;
        iconImage.color = temp;
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