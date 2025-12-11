using _Project.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleDialogPanel : MonoBehaviour
{
    public Transform canvas;
    public DialogPanel dialogPrefab;

    private DialogPanel _dialogInstance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_dialogInstance)
                return;
            
            _dialogInstance = Instantiate(dialogPrefab, canvas);
            _dialogInstance.Initialize(null, "Name", "some text some text <color=\"red\">some text</color> some text some text some text ", 100, () => Debug.Log("다이얼 로그 종료"));
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!_dialogInstance)
                return;
            _dialogInstance?.SkipStreamText();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_dialogInstance)
                return;
            _dialogInstance?.EndDialog();
        }
    }
}
