using _Project.Scripts.EventStructs;
using DonutClash.UI.GlobalUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileArea : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text nameText;
    
    [SerializeField] private Button profileButton;
    
    private Coroutine _profileCoroutine;

    private void OnEnable()
    {
        SetNickname();
        SetProfileImage();
        
        profileButton.onClick.AddListener(ShowSelectPanel);
        profileButton.interactable = true;
        
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserProfileImageEvent>(OnBroadcast);
        EventHub.Instance.RegisterEvent<DataStructs.BroadcastSetUserNicknameEvent>(OnBroadcast);
    }

    private void OnDisable()
    {
        profileButton.onClick.RemoveListener(ShowSelectPanel);
        
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserProfileImageEvent>(OnBroadcast);
        EventHub.Instance?.UnRegisterEvent<DataStructs.BroadcastSetUserNicknameEvent>(OnBroadcast);

        if (_profileCoroutine != null)
        {
            StopCoroutine(_profileCoroutine);
            _profileCoroutine = null;
        }
    }

    private void OnBroadcast(DataStructs.BroadcastSetUserNicknameEvent evt) => SetNickname();
    private void OnBroadcast(DataStructs.BroadcastSetUserProfileImageEvent evt) => SetProfileImage();
    
    
    private void SetNickname() => nameText.text = DataManager.Instance.UserNickName;

    private void SetProfileImage()
    {
        string bakerUid = DataManager.Instance.UserProfileBaker;

        if (string.IsNullOrWhiteSpace(bakerUid))
        {
            List<BakerInstanceData> bakers = DataManager.Instance.Bakers;
            if (bakers.Count == 0)
            {
                avatarImage.sprite = null;
                avatarImage.color = Color.clear;
                return;
            }
            
            BakerInstanceData baker = bakers.Find(x => x.origin == "10100007");
            bakerUid = baker?.origin ?? bakers[0].origin;
            
            var evt = new DataStructs.RequestSetProfileImageEvent(bakerUid);
            EventHub.Instance.RaiseEvent(evt);
        }
        
        avatarImage.color = Color.white;
        DataManager.Instance.TryGetBakerData(bakerUid, out var origin);
        AddressableLoader.AssetLoadByPath<Sprite>(origin.resourcePath, x => avatarImage.sprite = x).Forget();
    }

    private void ShowSelectPanel()
    {
        profileButton.interactable = false;
        string current = DataManager.Instance.UserProfileBaker;
        var list = DataManager.Instance.Bakers.Select(d => d.origin).OrderBy(d => d).ToList();
        
        var param = new ImageSelectParam("프로필 이미지 선택", current, list, "변경하기", s =>
        {
            var evt = new DataStructs.RequestSetProfileImageEvent(s);
            EventHub.Instance.RaiseEvent(evt);
        });
        
        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.ImageSelectPopup, param));

        _profileCoroutine = StartCoroutine(InteractiveProfileAfterSecondsCoroutine(1f));
    }

    private IEnumerator InteractiveProfileAfterSecondsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        profileButton.interactable = true;
    }
}
