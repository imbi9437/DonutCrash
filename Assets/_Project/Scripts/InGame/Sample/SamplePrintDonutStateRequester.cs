using System.Text;
using UnityEngine;

using US = _Project.Scripts.EventStructs.UIStructs;

namespace _Project.Scripts.InGame.Sample
{
    public class SamplePrintDonutStateRequester : MonoBehaviour
    {
        private DonutObject _donutObject;
        private StringBuilder _sb;

        private void Start()
        {
            _donutObject = GetComponent<DonutObject>();
            _sb = new StringBuilder();
        }

        private void OnMouseEnter()
        {
            Debug.Log("상태 출력");
            _sb.Clear();
            _sb.AppendLine($"DonutObject : {_donutObject.name}");
            _sb.AppendLine($"InstanceUid : {_donutObject.GetUid()}");
            _sb.AppendLine($"Atk : {_donutObject.GetAtk()}");
            _sb.AppendLine($"Def : {_donutObject.GetDef()}");
            _sb.AppendLine($"SlingShotPower : {_donutObject.GetSlingShotPower()}");
            // EventHub.Instance?.RaiseEvent(new US.PrintDonutStateEvent(_sb.ToString(), Input.mousePosition));
        }

        private void OnMouseExit()
        {
            EventHub.Instance?.RaiseEvent(new US.PrintDonutStateEvent());
        }
    }
}
