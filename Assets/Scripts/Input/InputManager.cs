using MoreMountains.Tools;
using UnityEngine;
using System.Collections.Generic;

/*
    This is unrefined and made with AI, need to repurpose it for true local input (could possibly be networked)
*/
public class InputManager : MMSingleton<InputManager>
{
    [Header("Input State Machine")]
    [Tooltip("The state machine managing input system states")]
    public MMStateMachine<InputSystemStates> m_inputSystemStateMachine;

    [Header("Default Keyboard Bindings")]
    [Tooltip("Default key for moving forward")]
    [SerializeField] private KeyCode m_defaultKeyForward = KeyCode.W;

    [Tooltip("Default key for moving backward")]
    [SerializeField] private KeyCode m_defaultKeyBackward = KeyCode.S;

    [Tooltip("Default key for moving left")]
    [SerializeField] private KeyCode m_defaultKeyLeft = KeyCode.A;

    [Tooltip("Default key for moving right")]
    [SerializeField] private KeyCode m_defaultKeyRight = KeyCode.D;

    [Tooltip("Default key for jumping")]
    [SerializeField] private KeyCode m_defaultKeyJump = KeyCode.Space;

    [Tooltip("Default key for sprinting")]
    [SerializeField] private KeyCode m_defaultKeySprint = KeyCode.LeftShift;

    [Tooltip("Default key for interacting")]
    [SerializeField] private KeyCode m_defaultKeyInteract = KeyCode.E;

    [Tooltip("Default key for pausing")]
    [SerializeField] private KeyCode m_defaultKeyPause = KeyCode.Escape;

    [Tooltip("Default key for crouching")]
    [SerializeField] private KeyCode m_defaultKeyCrouch = KeyCode.LeftControl;

    [Tooltip("Default key for attacking")]
    [SerializeField] private KeyCode m_defaultKeyAttack = KeyCode.Mouse0;

    [Tooltip("Default key for secondary action")]
    [SerializeField] private KeyCode m_defaultKeySecondaryAction = KeyCode.Mouse1;

    [Header("Controller Settings")]
    [Tooltip("Dead zone for controller analog sticks (0-1)")]
    [SerializeField] private float m_controllerDeadZone = 0.2f;

    [Tooltip("Whether controller input is enabled")]
    [SerializeField] private bool m_controllerEnabled = true;

    // Current keybindings (loaded from PlayerPrefs or defaults)
    private Dictionary<string, KeyCode> m_keyboardBindings;

    // Input state tracking
    private Vector2 m_movementInput = Vector2.zero;
    private bool m_isMovementInputActive = false;
    private bool m_jumpPressedThisFrame = false;
    private bool m_jumpHeld = false;
    private bool m_sprintHeld = false;
    private bool m_interactPressedThisFrame = false;
    private bool m_pausePressedThisFrame = false;
    private bool m_crouchHeld = false;
    private bool m_attackPressedThisFrame = false;
    private bool m_attackHeld = false;
    private bool m_secondaryActionPressedThisFrame = false;
    private bool m_secondaryActionHeld = false;

    // Previous frame state (for detecting button releases)
    private bool m_jumpHeldPreviousFrame = false;
    private bool m_attackHeldPreviousFrame = false;
    private bool m_secondaryActionHeldPreviousFrame = false;

    // PlayerPrefs keys for saving bindings
    private const string PLAYER_PREFS_KEY_PREFIX = "InputBinding_";

    // Input action names
    private const string ACTION_FORWARD = "Forward";
    private const string ACTION_BACKWARD = "Backward";
    private const string ACTION_LEFT = "Left";
    private const string ACTION_RIGHT = "Right";
    private const string ACTION_JUMP = "Jump";
    private const string ACTION_SPRINT = "Sprint";
    private const string ACTION_INTERACT = "Interact";
    private const string ACTION_PAUSE = "Pause";
    private const string ACTION_CROUCH = "Crouch";
    private const string ACTION_ATTACK = "Attack";
    private const string ACTION_SECONDARY_ACTION = "SecondaryAction";

    // Events for input actions
    public System.Action<Vector2> OnMovementInputChanged;
    public System.Action OnJumpPressed;
    public System.Action OnJumpReleased;
    public System.Action<bool> OnSprintStateChanged;
    public System.Action OnInteractPressed;
    public System.Action OnPausePressed;
    public System.Action<bool> OnCrouchStateChanged;
    public System.Action OnAttackPressed;
    public System.Action OnAttackReleased;
    public System.Action OnSecondaryActionPressed;
    public System.Action OnSecondaryActionReleased;
    public System.Action<InputSystemStates> OnInputSystemStateChanged;

    // Public properties for reading input state
    public Vector2 MovementInput => m_movementInput;
    public bool IsMovementInputActive => m_isMovementInputActive;
    public bool JumpPressedThisFrame => m_jumpPressedThisFrame;
    public bool JumpHeld => m_jumpHeld;
    public bool SprintHeld => m_sprintHeld;
    public bool InteractPressedThisFrame => m_interactPressedThisFrame;
    public bool PausePressedThisFrame => m_pausePressedThisFrame;
    public bool CrouchHeld => m_crouchHeld;
    public bool AttackPressedThisFrame => m_attackPressedThisFrame;
    public bool AttackHeld => m_attackHeld;
    public bool SecondaryActionPressedThisFrame => m_secondaryActionPressedThisFrame;
    public bool SecondaryActionHeld => m_secondaryActionHeld;

    private void Start()
    {
        InitializeStateMachine();
        InitializeKeybindings();
    }

    private void Update()
    {
        if (m_inputSystemStateMachine.CurrentState == InputSystemStates.Disabled)
        {
            ResetAllInputs();
            return;
        }

        UpdateInputs();
    }

    private void LateUpdate()
    {
        // Reset frame-specific inputs after all systems have read them
        m_jumpPressedThisFrame = false;
        m_interactPressedThisFrame = false;
        m_pausePressedThisFrame = false;
        m_attackPressedThisFrame = false;
        m_secondaryActionPressedThisFrame = false;

        // Update previous frame states
        m_jumpHeldPreviousFrame = m_jumpHeld;
        m_attackHeldPreviousFrame = m_attackHeld;
        m_secondaryActionHeldPreviousFrame = m_secondaryActionHeld;
    }

    /// <summary>
    /// Initializes the input system state machine
    /// </summary>
    private void InitializeStateMachine()
    {
        m_inputSystemStateMachine = new MMStateMachine<InputSystemStates>(gameObject, true);
        m_inputSystemStateMachine.ChangeState(InputSystemStates.Enabled);
        m_inputSystemStateMachine.OnStateChange += OnStateMachineStateChanged;
    }

    /// <summary>
    /// Initializes keybindings from PlayerPrefs or uses defaults
    /// </summary>
    private void InitializeKeybindings()
    {
        m_keyboardBindings = new Dictionary<string, KeyCode>();

        // Load bindings from PlayerPrefs or use defaults
        m_keyboardBindings[ACTION_FORWARD] = LoadKeyBinding(ACTION_FORWARD, m_defaultKeyForward);
        m_keyboardBindings[ACTION_BACKWARD] = LoadKeyBinding(ACTION_BACKWARD, m_defaultKeyBackward);
        m_keyboardBindings[ACTION_LEFT] = LoadKeyBinding(ACTION_LEFT, m_defaultKeyLeft);
        m_keyboardBindings[ACTION_RIGHT] = LoadKeyBinding(ACTION_RIGHT, m_defaultKeyRight);
        m_keyboardBindings[ACTION_JUMP] = LoadKeyBinding(ACTION_JUMP, m_defaultKeyJump);
        m_keyboardBindings[ACTION_SPRINT] = LoadKeyBinding(ACTION_SPRINT, m_defaultKeySprint);
        m_keyboardBindings[ACTION_INTERACT] = LoadKeyBinding(ACTION_INTERACT, m_defaultKeyInteract);
        m_keyboardBindings[ACTION_PAUSE] = LoadKeyBinding(ACTION_PAUSE, m_defaultKeyPause);
        m_keyboardBindings[ACTION_CROUCH] = LoadKeyBinding(ACTION_CROUCH, m_defaultKeyCrouch);
        m_keyboardBindings[ACTION_ATTACK] = LoadKeyBinding(ACTION_ATTACK, m_defaultKeyAttack);
        m_keyboardBindings[ACTION_SECONDARY_ACTION] = LoadKeyBinding(ACTION_SECONDARY_ACTION, m_defaultKeySecondaryAction);
    }

    /// <summary>
    /// Loads a key binding from PlayerPrefs or returns the default
    /// </summary>
    private KeyCode LoadKeyBinding(string actionName, KeyCode defaultKey)
    {
        string prefKey = PLAYER_PREFS_KEY_PREFIX + actionName;
        if (PlayerPrefs.HasKey(prefKey))
        {
            return (KeyCode)PlayerPrefs.GetInt(prefKey);
        }
        return defaultKey;
    }

    /// <summary>
    /// Saves a key binding to PlayerPrefs
    /// </summary>
    private void SaveKeyBinding(string actionName, KeyCode key)
    {
        string prefKey = PLAYER_PREFS_KEY_PREFIX + actionName;
        PlayerPrefs.SetInt(prefKey, (int)key);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Updates all input states based on current input system state
    /// </summary>
    private void UpdateInputs()
    {
        InputSystemStates currentState = m_inputSystemStateMachine.CurrentState;

        // Movement input is available in Enabled and MovementOnly states
        if (currentState == InputSystemStates.Enabled || currentState == InputSystemStates.MovementOnly)
        {
            UpdateMovementInput();
        }
        else
        {
            m_movementInput = Vector2.zero;
            m_isMovementInputActive = false;
        }

        // Action inputs are only available in Enabled state
        if (currentState == InputSystemStates.Enabled)
        {
            UpdateActionInputs();
        }
        else
        {
            ResetActionInputs();
        }

        // Menu inputs are available in Enabled and MenuOnly states
        if (currentState == InputSystemStates.Enabled || currentState == InputSystemStates.MenuOnly)
        {
            UpdateMenuInputs();
        }
        else
        {
            m_pausePressedThisFrame = false;
        }
    }

    /// <summary>
    /// Updates movement input from keyboard and controller
    /// </summary>
    private void UpdateMovementInput()
    {
        Vector2 newMovementInput = Vector2.zero;

        // Keyboard input
        if (GetKeyHeld(m_keyboardBindings[ACTION_FORWARD]))
        {
            newMovementInput.y += 1f;
        }
        if (GetKeyHeld(m_keyboardBindings[ACTION_BACKWARD]))
        {
            newMovementInput.y -= 1f;
        }
        if (GetKeyHeld(m_keyboardBindings[ACTION_LEFT]))
        {
            newMovementInput.x -= 1f;
        }
        if (GetKeyHeld(m_keyboardBindings[ACTION_RIGHT]))
        {
            newMovementInput.x += 1f;
        }

        // Controller input (if enabled)
        if (m_controllerEnabled)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            // Apply dead zone
            if (Mathf.Abs(horizontalAxis) < m_controllerDeadZone)
            {
                horizontalAxis = 0f;
            }
            if (Mathf.Abs(verticalAxis) < m_controllerDeadZone)
            {
                verticalAxis = 0f;
            }

            // Combine keyboard and controller input (controller takes priority if active)
            if (Mathf.Abs(horizontalAxis) > 0.01f || Mathf.Abs(verticalAxis) > 0.01f)
            {
                newMovementInput = new Vector2(horizontalAxis, verticalAxis);
            }
        }

        // Normalize diagonal movement
        if (newMovementInput.magnitude > 1f)
        {
            newMovementInput.Normalize();
        }

        // Update movement input state
        bool wasMovementActive = m_isMovementInputActive;
        m_movementInput = newMovementInput;
        m_isMovementInputActive = m_movementInput.magnitude > 0.01f;

        // Trigger event if movement state changed
        if (wasMovementActive != m_isMovementInputActive || m_movementInput != Vector2.zero)
        {
            OnMovementInputChanged?.Invoke(m_movementInput);
        }
    }

    /// <summary>
    /// Updates action inputs (jump, sprint, interact, crouch, attack, etc.)
    /// </summary>
    private void UpdateActionInputs()
    {
        // Jump input
        bool jumpKeyPressed = GetKeyPressed(m_keyboardBindings[ACTION_JUMP]);
        bool jumpControllerPressed = m_controllerEnabled && Input.GetButtonDown("Jump");

        if (jumpKeyPressed || jumpControllerPressed)
        {
            m_jumpPressedThisFrame = true;
            m_jumpHeld = true;
            OnJumpPressed?.Invoke();
        }

        m_jumpHeld = GetKeyHeld(m_keyboardBindings[ACTION_JUMP]) ||
                     (m_controllerEnabled && Input.GetButton("Jump"));

        if (m_jumpHeldPreviousFrame && !m_jumpHeld)
        {
            OnJumpReleased?.Invoke();
        }

        // Sprint input
        bool sprintKeyHeld = GetKeyHeld(m_keyboardBindings[ACTION_SPRINT]);
        bool sprintControllerHeld = m_controllerEnabled && Input.GetAxis("Sprint") > 0.5f;
        bool newSprintState = sprintKeyHeld || sprintControllerHeld;

        if (m_sprintHeld != newSprintState)
        {
            m_sprintHeld = newSprintState;
            OnSprintStateChanged?.Invoke(m_sprintHeld);
        }

        // Interact input
        bool interactKeyPressed = GetKeyPressed(m_keyboardBindings[ACTION_INTERACT]);
        bool interactControllerPressed = m_controllerEnabled && Input.GetButtonDown("Interact");

        if (interactKeyPressed || interactControllerPressed)
        {
            m_interactPressedThisFrame = true;
            OnInteractPressed?.Invoke();
        }

        // Crouch input
        bool crouchKeyHeld = GetKeyHeld(m_keyboardBindings[ACTION_CROUCH]);
        bool crouchControllerHeld = m_controllerEnabled && Input.GetButton("Crouch");
        bool newCrouchState = crouchKeyHeld || crouchControllerHeld;

        if (m_crouchHeld != newCrouchState)
        {
            m_crouchHeld = newCrouchState;
            OnCrouchStateChanged?.Invoke(m_crouchHeld);
        }

        // Attack input
        bool attackKeyPressed = GetKeyPressed(m_keyboardBindings[ACTION_ATTACK]);
        bool attackControllerPressed = m_controllerEnabled && Input.GetButtonDown("Fire1");

        if (attackKeyPressed || attackControllerPressed)
        {
            m_attackPressedThisFrame = true;
            m_attackHeld = true;
            OnAttackPressed?.Invoke();
        }

        m_attackHeld = GetKeyHeld(m_keyboardBindings[ACTION_ATTACK]) ||
                       (m_controllerEnabled && Input.GetButton("Fire1"));

        if (m_attackHeldPreviousFrame && !m_attackHeld)
        {
            OnAttackReleased?.Invoke();
        }

        // Secondary action input
        bool secondaryKeyPressed = GetKeyPressed(m_keyboardBindings[ACTION_SECONDARY_ACTION]);
        bool secondaryControllerPressed = m_controllerEnabled && Input.GetButtonDown("Fire2");

        if (secondaryKeyPressed || secondaryControllerPressed)
        {
            m_secondaryActionPressedThisFrame = true;
            m_secondaryActionHeld = true;
            OnSecondaryActionPressed?.Invoke();
        }

        m_secondaryActionHeld = GetKeyHeld(m_keyboardBindings[ACTION_SECONDARY_ACTION]) ||
                                (m_controllerEnabled && Input.GetButton("Fire2"));

        if (m_secondaryActionHeldPreviousFrame && !m_secondaryActionHeld)
        {
            OnSecondaryActionReleased?.Invoke();
        }
    }

    /// <summary>
    /// Updates menu-related inputs (pause, etc.)
    /// </summary>
    private void UpdateMenuInputs()
    {
        bool pauseKeyPressed = GetKeyPressed(m_keyboardBindings[ACTION_PAUSE]);
        bool pauseControllerPressed = m_controllerEnabled && Input.GetButtonDown("Cancel");

        if (pauseKeyPressed || pauseControllerPressed)
        {
            m_pausePressedThisFrame = true;
            OnPausePressed?.Invoke();
        }
    }

    /// <summary>
    /// Resets all action inputs to their default state
    /// </summary>
    private void ResetActionInputs()
    {
        m_jumpHeld = false;
        m_sprintHeld = false;
        m_crouchHeld = false;
        m_attackHeld = false;
        m_secondaryActionHeld = false;
    }

    /// <summary>
    /// Resets all inputs to their default state
    /// </summary>
    private void ResetAllInputs()
    {
        m_movementInput = Vector2.zero;
        m_isMovementInputActive = false;
        ResetActionInputs();
    }

    /// <summary>
    /// Helper method to check if a key was pressed this frame
    /// </summary>
    private bool GetKeyPressed(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    /// <summary>
    /// Helper method to check if a key is currently held
    /// </summary>
    private bool GetKeyHeld(KeyCode key)
    {
        return Input.GetKey(key);
    }

    /// <summary>
    /// Remaps a key binding for a specific action
    /// </summary>
    /// <param name="actionName">The name of the action to remap</param>
    /// <param name="newKey">The new key code to bind</param>
    public void RemapKeyBinding(string actionName, KeyCode newKey)
    {
        if (m_keyboardBindings.ContainsKey(actionName))
        {
            m_keyboardBindings[actionName] = newKey;
            SaveKeyBinding(actionName, newKey);
        }
        else
        {
            Debug.LogWarning($"InputManager: Attempted to remap unknown action '{actionName}'");
        }
    }

    /// <summary>
    /// Gets the current key binding for a specific action
    /// </summary>
    /// <param name="actionName">The name of the action</param>
    /// <returns>The current KeyCode for the action, or KeyCode.None if not found</returns>
    public KeyCode GetKeyBinding(string actionName)
    {
        if (m_keyboardBindings.ContainsKey(actionName))
        {
            return m_keyboardBindings[actionName];
        }
        return KeyCode.None;
    }

    /// <summary>
    /// Resets all keybindings to their default values
    /// </summary>
    public void ResetKeybindingsToDefaults()
    {
        m_keyboardBindings[ACTION_FORWARD] = m_defaultKeyForward;
        m_keyboardBindings[ACTION_BACKWARD] = m_defaultKeyBackward;
        m_keyboardBindings[ACTION_LEFT] = m_defaultKeyLeft;
        m_keyboardBindings[ACTION_RIGHT] = m_defaultKeyRight;
        m_keyboardBindings[ACTION_JUMP] = m_defaultKeyJump;
        m_keyboardBindings[ACTION_SPRINT] = m_defaultKeySprint;
        m_keyboardBindings[ACTION_INTERACT] = m_defaultKeyInteract;
        m_keyboardBindings[ACTION_PAUSE] = m_defaultKeyPause;
        m_keyboardBindings[ACTION_CROUCH] = m_defaultKeyCrouch;
        m_keyboardBindings[ACTION_ATTACK] = m_defaultKeyAttack;
        m_keyboardBindings[ACTION_SECONDARY_ACTION] = m_defaultKeySecondaryAction;

        // Save all defaults to PlayerPrefs
        foreach (var binding in m_keyboardBindings)
        {
            SaveKeyBinding(binding.Key, binding.Value);
        }
    }

    /// <summary>
    /// Called when the input system state machine changes state
    /// </summary>
    private void OnStateMachineStateChanged()
    {
        OnInputSystemStateChanged?.Invoke(m_inputSystemStateMachine.CurrentState);
    }

    /// <summary>
    /// Gets the movement direction as a normalized Vector2
    /// Forward/Backward is Y axis, Left/Right is X axis
    /// </summary>
    public Vector2 GetMovementDirection()
    {
        return m_movementInput;
    }

    /// <summary>
    /// Gets the raw movement input without normalization
    /// </summary>
    public Vector2 GetRawMovementInput()
    {
        return m_movementInput;
    }

    private void OnDestroy()
    {
        if (m_inputSystemStateMachine != null)
        {
            m_inputSystemStateMachine.OnStateChange -= OnStateMachineStateChanged;
        }
    }
}

