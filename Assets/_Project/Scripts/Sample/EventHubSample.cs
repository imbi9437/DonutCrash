using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.EventStructs;
using UnityEngine;

public class EventHubSample : MonoBehaviour
{
    private void OnEnable() // 함수 등록
    {
        EventHub.Instance.RegisterEvent<SampleStructs.SampleStruct>(SampleFunction);
        EventHub.Instance.RegisterEvent<SampleStructs.SampleStruct2>(SampleFunction2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //등록 함수 수행
        {
            EventHub.Instance.RaiseEvent(new SampleStructs.SampleStruct());
            EventHub.Instance.RaiseEvent(new SampleStructs.SampleStruct2(1, 2, "3"));
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            EventHub.Instance.RaiseEvent(new ChangeSceneStructs.RequestChangeSceneEvent("04.StageSample",isDirect:false));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EventHub.Instance.RaiseEvent(new ChangeSceneStructs.RequestChangeSceneEvent("04.StageSample", isDirect:true));
        }
    }

    private void OnDisable() // 함수 등록 해제
    {
        EventHub.Instance.UnRegisterEvent<SampleStructs.SampleStruct>(SampleFunction);
        EventHub.Instance.UnRegisterEvent<SampleStructs.SampleStruct2>(SampleFunction2);
    }
    

    private void SampleFunction(SampleStructs.SampleStruct s)
    {
        Debug.Log("SampleStruct");
    }

    private void SampleFunction2(SampleStructs.SampleStruct2 s)
    {
        Debug.Log("SampleStruct2");
        Debug.Log(s.data1);
        Debug.Log(s.data2);
        Debug.Log(s.data3);
    }
}
