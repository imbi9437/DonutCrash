using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingInfoItem : MonoBehaviour
{
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private TMP_Text nicknameText;

    [SerializeField] private Image providerImage;
    [SerializeField] private TMP_Text providerText;
    
    [SerializeField] private Button switchCredentialButton;
    [SerializeField] private TMP_Text switchCredentialText;

    [Header("임시 캐싱")]
    [SerializeField] ProviderInfo guestProviderInfo;
    [SerializeField] ProviderInfo gpgsProviderInfo;
    
    
    private void OnEnable()
    {
        versionText.text = $"{Application.version}";
        uidText.text = DataManager.Instance.UserUid;
        nicknameText.text = DataManager.Instance.UserNickName;

        if (FirebaseManager.Instance.IsAnonymous)
        {
            providerImage.sprite = guestProviderInfo.icon;
            providerText.text = guestProviderInfo.name;
            switchCredentialButton.gameObject.SetActive(true);
        }
        else
        {
            providerImage.sprite = gpgsProviderInfo.icon;
            providerText.text = gpgsProviderInfo.name;
            switchCredentialButton.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        switchCredentialButton.onClick.AddListener(OnClickSwitchCredentialButton);
        
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseSwitchCredentialSuccess>(OnSwitchedProvider);
        EventHub.Instance.RegisterEvent<FirebaseEvents.FirebaseSwitchCredentialFailed>(OnFailedSwitchProvider);
    }

    private void OnDestroy()
    {
        switchCredentialButton.onClick.RemoveListener(OnClickSwitchCredentialButton);
        
        EventHub.Instance?.UnRegisterEvent<FirebaseEvents.FirebaseSwitchCredentialSuccess>(OnSwitchedProvider);
        EventHub.Instance?.UnRegisterEvent<FirebaseEvents.FirebaseSwitchCredentialFailed>(OnFailedSwitchProvider);
    }


    private void OnClickSwitchCredentialButton()
    {
        switchCredentialButton.interactable = false;
        switchCredentialText.text = "전환 중";
        
        EventHub.Instance.RaiseEvent(new FirebaseEvents.RequestSwitchCredentialEvent());
    }

    private void OnSwitchedProvider(FirebaseEvents.FirebaseSwitchCredentialSuccess evt)
    {
        switchCredentialButton.gameObject.SetActive(false);
        
        providerImage.sprite = gpgsProviderInfo.icon;
        providerText.text = gpgsProviderInfo.name;

        var param = new OneButtonParam("성공", "계정 전환에 성공했습니다.", "확인");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup,param));

        switchCredentialButton.interactable = true;
    }
    
    private void OnFailedSwitchProvider(FirebaseEvents.FirebaseSwitchCredentialFailed evt)
    {
        var param = new OneButtonParam("실패", "계정 전환에 실패했습니다.", "확인");
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.OneButtonPopup,param));
        
        switchCredentialButton.interactable = true;
        switchCredentialText.text = "계정 전환";
    }
}

[Serializable]
public class ProviderInfo
{
    public Sprite icon;
    public string name;
}