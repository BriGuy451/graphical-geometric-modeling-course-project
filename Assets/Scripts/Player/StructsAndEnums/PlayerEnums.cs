public enum IndividualPlayerEvents
{
    // Player Properties Events
    OnHealthChanged,
    OnStaminaChanged,

    // Player Lifecycle Events
    OnLifecycleStateChanged,
    OnPlayerHealthy,
    OnPlayerHurt,
    OnPlayerCritical,
    OnPlayerDeath,
    OnPlayerReborn,

    // Player Exfil Events
    OnPlayerActivateExfil,
    OnPlayerDeactivateExfil,

    // Player Interaction Events
    OnPlayerInteractionIdle,
    OnPlayerInteractionAttemptingInteraction,
    OnPlayerInteractionInteracting,
    OnPlayerInteractionBusy
}

public enum Skin
{
    Green,
    Black,
    Blue,
    Pink,
    Red
}