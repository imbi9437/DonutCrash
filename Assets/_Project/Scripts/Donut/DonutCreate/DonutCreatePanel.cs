using _Project.Scripts.EventStructs;
using Cysharp.Threading.Tasks;
using DonutClash.UI.GlobalUI;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DCS = _Project.Scripts.EventStructs.DonutControlStructs;
using RES = _Project.Scripts.EventStructs.RecipeEventStructs;

public class DonutCreatePanel : MonoBehaviour
{
    [SerializeField] private Image ovenImage;
    [SerializeField] private Sprite defaultOven;
    [SerializeField] private Sprite startOven;
    [SerializeField] private Sprite endOven;

    [SerializeField] private Button getDonutBtn;


    [SerializeField] private Button openTrayPanelBtn;

    public GameObject donutCreatePanel;
    public RectTransform donutRecipeArea;
    public GameObject donutPrefab;

    public GameObject trayPanel;

    public GameObject bakingTimeBar;
    public TextMeshProUGUI bakingTimeText;
    //도넛 생성할때 걸릴 시간에 대한 변수
    private bool isBaking;
    private CancellationTokenSource cts;

    void Start()
    {
        //EventHub.Instance.RegisterEvent<RES.OnSelectedDonutViewEvent>(SelectedDonutPanelRapper);
        EventHub.Instance.RegisterEvent<RES.CreatePanelCloseViewEvent>(OnClickDonutSelectButtonRapper);
        EventHub.Instance.RegisterEvent<RES.StartBakingViewEvent>(StartBakingRapper);
        EventHub.Instance.RegisterEvent<RES.OnDonutSelectPanelViewEvent>(OnDonutSelectPanelRapper);

        openTrayPanelBtn.onClick.RemoveAllListeners();
        openTrayPanelBtn.onClick.AddListener(OnClickOpenTrayPanel);
        getDonutBtn.onClick.RemoveAllListeners();
        getDonutBtn.onClick.AddListener(OnClickGetDonut);

        getDonutBtn.gameObject.SetActive(false);
        bakingTimeBar.SetActive(false);
        donutCreatePanel.gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        //EventHub.Instance?.UnRegisterEvent<RES.OnSelectedDonutViewEvent>(SelectedDonutPanelRapper);
        EventHub.Instance?.UnRegisterEvent<RES.CreatePanelCloseViewEvent>(OnClickDonutSelectButtonRapper);
        EventHub.Instance?.UnRegisterEvent<RES.StartBakingViewEvent>(StartBakingRapper);
        EventHub.Instance?.UnRegisterEvent<RES.OnDonutSelectPanelViewEvent>(OnDonutSelectPanelRapper);
        openTrayPanelBtn.onClick.RemoveAllListeners();
    }

    //private void SelectedDonutPanelRapper(RES.OnSelectedDonutViewEvent evt) => SelectedDonutPanel(evt.slotNumber, evt.recipeId, evt.donutName);
    private void OnClickDonutSelectButtonRapper(RES.CreatePanelCloseViewEvent evt) => OnClickDonutSelectButton();
    private void StartBakingRapper(RES.StartBakingViewEvent evt) => StartBaking(evt.startTime, evt.endTime);
    private void OnDonutSelectPanelRapper(RES.OnDonutSelectPanelViewEvent evt) => OnDonutSelectPanel();

    private void OnDisable()
    {
        donutCreatePanel.gameObject.SetActive(false);
    }

    private void OnClickOpenTrayPanel()
    {
        if (!trayPanel.gameObject.activeSelf) trayPanel.gameObject.SetActive(true);
        EventHub.Instance.RaiseEvent(new DCS.TrayPanelOpenControlEvent());
    }
    //제작 할 도넛을 선택후 제작 버튼을 눌렀을 경우 이벤트 호출
    private void OnClickDonutSelectButton()
    {
        donutCreatePanel.gameObject.SetActive(false);
    }
    private void OnDonutSelectPanel()
    {
        if (!donutCreatePanel.gameObject.activeSelf)
        {
            donutCreatePanel.gameObject.SetActive(true);
            foreach (Transform child in donutRecipeArea.gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void StartBaking(DateTime startTime, DateTime endTime)
    {
        trayPanel.gameObject.SetActive(false);
        ovenImage.sprite = startOven;
        openTrayPanelBtn.onClick.RemoveAllListeners();
        openTrayPanelBtn.onClick.AddListener(OnClickCancelCreating);
        openTrayPanelBtn.GetComponentInChildren<TextMeshProUGUI>().text = "제작 취소";
        //트레이 패널을 닫고, 기존 패널에서 도넛을 생성하기 위해 걸리는 시간 표시,
        //단순 시간만 표시해 줄 구조
        if (isBaking) return;
        bakingTimeBar.SetActive(true);
        isBaking = true;

        Debug.Log($"도넛 생산 시작: {startTime}");
        Debug.Log($"생산까지 남은 시간: {endTime}");

        cts?.Cancel();
        cts = new CancellationTokenSource();
        Timer(startTime, endTime, cts.Token).Forget();

    }


    async UniTaskVoid Timer(DateTime startTime, DateTime endTime, CancellationToken ct)
    {
        while (true)
        {
            var remaining = endTime - DateTime.Now;

            if (remaining.TotalSeconds <= 0 && !ct.IsCancellationRequested)
            {
                Debug.Log("도넛 완성!");
                ovenImage.sprite = endOven;
                isBaking = false;
                break;
            }

            bakingTimeText.text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            await UniTask.Delay(1000, cancellationToken: ct); // 1초마다 갱신
        }
        //도넛 생성 이벤트 호출
        EventHub.Instance.RaiseEvent(new RES.RequestOnSuccessMark(true));
        SuccessCreateDonut();
        //EventHub.Instance.RaiseEvent(new DCS.CreateDonutControlEvent());
    }

    private void SuccessCreateDonut()
    {
        getDonutBtn.gameObject.SetActive(true);
        openTrayPanelBtn.gameObject.SetActive(false);
        openTrayPanelBtn.onClick.RemoveAllListeners();
        openTrayPanelBtn.onClick.AddListener(OnClickOpenTrayPanel);
    }
    private void OnClickCancelCreating()
    {
        EventHub.Instance.RaiseEvent(new RES.RequestOnSuccessMark(false));
        TwoButtonParam param = new TwoButtonParam("제작 취소", $"제작을 취소하시겠습니까 ? <br>제작에 사용 된 도넛재료와 골드는 모두 반환됩니다.", "예", "아니요");
        param.confirm = () => aasd();
        //EventHub.Instance.RaiseEvent(new DCS.TrayClearControlEvnet());

        EventHub.Instance.RaiseEvent(new UIStructs.RequestOpenGlobalPanel(GlobalPanelType.TwoButtonPopup, param));
    }
    private void OnClickGetDonut()
    {
        EventHub.Instance.RaiseEvent(new RES.RequestOnSuccessMark(false));
        EventHub.Instance.RaiseEvent(new DCS.CreateDonutControlEvent());

        aasd();
    }
    private void aasd()
    {
        openTrayPanelBtn.onClick.RemoveAllListeners();
        openTrayPanelBtn.onClick.AddListener(OnClickOpenTrayPanel);
        getDonutBtn.gameObject.SetActive(false);
        openTrayPanelBtn.gameObject.SetActive(true);
        bakingTimeBar.SetActive(false);
        ovenImage.sprite = defaultOven;
        openTrayPanelBtn.GetComponentInChildren<TextMeshProUGUI>().text = "도넛 제작하기";
        EventHub.Instance.RaiseEvent(new DCS.TrayClearControlEvnet());
    }
}
