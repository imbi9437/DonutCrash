using System.Collections.Generic;

namespace _Project.Scripts.EventStructs
{
    public static class DonutControlStructs
    {
        //도넛 생성에 사용되는 이벤트들 

        /// <summary> 도넛생성을 위해 재료 검사 이벤트 </summary>
        public struct CheckRequiredIngredientsControlEvent : IEvent
        {
            public string recipeId;
            public int requiredCount;
            public int trayNum;
            public CheckRequiredIngredientsControlEvent(string recipeId, int requiredCount, int trayNum)
            {
                this.recipeId = recipeId;
                this.requiredCount = requiredCount;
                this.trayNum = trayNum;
            }
        }
        public struct TrayPanelOpenControlEvent : IEvent { }

        public struct OnTrayDonutCancelControlEvent : IEvent
        {

            public int slotNumber;
            public OnTrayDonutCancelControlEvent(int num)
            {
                this.slotNumber = num;
            }
        }
        public struct StartBakingControlEvent : IEvent { }

        //도넛 생성 완료 이벤트
        public struct CreateDonutControlEvent : IEvent { }
        public struct TrayClearControlEvnet : IEvent { }


        public struct OnClickMergeControlEvnet : IEvent { }


        public struct SuccessOnTrayControlEvnet : IEvent
        {
            public string recipeUid;
            public int requiredCount;
            public int slotNumber;
            public SuccessOnTrayControlEvnet(string r, int c, int num)
            {
                this.recipeUid = r;
                this.requiredCount = c;
                this.slotNumber = num;
            }
        }

        //Todo : 패널 => 패널로 이벤트가 전달이 되고있음. 패널 => 매니저 => 패널 구조로 변경 필요.
        //제작 할 도넛을 선택하기 위한 선택버튼이 눌렀을 때
        //제작 할 도넛을 선택하는 패널을 활성화 하는 이벤트

        //Todo : 기능적인 부분과 UI적인 부분이 같이 등록되어있는데 분리 할 것.

        public struct AutoMergeControlEvent : IEvent { }

        public struct OpenInvenControlEvent : IEvent { }
        public struct SelectedDonutControlEvent : IEvent
        {
            public int slotNum;
            public SelectedDonutControlEvent(int slotnum)
            {
                this.slotNum = slotnum;
            }
        }
        //도넛 머지를 할 때 인벤토리에서 선택한 도넛에 대한 패널 활성화 이벤트
        public struct GetMergeableControlEvent : IEvent
        {
            public string instanceId;
            public string origin;
            public GetMergeableControlEvent(string donutInstanceData, string donutOrigin)
            {
                this.instanceId = donutInstanceData;
                this.origin = donutOrigin;
            }
        }
        #region 노드 부분
        public struct InitNodeDonutsControlEvent : IEvent
        {
            public List<DonutNode> rootNodes;
            public InitNodeDonutsControlEvent(List<DonutNode> rootNodes)
            {
                this.rootNodes = rootNodes;
            }
        }
        //시작시 유저가 소지하고있는 레시피를 통해 셋팅
        public struct InitUnlockRecipeControlEvent : IEvent
        {
            public DonutNode node;
            public InitUnlockRecipeControlEvent(DonutNode node)
            {
                this.node = node;
            }
        }
        //해금 버튼을 눌렀을 때 호출되는 이벤트
        //해금 된 레시피를 유저데이터에 추가 시킴.
        public struct OnClickUnlockControlEvent : IEvent
        {
            public DonutNode node;
            public OnClickUnlockControlEvent(DonutNode node)
            {
                this.node = node;
            }
        }
        #endregion
    }
}
