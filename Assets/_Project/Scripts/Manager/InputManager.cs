using _Project.Scripts.EventStructs;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    private InputAction _point;
    private InputAction _click;
    
    private void Start()
    {
        InputActionMap playerActionMap = InputSystem.actions.FindActionMap("Player");
        
        InputActionMap uiActionMap = InputSystem.actions.FindActionMap("UI");
        _point = uiActionMap.FindAction("Point");
        _click = uiActionMap.FindAction("Click");
        
        _point.performed += OnPoint;
        _click.performed += OnClick;
    }

    private void OnDestroy()
    {
        _point.performed -= OnPoint;
        _click.performed -= OnClick;
    }

    private void OnPoint(InputAction.CallbackContext context) => OnInputActionPerformed<InputStructs.Point>(context);
    private void OnClick(InputAction.CallbackContext context) => OnInputActionPerformed<InputStructs.Click>(context);

    private void OnInputActionPerformed<T>(InputAction.CallbackContext context) where T : struct, IInputEvent
    {
        T t = new();
        t.Init(context);
        EventHub.Instance?.RaiseEvent(t);
    }
}
