using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RestAPIClass<T>
{
    public Action<T> callback;
    public Action<string> errorCallback;
    
    public bool isSuccess;
    public int code;
    public string message;
    public string httpStatus;
    public T data;
    public string[] error;
    
    public void SetValue(RestAPIClass<T> restAPIClass)
    {
        isSuccess = restAPIClass.isSuccess;
        code = restAPIClass.code;
        message = restAPIClass.message;
        httpStatus = restAPIClass.httpStatus;
        data = restAPIClass.data;
        error = restAPIClass.error;
    }
}
