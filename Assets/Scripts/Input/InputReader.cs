using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    private GameInput gameInput;
    
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction ShootEvent = delegate { };
    public event UnityAction WriteEvent = delegate { };
    public event UnityAction SendEvent = delegate { };

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) { ShootEvent.Invoke(); }
    }
    public void OnWrite(InputAction.CallbackContext context)
    {
        if (context.performed) { WriteEvent.Invoke(); }
    }
    public void OnSend(InputAction.CallbackContext context)
    {
        if (context.performed) { SendEvent.Invoke(); }
    }

    private void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
            gameInput.Gameplay.Enable();
        }
    }
}