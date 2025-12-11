using _Project.Scripts.BehaviourTree;
using _Project.Scripts.DialogSystem;
using _Project.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;

using US = _Project.Scripts.EventStructs.UIStructs;

public class SampleDialogTree : MonoBehaviour
{
    public static int CurrentUI = 0;
    [SerializeField] private Transform canvas;
    [SerializeField] private DialogPanel dialogPrefab;

    private TutorialDialogTree _tutorialDialogTree;
    
    private void Start()
    {
        _tutorialDialogTree = new TutorialDialogTree();
        MenuTutorialAction menuTutorialAction =  new (_tutorialDialogTree);
        SelectorNode menuSelectorNode = new (new List<INode> { menuTutorialAction });
        DecoratorNode menuDecorator = new (menuSelectorNode, () => CurrentUI == 0);
        MergeTutorialAction mergeTutorialAction = new (_tutorialDialogTree);
        DecoratorNode mergeDecorator = new (mergeTutorialAction, () => CurrentUI == 1);
        _tutorialDialogTree.Initialize(new List<INode> { menuDecorator, mergeDecorator }, canvas, dialogPrefab);
    }

    private void Update()
    {
        // 아래의 두 다이얼로그 이벤트 호출은 각 상황에 맞는 이벤트를 호출하여 다이얼 로그를 진행할 수 있도록 변경하여 사용
        // 동작의 테스트를 위한 이벤트 발생 코드
        if (Input.GetKeyDown(KeyCode.A))
            EventHub.Instance.RaiseEvent(new US.SkipDialogEvent());
        
        if (Input.GetKeyDown(KeyCode.S))
            EventHub.Instance.RaiseEvent(new US.EndDialogEvent());
        
        if (Input.GetKeyDown(KeyCode.D))
            _tutorialDialogTree.UpdateDialogTree();

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"머지 팝업 등장 가정, 머지 팝업 최초 등장시 머지 튜토리얼 진행");
            //임의의 UI 상태를 나타내는 변수를 변경, 추후에는 이벤트를 통해 호출할 가능성 높음
            CurrentUI = (CurrentUI + 1) % 2;
            _tutorialDialogTree.UpdateDialogTree();
        }
    }
}
