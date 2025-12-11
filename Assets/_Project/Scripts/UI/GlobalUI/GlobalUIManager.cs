using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using US = _Project.Scripts.EventStructs.UIStructs;

namespace DonutClash.UI.GlobalUI
{
    public enum GlobalPanelType
    {
        OneButtonPopup,
        TwoButtonPopup,
        ImageSelectPopup,
        SettingPopup,
    }

    /// <summary> 모든 씬에서 사용할 UI를 관리하는 코드 </summary>
    public class GlobalUIManager : MonoSingleton<GlobalUIManager>
    {
        private readonly Dictionary<GlobalPanelType, GlobalPanel> _globalPanels = new();
        
        [SerializeField] private List<GlobalPanel> panelPrefabs;
        [SerializeField] private Transform canvas;
        
        #region Unity Message Methods

        protected override void Awake()
        {
            base.Awake();
            
            panelPrefabs.ForEach(x => _globalPanels.TryAdd(x.GlobalPanelType, x));
        }

        private void Start()
        {
            EventHub.Instance?.RegisterEvent<US.RequestOpenGlobalPanel>(OnRequestOpenGlobalPanel);
            EventHub.Instance?.RegisterEvent<US.RequestCloseAllGlobalPanel>(OnRequestCloseAllGlobalPanel);
        }

        private void OnDestroy()
        {
            EventHub.Instance?.UnRegisterEvent<US.RequestOpenGlobalPanel>(OnRequestOpenGlobalPanel);
            EventHub.Instance?.UnRegisterEvent<US.RequestCloseAllGlobalPanel>(OnRequestCloseAllGlobalPanel);
        }

        #endregion
        
        #region Event Wrapper Methods

        private void OnRequestOpenGlobalPanel(US.RequestOpenGlobalPanel evt) => OpenPanel(evt.type, evt.param);
        private void OnRequestCloseAllGlobalPanel(US.RequestCloseAllGlobalPanel evt) => CloseAllPanels();
        
        #endregion

        private void OpenPanel(GlobalPanelType panelType, GlobalPanelParam parm)
        {
            GlobalPanel gp = Instantiate(_globalPanels[panelType], canvas);
            gp.Initialize();
            gp.Show(parm);
        }

        private void CloseAllPanels()
        {
            foreach (GlobalPanel i in canvas.GetComponentsInChildren<GlobalPanel>())
            {
                i.Hide();
            }
        }
    }
}