using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableLoader
{
    //개선용
    private static readonly Dictionary<string, AsyncOperationHandle> PathHandle = new();

    public static async UniTaskVoid AssetLoadByPath<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.Log($"<color=yellow>[AddressableLoader] Path is Empty</color>");
            return;
        }
        
        string typePath = $"{typeof(T).Name}/{path}";
        
        try
        {
            // check whether there is same path handle
            if (PathHandle.TryGetValue(typePath, out AsyncOperationHandle handle) == false)
            {
                handle = Addressables.LoadAssetAsync<T>(typePath);
                PathHandle.Add(typePath, handle);
            }
            
            // whatever the handle was reused or newly generated, wait for completing load.
            await handle.Task.AsUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"<color=green>[AddressableLoader] Asset Loaded (Type : {typeof(T)})</color>");
                callback?.Invoke(handle.Result as T);
            }
            else
            {
                Debug.LogError($"<color=red>[AddressableLoader] Asset Load Failed (Type : {typeof(T)})</color>");
                handle.Release();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>[AddressableLoader] Load Failed : {e.Message}</color>");
        }
    }
    
    public static void ReleaseAllPathHandle()
    {
        foreach (AsyncOperationHandle i in PathHandle.Values)
        {
            i.Release();
        }
        
        PathHandle.Clear();
    }
}
