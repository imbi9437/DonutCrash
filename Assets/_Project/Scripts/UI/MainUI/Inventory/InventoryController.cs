using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using US = _Project.Scripts.EventStructs.UIStructs;

public enum InventoryButtonType
{
    Null,
    OnlyDonut,
    OnlyBaker,
    OnlyIngredient,
    Combine
}

[Flags]
public enum InventoryCategory
{
    Donut = 1 << 0,
    Baker = 1 << 1,
    Ingredient = 1 << 2,
}

public enum DonutSortKey
{
    Level,
    Origin
}

public enum SortOrder
{
    Asc,
    Desc
}


public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryContent; //슬롯이 자식으로 들어갈 부모
    [SerializeField] private GameObject slotPrefab; //슬롯 프리펩
    [SerializeField] private List<DonutInstanceData> donuts = new(); //유저 도넛인벤토리
    [SerializeField] private List<BakerInstanceData> bakers = new(); //유저 마녀인벤토리
    [SerializeField] private DeckData userDeckData;
    private Dictionary<string, int> ingredients = new();

    [SerializeField] private Toggle onlyDonutButton;
    [SerializeField] private Toggle onlyBakerButton;
    [SerializeField] private Toggle onlyIngredientButton;
    [SerializeField] private Toggle combineButton;
    
    [SerializeField] private CustomButton sortLevelButton;
    [SerializeField] private CustomButton sortTierButton;


    private InventoryButtonType currentslot = InventoryButtonType.Null;
    private InventoryCategory currentcategory;

    [SerializeField] private Transform slotParent;
    [SerializeField] private InventorySlotUI slotUIPrefab;

    private List<InventorySlotUI> slots = new();
    private ObjectPool<InventorySlotUI> slotPool;
    private int currentSlotIndex = 0;

    private DonutSortKey currentSortKey = DonutSortKey.Origin;
    private SortOrder currentSortOrder = SortOrder.Asc;

    private void OnEnable()
    {
        slotPool = new ObjectPool<InventorySlotUI>(
            createFunc: OnCreate,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease
        );
        EventHub.Instance?.RegisterEvent<US.OpenInventoryPanelEvent>(StartInventory);
        EventHub.Instance?.RegisterEvent<US.CloseInventoryPanelEvent>(CurrentSlotReSet);
        SubButtons();
    }

    private void OnDisable()
    {
        UnSubButtons();
        EventHub.Instance?.UnRegisterEvent<US.OpenInventoryPanelEvent>(StartInventory);
        EventHub.Instance?.UnRegisterEvent<US.CloseInventoryPanelEvent>(CurrentSlotReSet);
    }


    //유저 인벤토리데이터 가져오기 
    private void DataConnect()
    {
        donuts = DataManager.Instance.Donuts;
        bakers = DataManager.Instance.Bakers;
        ingredients = DataManager.Instance.Ingredients;
        userDeckData = DataManager.Instance.DeckData;
    }

    private void StartInventory(US.OpenInventoryPanelEvent evt)
    {
        DataConnect();
        InventoryButtonSelect(evt.panelButtonType);
    }
    
    private void InventoryButtonSelect(InventoryButtonType type)
    {
        switch (type)
        {
            case InventoryButtonType.OnlyDonut:
                ShowItem(InventoryCategory.Donut);
                onlyDonutButton.gameObject.SetActive(true);
                onlyBakerButton.gameObject.SetActive(false);
                onlyIngredientButton.gameObject.SetActive(false);
                combineButton.gameObject.SetActive(false);
                sortLevelButton.gameObject.SetActive(true);
                sortTierButton.gameObject.SetActive(true);
                break;
            case InventoryButtonType.OnlyBaker:
                ShowItem(InventoryCategory.Baker);
                onlyBakerButton.gameObject.SetActive(true);
                onlyIngredientButton.gameObject.SetActive(false);
                onlyDonutButton.gameObject.SetActive(false);
                combineButton.gameObject.SetActive(false);
                sortLevelButton.gameObject.SetActive(false);
                sortTierButton.gameObject.SetActive(false);
                break;
            case InventoryButtonType.OnlyIngredient:
                ShowItem(InventoryCategory.Ingredient);
                onlyIngredientButton.gameObject.SetActive(true);
                onlyBakerButton.gameObject.SetActive(false);
                onlyDonutButton.gameObject.SetActive(false);
                combineButton.gameObject.SetActive(false);
                sortLevelButton.gameObject.SetActive(false);
                sortTierButton.gameObject.SetActive(false);
                break;
            case InventoryButtonType.Combine:
                ShowItem(InventoryCategory.Donut | InventoryCategory.Baker | InventoryCategory.Ingredient); //3개 동시에 슬롯넣기 
                combineButton.gameObject.SetActive(true);
                onlyBakerButton.gameObject.SetActive(true);
                onlyIngredientButton.gameObject.SetActive(true);
                onlyDonutButton.gameObject.SetActive(true);
                sortLevelButton.gameObject.SetActive(true);
                sortTierButton.gameObject.SetActive(true);
                break;
        }
    }

    private void CurrentSlotReSet(US.CloseInventoryPanelEvent evt) //인벤토리가 닫혔을때 currentslot 초기화
    {
        currentslot = InventoryButtonType.Null;
    }

    private void SubButtons()
    {
        onlyBakerButton.onValueChanged.AddListener(ison =>
        {
            if (ison && currentslot != InventoryButtonType.OnlyBaker)
            {
                EventHub.Instance.RaiseEvent(new US.ChangeInventoryCategoryEvent());
                ShowItem(InventoryCategory.Baker);
                sortLevelButton.gameObject.SetActive(false);
                sortTierButton.gameObject.SetActive(false);
                currentslot = InventoryButtonType.OnlyBaker;
            }
            
        }); //ison이 true일때만 실행시키게 만들어서 버그 수정 완료
        onlyDonutButton.onValueChanged.AddListener(ison =>
        {
            if (ison && currentslot != InventoryButtonType.OnlyDonut)
            {
                EventHub.Instance.RaiseEvent(new US.ChangeInventoryCategoryEvent());
                ShowItem(InventoryCategory.Donut);
                sortLevelButton.gameObject.SetActive(true);
                sortTierButton.gameObject.SetActive(true);
                currentslot = InventoryButtonType.OnlyDonut;
            }
        });
        onlyIngredientButton.onValueChanged.AddListener(ison =>
        {
            if (ison && currentslot != InventoryButtonType.OnlyIngredient)
            {
                EventHub.Instance.RaiseEvent(new US.ChangeInventoryCategoryEvent());
                ShowItem(InventoryCategory.Ingredient);
                sortLevelButton.gameObject.SetActive(false);
                sortTierButton.gameObject.SetActive(false);
                currentslot = InventoryButtonType.OnlyIngredient;
            }
        });
        combineButton.onValueChanged.AddListener(ison =>
        {
            if (ison && currentslot != InventoryButtonType.Combine)
            {
                EventHub.Instance.RaiseEvent(new US.ChangeInventoryCategoryEvent());
                ShowItem((InventoryCategory)7);
                sortLevelButton.gameObject.SetActive(true);
                sortTierButton.gameObject.SetActive(true);
                currentslot = InventoryButtonType.Combine;
            } 
        }); //3가지 동시에 실행 
        sortTierButton.onClick.AddListener(() =>
        {
            if (currentSortKey == DonutSortKey.Origin)
                OnClickToggleSortOrder();          
            else
                currentSortOrder = SortOrder.Asc;
            currentSortKey = DonutSortKey.Origin;
            RefreshInventory();
        });
        sortLevelButton.onClick.AddListener(() =>
        {
            if (currentSortKey == DonutSortKey.Level)
                OnClickToggleSortOrder();          
            else
                currentSortOrder = SortOrder.Asc;  

            currentSortKey = DonutSortKey.Level;
            RefreshInventory();
        });
    }

    private void UnSubButtons()
    {
        onlyBakerButton.onValueChanged.RemoveAllListeners();
        onlyDonutButton.onValueChanged.RemoveAllListeners();
        onlyIngredientButton.onValueChanged.RemoveAllListeners();
        combineButton.onValueChanged.RemoveAllListeners();
        sortLevelButton.onClick.RemoveAllListeners();
        sortTierButton.onClick.RemoveAllListeners();
    }


    #region ObjectPool Methods

    private InventorySlotUI OnCreate() => Instantiate(slotUIPrefab, slotParent);

    private void OnGet(InventorySlotUI slot)
    {
        slot.gameObject.SetActive(true);
        slots.Add(slot);
    }

    private void OnRelease(InventorySlotUI slot)
    {
        slot.DonutNotUsing();
        slot.ResetFrame();
        slot.gameObject.SetActive(false);
        slots.Remove(slot);
    }

    #endregion

    private void ShowItem(InventoryCategory category)
    {
        bool showDonut = (category & InventoryCategory.Donut) == InventoryCategory.Donut;
        bool showBaker = (category & InventoryCategory.Baker) == InventoryCategory.Baker;
        bool showIngredient = (category & InventoryCategory.Ingredient) == InventoryCategory.Ingredient;

        currentcategory = category;
        foreach (var slot in slots.ToList())
        {
            slotPool.Release(slot);
        }
        List<object> allFilteredItems = new List<object>();

        if (showDonut) {
            var sorted = SortDonuts(donuts, currentSortKey, currentSortOrder);

            foreach (var donut in sorted)
                allFilteredItems.Add(donut);
        }

        if (showBaker)
        {
            foreach (BakerInstanceData baker in bakers.OrderBy(d=>d.origin))
            {
                allFilteredItems.Add(baker);
            }
        }
        if (showIngredient)
        {
            foreach (var ingredient in ingredients)
            {
                if (ingredient.Value > 0) //수량이 0이상만 슬롯에 넣기
                {
                    allFilteredItems.Add(ingredient); 
                }
            }
        }

        bool isinfo = false;
        for (int i = 0; i < allFilteredItems.Count; i++)
        {
            InventorySlotUI slot = slotPool.Get();
            var item = allFilteredItems[i];
            slot.transform.SetSiblingIndex(i);
            if (item is DonutInstanceData donut) //도넛 슬롯 넣기
            {
                slot.DonutInitialize(donut);
                slot.SetData(donut);
                bool isDonutUsing = false;
                foreach (DonutInstanceData data in userDeckData.waitingDonuts)
                {
                    if (data == null) continue;             
                    if (data.uid == null) continue;     
                    if (donut == null || donut.uid == null) continue;
                    if (data.uid == donut.uid) { isDonutUsing = true; break; }
                }
                if (isDonutUsing) { slot.DonutUsing();}
                else { slot.DonutNotUsing(); }
            
                if (!isinfo)
                {
                    EventHub.Instance.RaiseEvent(new US.SelectInventorySlotEvent(slot));
                    EventHub.Instance.RaiseEvent(new US.RequestDonutInstanceData(donut)); 
                    isinfo = true;
                }
            }
            else if (item is BakerInstanceData baker) //마녀 슬롯넣기
            {
                slot.BakerInitialize(baker);
                slot.SetData(baker);
                if (!isinfo)
                {
                    EventHub.Instance.RaiseEvent(new US.SelectInventorySlotEvent(slot));
                    EventHub.Instance.RaiseEvent(new US.RequestBakerInstanceData(baker)); 
                    isinfo = true;
                }
            }
            else if(item is KeyValuePair<string, int> ingredient) //재료 슬롯 넣기 
            {
                slot.IngredientInitialize(ingredient.Key,ingredient.Value);
                slot.SetData(ingredient.Key,ingredient.Value);
                if (!isinfo)
                {
                    EventHub.Instance.RaiseEvent(new US.SelectInventorySlotEvent(slot));
                    EventHub.Instance.RaiseEvent(new US.RequestIngredientData(ingredient.Key, ingredient.Value));
                    isinfo = true;
                }
            }
        }
    }
    
    private void RefreshInventory()
    {
        ShowItem(currentcategory);
        Debug.Log("Refresh Inventory");
    }
    
    private IEnumerable<DonutInstanceData> SortDonuts(IEnumerable<DonutInstanceData> list, DonutSortKey key, SortOrder order)
    {
        Func<DonutInstanceData, object> selector = key switch
        {
            DonutSortKey.Level => d => d.level,
            DonutSortKey.Origin   => d => d.origin,
            _ => d => d.level
        };

        return order == SortOrder.Asc ? list.OrderBy(selector) : list.OrderByDescending(selector);
    }
    
    private void OnClickToggleSortOrder()
    {
        currentSortOrder = (currentSortOrder == SortOrder.Asc)
            ? SortOrder.Desc
            : SortOrder.Asc;

        RefreshInventory();
    }
}