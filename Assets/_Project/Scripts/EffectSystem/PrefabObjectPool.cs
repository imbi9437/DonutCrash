using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PrefabObjectPool<T> where T : Component
{
    private ObjectPool<T> _pool;
    protected T prefab;
    protected Transform parent;

    public PrefabObjectPool<T> Initialize(T prefab, Transform parent)
    {
        if (_pool != null)
        {
            Debug.LogError("이미 초기화된 풀입니다. 풀은 재사용할 수 없습니다.");
            return this;
        }
        
        this.prefab = prefab;
        this.parent = parent;

        _pool = new ObjectPool<T>(OnCreate, OnGet, OnRelease, OnDestroy);
        return this;
    }
    
    public void Get(out T go) => _pool.Get(out go);
    public void Release(T go) => _pool.Release(go);
    public void ClearPool() => _pool.Clear();

    protected virtual T OnCreate()
    {
        T go = Object.Instantiate(prefab, parent);
        return go;
    }

    protected virtual void OnGet(T go)
    {
        go.transform.SetParent(null);
        go.gameObject.SetActive(true);
    }

    protected virtual void OnRelease(T go)
    {
        go.gameObject.SetActive(false);
        go.transform.SetParent(parent);
    }

    protected virtual void OnDestroy(T go)
    {
        if (go != null)
            Object.Destroy(go.gameObject);
    }
}
