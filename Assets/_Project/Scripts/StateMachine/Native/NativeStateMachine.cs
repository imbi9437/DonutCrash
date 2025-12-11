using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.StateMachine.Native;
using UnityEngine;

/// <summary>
/// 기본적인 상태 패턴 머신
/// </summary>
/// <typeparam name="T">해당 머신을 사용할 객체 EX)몬스터, NPC등</typeparam>
public abstract class NativeStateMachine<T> where T : class
{
    private T _owner;
    
    private IState<T> _currentState;
    private IState<T> _prevState;
    
    public NativeStateMachine(T owner) => _owner = owner;
    
    public void Update() => _currentState?.Update(_owner);

    public void ChangeState(IState<T> state)
    {
        if (state == null) return;
        
        _prevState?.Exit(_owner);
        _prevState = _currentState;
        _currentState = state;
        _currentState.Enter(_owner);
    }
    
    public void RollbackState() => ChangeState(_prevState);
}
