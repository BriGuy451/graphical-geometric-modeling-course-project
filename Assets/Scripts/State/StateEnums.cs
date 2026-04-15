public enum PlayerType
{
    Host,
    Client
}

public enum UIViewShowingStates
{
    FocusOn,
    NonFocusOn,
    OFF
}

public enum UIComplexStates
{
    None,
    MainMenuScreen,
    
    IsolatedMenu,
    IsolatedLobbyHost,
    IsolatedLobbyClient,

    PlayerLobbyHostRoomScreen,
    PlayerCreateHostRoomFormScreen,
    PlayerClientRoomListScreen,
    PlayerLobbyClientRoomScreen,
    
    StartScreen,
    PauseScreen,
    EndGameScreen,
    DeathScreen,
    InMissionScreen,
    MissionReviewScreen,
    FailedMissionScreen,
    SuccessMissionScreen,

    // Central Scene
    MainMenuScreenGeom,

    // Effects Scene
    TitleOnGeom,
    TunerVisibleOnGeom,
    TunerVisibleOffGeom
}

public enum LoadingStates
{
    None,
    BlockInputOverlay
}

public enum PillarObjectiveState
{
    Deactivated,
    Activated
}

/// <summary>
/// Represents the different states the input system can be in.
/// Used by the InputManager to manage input availability and context.
/// </summary>
public enum InputSystemStates
{
    Enabled,        // Input is fully enabled and active
    Disabled,       // Input is completely disabled
    MenuOnly,       // Only menu/navigation input is enabled
    MovementOnly    // Only movement input is enabled (no actions)
}

/// <summary>
/// Represents the different states a mission can be in during gameplay.
/// Used by the MissionManager to track mission progress and completion status.
/// </summary>
public enum MissionState
{
    NotStarted,      // Mission has not yet begun
    InProgress,      // Mission is currently active and being played
    Exfil,           // Mission exfiltration has been activated
    AllDead,         // Mission has all players dead.
    TimeLimitExceed, // Mission has all players dead.
    Failed,          // Mission has failed (determines if next mission is unlocked)
    Complete,        // Mission has been successfully completed (determines if next mission is unlocked)
}

public enum InteractableStates
{
    Available,
    Pending,
    InUse,
    Cooldown,
    Inactive
}

/// <summary>
/// Represents the different states the NetworkManager can be in during networking lifecycle.
/// Used by the NetworkManager to track network connection and session state.
/// </summary>
public enum NetworkState
{
    Uninitialized,      // NetworkManager has not been initialized
    StartingRunner,     // NetworkRunner is being started
    Connecting,         // Attempting to connect to server/session
    Connected,          // Successfully connected to server/session
    LoadingScene,       // Loading a networked scene
    InGame,             // Fully connected and in-game
    ShuttingDown        // NetworkRunner is shutting down
}

public enum GameStateMaze
{
    Waiting,
    Playing,
    GameOver
}