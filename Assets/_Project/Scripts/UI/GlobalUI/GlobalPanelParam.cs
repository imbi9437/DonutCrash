using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalPanelParam
{
}

public class OneButtonParam : GlobalPanelParam
{
    public string titleText;
    public string contentText;

    public string confirmText;

    public Action confirm;

    public OneButtonParam(string titleText, string contentText, string confirmText = null, Action confirm = null)
    {
        this.titleText = titleText;
        this.contentText = contentText;

        this.confirmText = confirmText;
        this.confirm = confirm;
    }
}

public class TwoButtonParam : GlobalPanelParam
{
    public string titleText;
    public string contentText;

    public string confirmText;
    public string cancelText;

    public Action confirm;
    public Action cancel;

    public TwoButtonParam(string titleText, string contentText, string confirmText = null, string cancelText = null,
        Action confirm = null, Action cancel = null)
    {
        this.titleText = titleText;
        this.contentText = contentText;

        this.confirmText = confirmText;
        this.cancelText = cancelText;

        this.confirm = confirm;
        this.cancel = cancel;
    }
}

public class ImageSelectParam : GlobalPanelParam
{
    public string titleText;
    public string currentPath;
    public List<string> spritePath;

    public string confirmText;
    public Action<string> confirm;

    public ImageSelectParam(string titleText, string currentPath, List<string> spritePath, string confirmText = null,
        Action<string> confirm = null)
    {
        this.titleText = titleText;
        this.currentPath = currentPath;
        this.spritePath = spritePath;

        this.confirmText = confirmText;
        this.confirm = confirm;
    }
}

public class SettingParam : GlobalPanelParam
{
}