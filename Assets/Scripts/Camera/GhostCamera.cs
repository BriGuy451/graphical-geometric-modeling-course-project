using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Add this class to a camera and you'll be able to pilot it using the horizontal/vertical axis, and up/down controls set via its inspector. 
/// It's got an activation button, a run button.
/// </summary>
[AddComponentMenu("Tools/Camera/GhostCamera")]
public class GhostCamera : MonoBehaviour
{
    [Header("Speed")]
    public float MovementSpeed = 10f;
    public float RunFactor = 4f;
    public float Acceleration = 5f;
    public float Deceleration = 5f;
    public float RotationSpeed = 40f;

    [Header("Controls")]
    #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        public InputAction HorizontalAction;
        public InputAction VerticalAction;
        public InputAction MousePositionAction;
        
        /// the button used to toggle the camera on/off
        public Key ActivateKey = Key.LeftShift;
        /// the button to use to go up
        public Key UpKey = Key.Space;
        /// the button to use to go down
        public Key DownKey = Key.C;
        /// the button to use to switch between mobile and desktop control mode
        public Key ControlsModeSwitchKey = Key.M;
        /// the button used to modify the timescale
        public Key TimescaleModificationKey = Key.F;
        /// the button used to run while it's pressed
        public Key RunKey = Key.RightShift;
    #else
        /// the button used to toggle the camera on/off
        public KeyCode ActivateButton = KeyCode.LeftShift;
        /// the name of the InputManager's horizontal axis
        public string HorizontalAxisName = "Horizontal";
        /// the name of the InputManager's vertical axis
        public string VerticalAxisName = "Vertical";
        /// the button to use to go up
        public KeyCode UpButton = KeyCode.Space;
        /// the button to use to go down
        public KeyCode DownButton = KeyCode.C;
        /// the button to use to switch between mobile and desktop control mode
        public KeyCode ControlsModeSwitch = KeyCode.M;
        /// the button used to modify the timescale
        public KeyCode TimescaleModificationButton = KeyCode.F;
        /// the button used to run while it's pressed
        public KeyCode RunButton = KeyCode.RightShift;
    #endif
    
    [Header("Mouse")]
    /// the mouse's sensitivity
    public float MouseSensitivity = 0.02f;

    [Header("Settings")]
    public bool AutoActivation = true;
    public bool MovementEnabled = true;
    public bool RotationEnabled = true;
    
    [ReadOnly]
    /// whether this camera is active or not right now
    public bool Active = false;

    protected Vector3 _currentInput;
    protected Vector3 _lerpedInput;
    protected Vector3 _normalizedInput;
    protected float _acceleration;
    protected float _deceleration;
    protected Vector3 _movementVector = Vector3.zero;
    protected float _speedMultiplier;
    protected Vector3 _newEulerAngles;
    protected Vector2 _mouseInput;

    /// <summary>
    /// On start, activate our camera if needed
    /// </summary>
    protected virtual void Start()
    {
        if (AutoActivation)
            ToggleFreeCamera();
        
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            HorizontalAction.Enable();
            VerticalAction.Enable();
            MousePositionAction.Enable();
            HorizontalAction.performed += context => _currentInput.x = context.ReadValue<float>();
            VerticalAction.performed += context => _currentInput.z = context.ReadValue<float>();
            MousePositionAction.performed += context => _mouseInput = context.ReadValue<Vector2>();
            HorizontalAction.canceled += context => _currentInput.x = 0f;
            VerticalAction.canceled += context => _currentInput.z = 0f;
            MousePositionAction.canceled += context => _mouseInput = Vector2.zero;
        #endif
    }

    /// <summary>
    /// On Update we grab our input and move accordingly
    /// </summary>
    protected virtual void Update()
    {
        if (!Active)
            return;

        GetInput();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Translate();
            Rotate();
            Move();
        }
    }

    /// <summary>
    /// Grabs and stores the various input values
    /// </summary>
    protected virtual void GetInput()
    {
        #if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
            _currentInput.x = Input.GetAxis("Horizontal");
            _currentInput.y = 0f;
            _currentInput.z = Input.GetAxis("Vertical");
            _mouseInput.x = Input.GetAxis("Mouse X");
            _mouseInput.y = Input.GetAxis("Mouse Y");
        #endif
    
        bool upButton = false;
        bool downButton = false;
        bool runButton = false;
        bool timeScaleButton = false;
        
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            upButton = Keyboard.current[UpKey].isPressed;
            downButton = Keyboard.current[DownKey].isPressed;
            runButton = Keyboard.current[RunKey].isPressed;
            timeScaleButton = Keyboard.current[TimescaleModificationKey].wasPressedThisFrame;
        #else
            upButton = Input.GetKey(UpButton);
            downButton = Input.GetKey(DownButton);
            runButton = Input.GetKey(RunButton);
            timeScaleButton = Input.GetKeyDown(TimescaleModificationButton);
        #endif

        _currentInput.y = 0f;
        
        if (upButton)
        {
            _currentInput.y = 1f; 
        }
        if (downButton)
        {
            _currentInput.y = -1f;
        }

        _speedMultiplier = runButton ? RunFactor : 1f;
        _normalizedInput = _currentInput.normalized;         
    }

    /// <summary>
    /// Computes the new position
    /// </summary>
    protected virtual void Translate()
    {
        if (!MovementEnabled)
        {
            return;
        }

        if ((Acceleration == 0) || (Deceleration == 0))
        {
            _lerpedInput = _currentInput;
        }
        else
        {
            if (_normalizedInput.magnitude == 0)
            {
                _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
                _lerpedInput = Vector3.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
            }
            else
            {
                _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                _lerpedInput = Vector3.ClampMagnitude(_normalizedInput, _acceleration);
            }
        }

        _movementVector = _lerpedInput;
        _movementVector *= MovementSpeed * _speedMultiplier;

        if (_movementVector.magnitude > MovementSpeed * _speedMultiplier)
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed * _speedMultiplier);
        }
    }

    /// <summary>
    /// Computes the new rotation
    /// </summary>
    protected virtual void Rotate()
    {
        if (!RotationEnabled)
        {
            return;
        }
        _newEulerAngles = this.transform.eulerAngles;

        _newEulerAngles.x += -_mouseInput.y * 359f * MouseSensitivity;
        _newEulerAngles.y += _mouseInput.x * 359f * MouseSensitivity;

        _newEulerAngles = Vector3.Lerp(this.transform.eulerAngles, _newEulerAngles, Time.deltaTime * RotationSpeed);
    }

    /// <summary>
    /// Modifies the camera's transform's position and rotation
    /// </summary>
    protected virtual void Move()
    {
        transform.eulerAngles = _newEulerAngles;
        transform.position += transform.rotation * _movementVector * Time.deltaTime;
    }

    /// <summary>
    /// Toggles the camera's active state
    /// </summary>
    protected virtual void ToggleFreeCamera()
    {
        Active = !Active;
        Cursor.lockState = Active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Active;
    }
}
