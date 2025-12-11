using _Project.Scripts.InGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private InGameOwner owner;

    private void Start()
    {
        EventHub.Instance?.RegisterEvent<IGS.RequestSetUserProfile>(OnRequestSetUserProfile);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.RequestSetUserProfile>(OnRequestSetUserProfile);
    }

    private void OnRequestSetUserProfile(IGS.RequestSetUserProfile evt) => Setup(evt.user, evt.nickName, evt.bakerUid);

    private void Setup(InGameOwner user, string nickName, string bakerUid)
    {
        if (user != owner)
            return;
        
        nickNameText.text = nickName;

        if (bakerUid.IsNullOrEmpty() == false)
        {
            if (DataManager.Instance.TryGetBakerData(bakerUid, out var bakerData) == false)
                return;

            AddressableLoader.AssetLoadByPath<Sprite>(bakerData.resourcePath, x =>
            {
                if (iconImage != null)
                    iconImage.sprite = x;
            }).Forget();
        }
    }
}
