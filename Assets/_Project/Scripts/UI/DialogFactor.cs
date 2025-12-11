using _Project.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFactor
{
    public DialogPanel dialogPrefab;
    public Sprite iconSprite;
    public string iconStr;
    public string contentStr;
    public int typeDelay;
    public Action endAction;

    public DialogFactor(DialogPanel dialogPrefab, Sprite iconSprite, string iconStr, string contentStr, int typeDelay = 10, Action endAction = null)
    {
        this.dialogPrefab = dialogPrefab;
        this.iconSprite = iconSprite;
        this.iconStr = iconStr;
        this.contentStr = contentStr;
        this.typeDelay = typeDelay;
        this.endAction = endAction;
    }
}
