using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.EventStructs;
using UnityEngine;

/// <summary>
/// <see cref="EventHub"/>샘플용 구조체 구현 스크립트 <br/>
/// 추후 제거 예정
/// </summary>
public static class SampleStructs
{
    public struct SampleStruct : IEvent { }

    public struct SampleStruct2 : IEvent
    {
        public int data1;
        public float data2;
        public string data3;

        public SampleStruct2(int data1, float data2, string data3)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
        }
    }
}
