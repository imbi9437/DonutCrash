using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using US = _Project.Scripts.EventStructs.UIStructs;

public class SampleDialogPanel_Sequence : MonoBehaviour
{
    public Transform canvas;
    public DialogPanel dialogPanel;
    
    private DialogSequence _dialogSequence;
    
    private Dictionary<KeyCode, Action> _actions = new Dictionary<KeyCode, Action>();

    private void Start()
    {
        _actions.Add(KeyCode.A, () => EventHub.Instance.RaiseEvent(new US.SkipDialogEvent()));
        _actions.Add(KeyCode.S, () => EventHub.Instance.RaiseEvent(new US.EndDialogEvent()));
        _actions.Add(KeyCode.D, null);
        _actions.Add(KeyCode.F, null);
        
        _dialogSequence = new DialogSequence(canvas, new []
        {
            new DialogFactor(dialogPanel, null, "Sequence No.0", "Press A to Skip, Press S to End", 20),
            new DialogFactor(dialogPanel, null, "Sequence No.1", "Press A to Skip, Press S to End", 30),
            new DialogFactor(dialogPanel, null, "Sequence No.2", "Press A to Skip, Press S to End", 40),
        });
        
        _dialogSequence.StartSequence();
    }

    private void OnDisable()
    {
        _dialogSequence?.CancelSequence();
    }

    // 샘플로 각각 A키가 스킵 S키가 종료로 고정되어 있는데 임의의 버튼을 생성하여 버틀 클릭시 이벤트 허브의 이벤트 발생으로 처리 가능합니다.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            _actions[KeyCode.A]?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.S))
            _actions[KeyCode.S]?.Invoke();
    }
}
