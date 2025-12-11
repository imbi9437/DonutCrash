using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글턴 최상위 객체 <br/>
/// 해당 클래스를 상속받을 경우 상속받는 객체는 싱글턴이 된다.
/// </summary>
/// <typeparam name="T">상속받는 객체의 타입</typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _isApplicationQuitting = false;
    
    [SerializeField] private bool isDontDestroyOnLoad = true;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_isApplicationQuitting) return null;
                if (_instance == null) _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                
                var obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else if (_instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        if (isDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
    }

    protected void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
}
