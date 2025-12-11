using TMPro;
using UnityEngine;
using WebSocketSharp;
using US = _Project.Scripts.EventStructs.UIStructs;

namespace _Project.Scripts.InGame.Sample
{
    public class SampleDonutStatePrinter : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private void Start()
        {
            if (text)
                text = GetComponentInChildren<TextMeshProUGUI>();
            
            EventHub.Instance?.RegisterEvent<US.PrintDonutStateEvent>(OnPrintDonutStateEvent);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventHub.Instance?.UnRegisterEvent<US.PrintDonutStateEvent>(OnPrintDonutStateEvent);
        }

        private void OnPrintDonutStateEvent(US.PrintDonutStateEvent evt)
        {
            // transform.position = new Vector3(evt.position.x, evt.position.y, 0);
            // if (evt.message.IsNullOrEmpty())
            //     gameObject.SetActive(false);
            // else
            //     SetText(evt.message);
        }

        private void SetText(string message)
        {
            gameObject.SetActive(true);
            text.text = message;
        }
    }
}
