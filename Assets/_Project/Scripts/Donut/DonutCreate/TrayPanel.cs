using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;
public class TrayPanel : MonoBehaviour
{
    [SerializeField] private Button createBtn;
    [SerializeField] private List<TraySlot> donutInfos = new();
    [SerializeField] private GameObject requireGoldBar;

    public TextMeshProUGUI trayGradeText;
    void Start()
    {
        EventHub.Instance.RegisterEvent<RES.OnTrayPanleViewEvent>(OnPanelRapper);
        EventHub.Instance.RegisterEvent<RES.AddToTrayViewEvent>(PlacedDonutRapper);
        EventHub.Instance.RegisterEvent<RES.CanCreateDonutViewEvnet>(OnCreateButtonRapper);
        EventHub.Instance.RegisterEvent<RES.RefreshGoldViewEvent>(RefreshGoldTextRapper);

        gameObject.SetActive(false);
        requireGoldBar.SetActive(false);
    }
    void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<RES.OnTrayPanleViewEvent>(OnPanelRapper);
        EventHub.Instance?.UnRegisterEvent<RES.AddToTrayViewEvent>(PlacedDonutRapper);
        EventHub.Instance?.UnRegisterEvent<RES.CanCreateDonutViewEvnet>(OnCreateButtonRapper);
        EventHub.Instance?.UnRegisterEvent<RES.RefreshGoldViewEvent>(RefreshGoldTextRapper);
    }

    void OnDisable()
    {
        ResetTray();
    }
    private void OnPanelRapper(RES.OnTrayPanleViewEvent evt) => OnPanel();
    private void PlacedDonutRapper(RES.AddToTrayViewEvent evt) => PlacedDonut(evt.donutInfo, evt.requiredCount, evt.slotNumber, evt.requiredGold, evt.trayGrade);
    private void OnCreateButtonRapper(RES.CanCreateDonutViewEvnet evt) => OnCreateButton(evt.canCreate);
    private void RefreshGoldTextRapper(RES.RefreshGoldViewEvent evt) => RefreshGoldText(evt.requiredGold);


    /// <summary> Tray패널이 활성화 되었을 때 <summary>
    private void OnPanel()
    {
        SetTray();
        EventHub.Instance.RaiseEvent(new DCS.TrayClearControlEvnet());
    }
    /// <summary> Tray패널 최초 세팅 <summary>
    private void SetTray()
    {
        int num = 0;
        foreach (var r in donutInfos)
        {
            r.selectedDonutBtn.onClick.RemoveAllListeners();
            r.cancelBtn.onClick.RemoveAllListeners();
            r.selectedDonutBtn.onClick.AddListener(() => OnClickSelectedDonut(r.trayNum));
            r.cancelBtn.onClick.AddListener(() => OnClickCancelButton(r.trayNum));
            r.trayNum = num;
            num++;
            ResetSlot(r);
        }
        trayGradeText.text = $"도넛 최대 중첩 개수 : {DataManager.Instance.TrayData.grade}";
        createBtn.interactable = false;
        requireGoldBar.GetComponentInChildren<TextMeshProUGUI>().text = "";
        requireGoldBar.SetActive(false);
        createBtn.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
        createBtn.onClick.RemoveAllListeners();
        createBtn.onClick.AddListener(OnClickDonutCreatButton);
    }
    /// <summary> 트레이패널 초기화 <summary>
    private void ResetTray()
    {
        foreach (var r in donutInfos)
        {
            ResetSlot(r);
        }
        gameObject.SetActive(false);
    }
    /// <summary> 슬롯 초기화 <summary>
    void ResetSlot(TraySlot slot)
    {
        slot.transform.Find("DefaultDonut").gameObject.SetActive(false);
        slot.donutNameText.text = $"빈 슬롯";
        slot.donutNameText.color = Color.black;
        slot.donutCountText.gameObject.SetActive(false);
        createBtn.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
    }
    /// <summary> 트레이에 도넛이 올라갔을 때 <summary>
    private void PlacedDonut(DonutData donutData, int requiredCount, int slotNum, int requiredGold, int trayGrade)
    {
        var slot = donutInfos.Find(x => x.trayNum == slotNum);

        slot.donutNameText.text = $"{donutData.donutName} x {requiredCount}";
        slot.donutNameText.color = Color.red;

        slot.transform.Find("DefaultDonut").gameObject.SetActive(true);
        AddressableLoader.AssetLoadByPath<Sprite>(donutData.resourcePath, x =>
        {
            slot.transform.Find("DefaultDonut/Frame Out/Frame In/Icon").GetComponent<Image>().sprite = x;
        }).Forget();
        slot.donutCountText.gameObject.SetActive(true);

        slot.donutCountText.text = $"x{requiredCount}";

        trayGradeText.text = $"트레이 중첩 : {trayGrade}";

        RefreshGoldText(requiredGold);

    }

    /// <summary> 트레이버튼 리스너 <summary>
    private void OnClickSelectedDonut(int slotNum)
    {
        EventHub.Instance.RaiseEvent(new DCS.SelectedDonutControlEvent(slotNum));
    }


    /// <summary> 도넛 제작버튼 활성화 여부 <summary>
    private void OnCreateButton(bool canCreate)
    {
        createBtn.interactable = canCreate;
    }
    /// <summary> 도넛 제작버튼 리스너 <summary>
    private void OnClickDonutCreatButton()
    {
        ResetTray();
        EventHub.Instance.RaiseEvent(new DCS.StartBakingControlEvent());
    }
    /// <summary> 트레이에 올라가있는 도넛 취소 버튼 리스너 <summary>
    private void OnClickCancelButton(int slotNum)
    {
        if (donutInfos[slotNum] == null) return;
        ResetSlot(donutInfos[slotNum]);

        EventHub.Instance.RaiseEvent(new DCS.OnTrayDonutCancelControlEvent(slotNum));
    }
    /// <summary> 소모골드 표시 <summary>
    private void RefreshGoldText(int requiredGold)
    {
        Debug.Log($"Refresh {requiredGold}");
        if (requiredGold <= 0)
        {
            requireGoldBar.SetActive(false);
            return;
        }

        requireGoldBar.SetActive(true);
        requireGoldBar.GetComponentInChildren<TextMeshProUGUI>().text = $"{requiredGold}";
        createBtn.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
    }

}
