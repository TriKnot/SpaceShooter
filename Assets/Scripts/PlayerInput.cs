using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ShipEngine _shipEngine;
    
    private PlayerInputActions _playerInputActions;
    
    // Actions
    private InputAction _move;
    private InputAction _look;
    private InputAction _fire;
    private InputAction _stop;
    
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Enable the actions
        
        if(_shipEngine)
        {
            _move = _playerInputActions.Player.Move;
            _move.Enable();
            _move.performed += OnMove;
            _move.canceled += OnMove;

            _look = _playerInputActions.Player.Look;
            _look.Enable();
            _look.performed += OnLook;
            _look.canceled += OnLook;

            _stop = _playerInputActions.Player.Stop;
            _stop.Enable();
            _stop.started += OnStop;
            _stop.canceled += OnStop;
        }
        
        _fire = _playerInputActions.Player.Fire;
        _fire.Enable();
        _fire.performed += OnFire;
        
    }


    private void OnDisable()
    {
        // Show and unlock the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Disable the actions
        
        if(_shipEngine)
        {
            _move.Disable();
            _move.performed -= OnMove;
            _move.canceled -= OnMove;
            
            _look.Disable();
            _look.performed -= OnLook;
            _look.canceled -= OnLook;
            
            _stop.Disable();
            _stop.performed -= OnStop;
            _stop.canceled -= OnStop;
        }
        _fire.performed -= OnFire;
    }
    
    private void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire");
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _shipEngine.SetThrust(input.y);
        _shipEngine.SetRoll(input.x);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _shipEngine.SetPitch(input.y);
        _shipEngine.SetYaw(input.x);
    }
    
    private void OnStop(InputAction.CallbackContext context)
    {
        _shipEngine.ShouldDampenCurrentValues = context.started;
        Debug.Log(context);
    }
}