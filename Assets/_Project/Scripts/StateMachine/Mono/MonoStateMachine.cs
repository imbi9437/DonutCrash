using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour 기반 상태 패턴 최상위 객체 <br/>
/// 상태 변경의 경우 int값 외에도 자식 객체에서 선언한 enum같은 값을 override를 통해 ChangeState 함수 확장하여 사용 <br/>
/// <code>public void ChangeState(Enum enum) => ChangeState((int)enum);</code>
/// </summary>
public abstract class MonoStateMachine : MonoBehaviour
{
    private readonly Dictionary<int, MonoState> _stateDic = new();
    private MonoState _prevState;
    private MonoState _currentState;

    protected virtual void Awake()
    {
        RegisterStates();
    }

    private void RegisterStates()
    {
        var states = GetComponentsInChildren<MonoState>(true);

        foreach (var state in states)
        {
            _stateDic.TryAdd(state.index, state);
            state.Initialize(this);
            state.gameObject.SetActive(false);
        }
    }

    public void ChangeState(int index)
    {
        if (_currentState != null && _currentState.index == index) return;
        if (_stateDic.ContainsKey(index) == false) return;
        
        _currentState?.gameObject.SetActive(false);
        _prevState = _currentState;
        _currentState = _stateDic[index];
        _currentState.gameObject.SetActive(true);
    }

    public void RollbackState()
    {
        if (_prevState != null) return;
        ChangeState(_prevState.index);
    }
}
