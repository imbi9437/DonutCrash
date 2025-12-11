using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            var data = DataManager.Instance.User;
            EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestSaveUserData(data));
        }
    }
}
