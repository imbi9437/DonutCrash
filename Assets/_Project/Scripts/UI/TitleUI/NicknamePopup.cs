using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePopup : TitlePopup
{
    public override int PanelType => (int)TitlePanelType.SetNickname;
    
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Button confirmButton;
    
    private TitleUIController _titleUIController;

    private void OnEnable()
    {
        nicknameInput.SetTextWithoutNotify("");
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(SetNickname);
    }

    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(SetNickname);
    }

    private void SetNickname()
    {
        nicknameInput.interactable = false;
        confirmButton.interactable = false;
        
        string nickname = nicknameInput.text;

        if (string.IsNullOrWhiteSpace(nickname))
        {
            nicknameInput.interactable = true;
            confirmButton.interactable = true;
            var warningParam = new OneButtonParam("경고", "닉네임을 입력해 주세요", "확인");
            EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, warningParam));
            return;
        }
        
        EventHub.Instance.RaiseEvent(new DataStructs.RequestSetNickNameEvent(nickname));

        var param = new OneButtonParam("알림","닉네임 설정 완료", "확인");
        param.confirm = () => EventHub.Instance.RaiseEvent(new ChangeSceneStructs.RequestChangeSceneEvent("02.Main",-1,true));
        
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup, param));
    }
}
