/// <summary>
/// Represents the different health states the player can be in during gameplay.
/// Used by the PlayerManager to manage player lifecycle and state transitions.
/// </summary>
public enum PlayerLifecycleStates
{
    Healthy,    // Player is at full or near-full health
    Hurt,       // Player has taken some damage but is still functional
    Critical,   // Player is at low health and in danger
    Dead        // Player has no health remaining
}

// Interaction States
public enum PlayerInteractionStates
{
    Idle,
    AttemptingInteraction,
    Interacting,
    Busy
}

// Locomotion States
public enum PlayerLocomotionStates
{
    Grounded,
    Airborne,
    Crouching,
    Stunned
}

// Action States
public enum PlayerActionStates
{
    None,
    Attacking,
    UsingItem,
    Cooldown,
}

// needs something for the ItemID
public enum PlayerItemTypeStates
{
    None,
    Key,
    Flashlight,
    Medkit,
    Tool,
    Special
}

// Equipment State
public enum PlayerEquipmentStates
{
    HoldingItem,
    ItemID,
    Swapping
}

// Environment State
public enum PlayerDarknessStates
{
    Light,
    Dark,
}
public enum PlayerHiddenStates
{
    Visible,
    Hidden
}
public enum PlayerChaseStates
{
    NotChased,
    BeingChased
}

// Resource States
public enum PlayerStaminaStates
{
    Normal,
    Low,
    Exhausted,
    Reserved
}

public enum PlayerFearStates
{
    Calm,
    Anxious,
    Terrified,
    Reserved
}

// Animation State (Local Only Not Networked)
public enum PlayerAnimationStates
{
    Idle,
    Walk,
    Run,
    Crouch,
    Attack,
    UseItem,
    Stunned,
    Death
}