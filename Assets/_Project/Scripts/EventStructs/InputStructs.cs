using UnityEngine.InputSystem;

namespace _Project.Scripts.EventStructs
{
    public interface IInputEvent : IEvent
    {
        public void Init(InputAction.CallbackContext context);
    }
    public static class InputStructs
    {
        public struct Point : IInputEvent
        {
            public InputAction.CallbackContext context;
            
            public void Init(InputAction.CallbackContext context) => this.context = context;
        }

        public struct Click : IInputEvent
        {
            public InputAction.CallbackContext context;

            public void Init(InputAction.CallbackContext context) => this.context = context;
        }
    }
}
