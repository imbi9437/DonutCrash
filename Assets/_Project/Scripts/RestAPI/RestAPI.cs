using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Web 통신용 static 클래스
/// </summary>
public static class RestAPI
{
    private const double timeout = 10;
    
    private static async UniTaskVoid RequestAsync<T>(UnityWebRequest request, RestAPIClass<T> reVal)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

        try
        {
            var res = await request.SendWebRequest().WithCancellation(cts.Token);
            
            var result = res.downloadHandler.data;
            var message = Encoding.UTF8.GetString(result);

            RestAPIClass<T> convert = JsonConvert.DeserializeObject<RestAPIClass<T>>(message);
            
            reVal.SetValue(convert);
            reVal.callback?.Invoke(reVal.data);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(request.responseCode);
            reVal.errorCallback?.Invoke(e.Message);
        }
        
        request.Dispose();
    }
    
    public static RestAPIClass<T> Get<T>(string uri)
    {
        var url = $"{uri}";
        RestAPIClass<T> reVal = new RestAPIClass<T>();

        //byte[] bodyRow = null;
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        
        // Token Set
        // reqeust.SetRequestHeader("Authorization",token)
        
        request.SetRequestHeader("Content-Type","application/json");
        
        RequestAsync(request,reVal).Forget();
        
        return reVal;
    }
    
    public static RestAPIClass<T> Post<T>(string uri, string data)
    {
        var url = $"{uri}";
        RestAPIClass<T> reVal = new RestAPIClass<T>();

        byte[] bodyRow = null;
        if (data != null) bodyRow = Encoding.UTF8.GetBytes(data);
        
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRow);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        // Token Set
        // reqeust.SetRequestHeader("Authorization",token)
        
        request.SetRequestHeader("Content-Type","application/json");
        RequestAsync(request, reVal).Forget();

        return reVal;
    }
    
    public static RestAPIClass<string> Put(string url)
    {
        return null;
    }
    
    public static RestAPIClass<string> Delete(string url)
    {
        return null;
    }
}
