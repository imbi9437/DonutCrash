using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private RectTransform progressBar;

    private Vector2 _anchorMax = Vector2.up;

    public float Value
    {
        get => _anchorMax.x;
        set
        {
            _anchorMax.x = value;
            progressBar.anchorMax = _anchorMax;
        }
    }
}
