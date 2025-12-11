using DonutClash.UI.GlobalUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace _Project.Scripts.EventStructs
{
    public static class UIStructs
    {
        public struct SkipDialogEvent : IEvent { }
        public struct EndDialogEvent : IEvent { }

        #region Main Shop UI Events

        public struct RequestOpenShopCategoryEvent : IEvent
        {
            public ShopCategoryType category;
            public RequestOpenShopCategoryEvent(ShopCategoryType category) => this.category = category;
        }

        #endregion

        // TODO : 아래는 샘플을 위한 이벤트 구조체입니다.
        public struct PrintDonutStateEvent : IEvent
        {
            public DonutInstanceData data;

            public PrintDonutStateEvent(DonutInstanceData data)
            {
                this.data = data;
            }
        }
        
        public struct RequestDonutInstanceData : IEvent //도넛 인스턴스를 보내는 이벤트 
        {
            public DonutInstanceData donutinstance;
            public RequestDonutInstanceData(DonutInstanceData donut)
            {
                this.donutinstance = donut;
            }

        }

        public struct RequestBakerInstanceData : IEvent //마녀 인스턴스 보내는 이벤트
        {
            public BakerInstanceData bakerinstance;
            public RequestBakerInstanceData(BakerInstanceData baker)
            {
                Debug.Log($"request baker instance ===={baker.uid}");
                this.bakerinstance = baker;
            }
        }

        public struct RequestIngredientData : IEvent //재료 수량과 key보내는 이벤트 
        {
            public string ingredient;
            public int ingredientstack;
            public RequestIngredientData(string ingredientUid, int stack)
            {
                this.ingredient = ingredientUid;
                this.ingredientstack = stack;
            }
        }

        public struct OpenInventoryPanelEvent : IEvent //인벤토리 패널을 골라서 여는 이벤트 
        {
            public InventoryButtonType panelButtonType;

            public OpenInventoryPanelEvent(InventoryButtonType buttontype)
            {
                this.panelButtonType = buttontype;
            }
        }

        public struct RequestOpenGlobalPanel : IEvent
        {
            public GlobalPanelType type;
            public GlobalPanelParam param;

            public RequestOpenGlobalPanel(GlobalPanelType type, GlobalPanelParam param)
            {
                this.type = type;
                this.param = param;
            }
        }
        public struct CloseInventoryPanelEvent : IEvent { } //인벤토리에서 나갔을때 사용하는 이벤트 

        public struct SelectInventorySlotEvent : IEvent //인벤토리에서 슬롯을 눌렀을떄 사용하는 이벤트 
        {
            public InventorySlotUI selectSlot;
            public SelectInventorySlotEvent(InventorySlotUI slotUI)
            {
                selectSlot = slotUI;
            }
        }
        
        public struct ChangeInventoryCategoryEvent : IEvent { }//인벤토리에서 카테고리가 달라졌을때 

        public struct RequestCloseAllGlobalPanel : IEvent { }
        public struct RequestPanelClear : IEvent { }
    }
}
