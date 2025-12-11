using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="MonoBehaviour"/> 기반의 상태 최상위 객체 <br/>
/// index의 경우에는 Property Setter로 설정 <code> public override int index => 1; </code> <br/>
/// 혹은 상태 머신에서 선언한 enum을 캐스팅하여 사용 <code> public override int index => (enumType)enumValue</code> <br/>
/// <see cref="MonoBehaviour"/>의 메세지 함수 활용해 상태에 대한 기능 구현 <br/>
/// OnEnable : 해당 상태 시작 <br/>
/// Update : 해당 상태 지속되는 경우 <br/>
/// OnDisable : 해당 상태 종료
/// </summary>
public abstract class MonoState : MonoBehaviour
{
    public abstract int index { get; }
    protected MonoStateMachine machine;

    public virtual void Initialize(MonoStateMachine machine)
    {
        this.machine = machine;
    }
    
    protected virtual void OnEnable() { }
    protected virtual void Update() { }
    protected virtual void OnDisable() { }

    private void Reset()
    {
        var other = GetComponents<MonoState>();
        if (other != null && other.Length > 1)
        {
            Destroy(this);
            Debug.Log($"<color=red>{typeof(MonoState)}의 경우 오브젝트에 하나의 {typeof(MonoState)}만 존재해야 합니다.</color>");
        }
    }
}