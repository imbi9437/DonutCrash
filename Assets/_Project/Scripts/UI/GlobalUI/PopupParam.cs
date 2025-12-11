using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupParam 
{
    public string titleText;
    public string contentText;

    public Action ok;
    public Action cancel;

    public PopupParam(string titleText, string contentText , Action ok , Action cancel) //2번튼용 
    {
        this.titleText = titleText;
        this.contentText = contentText;
        
        this.ok = ok;
        this.cancel = cancel;
    }
    
    public PopupParam(string titleText, string contentText) //1버튼용 
    {
        this.titleText = titleText;
        this.contentText = contentText;
    }
}
