using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FE = _Project.Scripts.EventStructs.FirebaseEvents;
using DS = _Project.Scripts.EventStructs.DataStructs;
using IGS = _Project.Scripts.EventStructs.InGameStructs;

namespace _Project.Scripts.InGame.Sample
{
    public class SampleLogin : MonoBehaviour
    {
        [SerializeField] private string email;
        [SerializeField] private string password;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClickLoginButton);
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         List<DonutInstanceData> donuts = new List<DonutInstanceData>();
        //         DataManager.Instance.TryGetDonutData("10000001", out var donut);
        //         donuts.Add(new DonutInstanceData(donut));
        //         donuts.Add(new DonutInstanceData(donut));
        //         donuts.Add(new DonutInstanceData(donut));
        //         donuts.Add(new DonutInstanceData(donut));
        //         donuts.Add(new DonutInstanceData(donut));
        //         EventHub.Instance.RaiseEvent(new DS.RequestSetDeckDonutEvent(donuts));
        //     }
        // }

        private void OnDestroy()
        {
            EventHub.Instance?.UnRegisterEvent<FE.FirebaseLoginSuccess>(OnLoginSuccess);
            EventHub.Instance?.UnRegisterEvent<FE.FirebaseRegisterSuccess>(OnCreateSuccess);
        }

        private void OnClickLoginButton()
        {
            EventHub.Instance?.RegisterEvent<FE.FirebaseLoginSuccess>(OnLoginSuccess);
            // EventHub.Instance?.RegisterEvent<FE.FirebaseRegisterSuccess>(OnCreateSuccess);
            EventHub.Instance?.RaiseEvent(new FE.RequestLoadTableData());
            // EventHub.Instance?.RaiseEvent(new FE.RequestCreateEmailEvent("tester00@gmail.com", "tester00", "tester00"));
            EventHub.Instance?.RaiseEvent(new FE.RequestSignInWithEmailEvent(email, password));
            Debug.Log("로그인 성공");
            gameObject.SetActive(false);
        }

        private void OnCreateSuccess(FE.FirebaseRegisterSuccess evt)
        {
            EventHub.Instance.RaiseEvent(new DS.RequestCreateNewDataEvent(evt.uid));
        }

        private void OnLoginSuccess(FE.FirebaseLoginSuccess evt)
        {
            
        }
    }
}
