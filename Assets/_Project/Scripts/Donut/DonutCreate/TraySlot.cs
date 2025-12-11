using SFMTexture;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraySlot : MonoBehaviour
{
    public GameObject donutSlot;
    public TextMeshProUGUI donutNameText;
    public Button cancelBtn;

    public int trayNum;

    public TextMeshProUGUI donutCountText;
    public Button selectedDonutBtn;

    private void Awake()
    {
        donutCountText = transform.Find("Count").GetComponent<TextMeshProUGUI>();
        selectedDonutBtn = GetComponent<Button>();

        donutNameText = donutSlot.transform.Find("SelectedDonut/DonutName").GetComponent<TextMeshProUGUI>();
        cancelBtn = donutSlot.transform.Find("CancelButton").GetComponent<Button>();
    }

}
