using System;
using System.Collections.Generic;
using UnityEditor;

namespace _Project.Scripts.EventStructs
{
    public static class RecipeEventStructs
    {
        //도넛 생성 버튼이 눌렸을 때 트레이에 도넛을 올리는 UI패널에 정보를 넘겨주는 이벤트
        public struct AddToTrayViewEvent : IEvent
        {
            public DonutData donutInfo;
            public int requiredCount;
            public int slotNumber;
            public int requiredGold;

            public int trayGrade;
            public AddToTrayViewEvent(DonutData data, int c, int num, int requiredGold, int trayGrade)
            {
                this.donutInfo = data;
                this.requiredCount = c;
                this.slotNumber = num;
                this.requiredGold = requiredGold;
                this.trayGrade = trayGrade;
            }
        }
        public struct OnTrayPanleViewEvent : IEvent { }
        /// <summary>요구하는 골드 텍스트 변경 </summary>
        public struct RefreshGoldViewEvent : IEvent
        {
            public int requiredGold;
            public RefreshGoldViewEvent(int requiredGold)
            {
                this.requiredGold = requiredGold;
            }
        }

        public struct OnSelectedDonutViewEvent : IEvent
        {
            public int slotNumber;
            public OnSelectedDonutViewEvent(int slotNumber)
            {
                this.slotNumber = slotNumber;
            }

        }
        public struct CanCreateDonutViewEvnet : IEvent
        {
            public bool canCreate;
            public CanCreateDonutViewEvnet(bool canCreate)
            {
                this.canCreate = canCreate;
            }
        }
        //도넛생성 패널 닫는 이벤트
        public struct CreatePanelCloseViewEvent : IEvent { }
        //트레이에서 도넛 생성버튼을 눌러 도넛 생성이 들어갔을 때
        public struct StartBakingViewEvent : IEvent
        {
            public DateTime startTime;
            public DateTime endTime;
            public StartBakingViewEvent(DateTime startTime, DateTime endTime)
            {
                this.startTime = startTime;
                this.endTime = endTime;
            }
        }
        public struct OnDonutSelectPanelViewEvent : IEvent { }
        //조건이 충족되어 트레이에 올리기버튼 활성화에 대한 이벤트
        public struct OnTrayButtonViewEvent : IEvent
        {
            public bool isOn;
            public OnTrayButtonViewEvent(bool isOn)
            {
                this.isOn = isOn;
            }
        }

        public struct RequestOpenCreateDonutPopUp : IEvent
        {
            public RecipeData recipeData;
            public int slotNum;
            public RequestOpenCreateDonutPopUp(RecipeData recipeData, int slotNum)
            {
                this.recipeData = recipeData;
                this.slotNum = slotNum;
            }
        }
        public struct RequestOnSuccessMark : IEvent
        {
            public bool ison;
            public RequestOnSuccessMark(bool ison)
            {
                this.ison = ison;
            }
        }

        #region 도넛 머지 뷰 이벤트
        //재료로 사용 가능한 도넛을 보여주는 패널 활성화 이벤트
        public struct OpenMergeViewlEvent : IEvent
        {
            public DonutInstanceData donutData;
            public OpenMergeViewlEvent(DonutInstanceData donutData)
            {
                this.donutData = donutData;
            }
        }

        //머지를 할 도넛 선택 할 인벤토리 활성화 이벤트
        public struct DonutInvenViewEvent : IEvent { }

        public struct CanAutoMergeViewEvent : IEvent
        {
            public bool isOn;
            public CanAutoMergeViewEvent(bool isOn)
            {
                this.isOn = isOn;
            }
        }
        public struct CanMergeViewEvnet : IEvent
        {
            public bool isOn;
            public CanMergeViewEvnet(bool isOn)
            {
                this.isOn = isOn;
            }
        }
        public struct RequestDonutInstanceData : IEvent
        {
            public DonutInstanceData donut;
            public RequestDonutInstanceData(DonutInstanceData donut)
            {
                this.donut = donut;
            }

        }
        public struct RequestOpenSuccessPopup : IEvent
        {
            public DonutInstanceData donut;
            public RequestOpenSuccessPopup(DonutInstanceData donut)
            {
                this.donut = donut;
            }
        }
        #endregion
        #region 도넛 해금 뷰 이벤트
        public struct SetNodePanelViewEvnet : IEvent
        {
            public DonutNode node;
            public SetNodePanelViewEvnet(DonutNode node)
            {
                this.node = node;
            }
        }

        public struct UnlockNodeViewEvent : IEvent
        {
            public DonutNode node;
            public UnlockNodeViewEvent(DonutNode node)
            {
                this.node = node;
            }
        }
        public struct CanUnlockViewEvent : IEvent
        {
            public DonutNode node;
            public CanUnlockViewEvent(DonutNode node)
            {
                this.node = node;
            }
        }
        public struct RequestPerfectRecipeCount : IEvent { }
        public struct LockNodeViewEvent : IEvent
        {
            public DonutNode node;
            public LockNodeViewEvent(DonutNode node)
            {
                this.node = node;
            }
        }

        public struct RequestRecipeDetailPopup : IEvent
        {
            public string recipeUid;
            public RequestRecipeDetailPopup(string recipeUid)
            {
                this.recipeUid = recipeUid;
            }
        }

        #endregion
    }
}
