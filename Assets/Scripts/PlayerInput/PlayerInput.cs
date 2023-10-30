using Ship;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerInput
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ShipEngine _shipEngine;
        [SerializeField] private WeaponSystem _weaponSystem;

        private PlayerInputActions _playerInputActions;

        private InputAction _move;
        private InputAction _look;
        private InputAction _fire;
        private InputAction _stop;
        private InputAction _quit;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            ConfigureCursorState(lockCursor: true);

            if (_shipEngine)
            {
                ConfigureMoveActions();
                ConfigureLookActions();
                ConfigureStopAction();
            }
            if (_weaponSystem)
            {
                ConfigureFireAction();
            }

            ConfigureQuitAction();
        }

        private void OnDisable()
        {
            ConfigureCursorState(lockCursor: false);

            if (_shipEngine)
            {
                DisableMoveActions();
                DisableLookActions();
                DisableStopAction();
            }
            if (_weaponSystem)
            {
                DisableFireAction();
            }

            DisableQuitAction();
        }

        private void ConfigureCursorState(bool lockCursor)
        {
            Cursor.visible = !lockCursor;
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void ConfigureMoveActions()
        {
            _move = _playerInputActions.Player.Move;
            _move.Enable();
            _move.performed += OnMove;
            _move.canceled += OnMove;
        }

        private void DisableMoveActions()
        {
            _move.Disable();
            _move.performed -= OnMove;
            _move.canceled -= OnMove;
        }

        private void ConfigureLookActions()
        {
            _look = _playerInputActions.Player.Look;
            _look.Enable();
            _look.performed += OnLook;
            _look.canceled += OnLook;
        }

        private void DisableLookActions()
        {
            _look.Disable();
            _look.performed -= OnLook;
            _look.canceled -= OnLook;
        }

        private void ConfigureStopAction()
        {
            _stop = _playerInputActions.Player.Stop;
            _stop.Enable();
            _stop.started += OnStop;
            _stop.canceled += OnStop;
        }

        private void DisableStopAction()
        {
            _stop.Disable();
            _stop.started -= OnStop;
            _stop.canceled -= OnStop;
        }

        private void ConfigureFireAction()
        {
            _fire = _playerInputActions.Player.Fire;
            _fire.Enable();
            _fire.started += OnFire;
            _fire.canceled += OnFire;
        }

        private void DisableFireAction()
        {
            _fire.Disable();
            _fire.started -= OnFire;
            _fire.canceled -= OnFire;
        }

        private void ConfigureQuitAction()
        {
            _quit = _playerInputActions.Player.Quit;
            _quit.Enable();
            _quit.started += OnQuit;
        }

        private void DisableQuitAction()
        {
            _quit.Disable();
            _quit.started -= OnQuit;
        }

        private void OnQuit(InputAction.CallbackContext context)
        {
            SceneManager.LoadScene(0);
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            _weaponSystem.ShouldShoot = context.started;
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
        }
    }
}
