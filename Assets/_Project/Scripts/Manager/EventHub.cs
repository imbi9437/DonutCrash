using System;
using System.Collections.Generic;
using _Project.Scripts.EventStructs;
using UnityEngine;


[DefaultExecutionOrder(-999)]   //스크립트의 초기화를 가장 먼저한다는 것을 보장하기 위한 어트리뷰트
public class EventHub : MonoSingleton<EventHub>
{
    private readonly Dictionary<Type, Delegate> _eventDic = new();

    /// <summary>
    /// <see cref="IEvent"/> 인터페이스를 구현하는 구조체를 매개변수로 사용하는 함수들을 등록하는 함수
    /// </summary>
    /// <param name="callback">등록할 함수</param>
    /// <typeparam name="T"><see cref="IEvent"/>를 구현하는 구조체</typeparam>
    public void RegisterEvent<T>(Action<T> callback) where T : struct, IEvent
    {
        if (_eventDic.TryGetValue(typeof(T), out var handler) == false)
            _eventDic.Add(typeof(T), callback);
        else
            _eventDic[typeof(T)] = (Action<T>)handler + callback;
    }

    /// <summary>
    /// <see cref="IEvent"/> 인터페이스를 구현하는 구조체를 매개변수로 사용하는 함수들을 등록 해제하는 함수
    /// </summary>
    /// <param name="callback">등록 해제할 함수</param>
    /// <typeparam name="T"><see cref="IEvent"/>를 구현하는 구조체</typeparam>
    public void UnRegisterEvent<T>(Action<T> callback) where T : struct, IEvent
    {
        if (_eventDic.TryGetValue(typeof(T), out var handler) == false) return;
        
        var curHandler = (Action<T>)handler - callback;
        
        if (curHandler == null) _eventDic.Remove(typeof(T));
        else _eventDic[typeof(T)] = curHandler;
    }

    /// <summary>
    /// 등록되어있는 <see cref="T"/>타입의 이벤트들을 모두 실행하는 함수
    /// </summary>
    /// <param name="args">매개변수로 사용할 구조체</param>
    /// <typeparam name="T"><see cref="IEvent"/>를 구현하는 구조체</typeparam>
    public void RaiseEvent<T>(T args) where T : struct, IEvent
    {
        if (_eventDic.TryGetValue(typeof(T), out var handler) == false) return;

        ((Action<T>)handler)?.Invoke(args);
    }
}
