using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.StateMachine.Native;
using UnityEngine;

/// <summary>
/// 상태의 최상위 객체
/// </summary>
/// <typeparam name="T">해당 상태를 사용할 객체 EX)몬스터, NPC등</typeparam>
public abstract class NativeState<T> : IState<T> where T : class
{
    public abstract void Enter(T owner);
    public abstract void Exit(T owner);
    public abstract void Update(T owner);
}
