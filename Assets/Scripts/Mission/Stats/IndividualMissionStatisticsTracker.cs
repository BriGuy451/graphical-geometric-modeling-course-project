using System;
using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// Tracks individual mission statistics for a single player character during a mission.
/// This class monitors and records various mission-related events and updates the associated data structure.
/// All tracked events are exposed as C# events for external systems to subscribe to.
/// </summary>
public class IndividualMissionStatisticsTracker : MonoBehaviour
{
    [Header("Player Character Reference")]
    [Tooltip("Reference to the player character GameObject this tracker is monitoring")]
    [SerializeField] private GameObject m_playerCharacterReference;
    
    [Header("Statistics Data Reference")]
    [Tooltip("Reference to the data structure that holds the statistics for this player")]
    [SerializeField] private IndividualMissionStatisticsData m_missionStatisticsData;
    
    /// <summary>
    /// Event that is triggered when this player encounters an enemy during the mission.
    /// Subscribers can use this to react to enemy encounters (e.g., play sounds, update UI).
    /// </summary>
    public Action<GameObject> OnIndividualEnemyEncounter;
    
    /// <summary>
    /// Event that is triggered when this player finds a customer during the mission.
    /// Subscribers can use this to react to customer discoveries (e.g., play sounds, update UI).
    /// </summary>
    public Action<GameObject> OnIndividualCustomerFound;
    
    /// <summary>
    /// Event that is triggered when this player successfully saves a customer during the mission.
    /// Subscribers can use this to react to successful customer saves (e.g., play sounds, update UI, award points).
    /// </summary>
    public Action<GameObject> OnIndividualCustomerSaved;
    
    /// <summary>
    /// Event that is triggered when this player finds an artifact during the mission.
    /// Subscribers can use this to react to artifact discoveries (e.g., play sounds, update UI).
    /// </summary>
    public Action<GameObject> OnIndividualArtifactFound;
    
    /// <summary>
    /// Event that is triggered when this player successfully retrieves an artifact during the mission.
    /// Subscribers can use this to react to successful artifact retrievals (e.g., play sounds, update UI, award points).
    /// </summary>
    public Action<GameObject> OnIndividualArtifactRetrieved;
    
    /// <summary>
    /// Gets the player character GameObject this tracker is monitoring
    /// </summary>
    public GameObject PlayerCharacterReference => m_playerCharacterReference;
    
    /// <summary>
    /// Gets the statistics data structure associated with this tracker
    /// </summary>
    public IndividualMissionStatisticsData MissionStatisticsData => m_missionStatisticsData;
    
    /// <summary>
    /// Initializes the tracker with a player character reference and statistics data structure.
    /// This should be called by the MissionManager when creating trackers for each player.
    /// </summary>
    /// <param name="playerCharacterReference">The GameObject representing the player character to track</param>
    /// <param name="statisticsData">The data structure that will hold this player's mission statistics</param>
    public void InitializeTracker(GameObject playerCharacterReference, IndividualMissionStatisticsData statisticsData)
    {
        if (playerCharacterReference == null)
        {
            Debug.LogError("IndividualMissionStatisticsTracker: Cannot initialize tracker with null player character reference!");
            return;
        }
        
        if (statisticsData == null)
        {
            Debug.LogError("IndividualMissionStatisticsTracker: Cannot initialize tracker with null statistics data!");
            return;
        }
        
        m_playerCharacterReference = playerCharacterReference;
        m_missionStatisticsData = statisticsData;
        
        Debug.Log($"IndividualMissionStatisticsTracker: Initialized tracker for player '{playerCharacterReference.name}'");
    }
    
    /// <summary>
    /// Records an enemy encounter for this player character.
    /// Increments the enemy encounter counter and triggers the OnIndividualEnemyEncounter event.
    /// </summary>
    /// <param name="enemyReference">Optional reference to the enemy GameObject that was encountered</param>
    public void RecordEnemyEncounter(GameObject enemyReference = null)
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot record enemy encounter - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.IncrementEnemyEncounterCount();
        OnIndividualEnemyEncounter?.Invoke(enemyReference != null ? enemyReference : m_playerCharacterReference);

        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_1");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);

        Debug.Log($"IndividualMissionStatisticsTracker: Recorded enemy encounter for player '{m_playerCharacterReference.name}'. Total encounters: {m_missionStatisticsData.TotalEnemiesEncountered}");
    }
    
    /// <summary>
    /// Records a customer found event for this player character.
    /// Increments the customer found counter and triggers the OnIndividualCustomerFound event.
    /// </summary>
    /// <param name="customerReference">Optional reference to the customer GameObject that was found</param>
    public void RecordCustomerFound(GameObject customerReference = null)
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot record customer found - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.IncrementCustomerFoundCount();
        OnIndividualCustomerFound?.Invoke(customerReference != null ? customerReference : m_playerCharacterReference);

        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_2");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);

        Debug.Log($"IndividualMissionStatisticsTracker: Recorded customer found for player '{m_playerCharacterReference.name}'. Total found: {m_missionStatisticsData.TotalCustomersFound}");
    }
    
    /// <summary>
    /// Records a customer saved event for this player character.
    /// Increments the customer saved counter and triggers the OnIndividualCustomerSaved event.
    /// </summary>
    /// <param name="customerReference">Optional reference to the customer GameObject that was saved</param>
    public void RecordCustomerSaved(GameObject customerReference = null)
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot record customer saved - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.IncrementCustomerSavedCount();
        OnIndividualCustomerSaved?.Invoke(customerReference != null ? customerReference : m_playerCharacterReference);

        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_3");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);

        Debug.Log($"IndividualMissionStatisticsTracker: Recorded customer saved for player '{m_playerCharacterReference.name}'. Total saved: {m_missionStatisticsData.TotalCustomersSaved}");
    }
    
    /// <summary>
    /// Records an artifact found event for this player character.
    /// Increments the artifact found counter and triggers the OnIndividualArtifactFound event.
    /// </summary>
    /// <param name="artifactReference">Optional reference to the artifact GameObject that was found</param>
    public void RecordArtifactFound(GameObject artifactReference = null)
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot record artifact found - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.IncrementArtifactFoundCount();
        OnIndividualArtifactFound?.Invoke(artifactReference != null ? artifactReference : m_playerCharacterReference);

        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_3");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);

        Debug.Log($"IndividualMissionStatisticsTracker: Recorded artifact found for player '{m_playerCharacterReference.name}'. Total found: {m_missionStatisticsData.TotalArtifactsFound}");
    }
    
    /// <summary>
    /// Records an artifact retrieved event for this player character.
    /// Increments the artifact retrieved counter and triggers the OnIndividualArtifactRetrieved event.
    /// </summary>
    /// <param name="artifactReference">Optional reference to the artifact GameObject that was retrieved</param>
    public void RecordArtifactRetrieved(GameObject artifactReference = null)
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot record artifact retrieved - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.IncrementArtifactRetrievedCount();
        OnIndividualArtifactRetrieved?.Invoke(artifactReference != null ? artifactReference : m_playerCharacterReference);

        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_4");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);

        Debug.Log($"IndividualMissionStatisticsTracker: Recorded artifact retrieved for player '{m_playerCharacterReference.name}'. Total retrieved: {m_missionStatisticsData.TotalArtifactsRetrieved}");
    }
    
    /// <summary>
    /// Resets all statistics for this player character.
    /// Useful when starting a new mission or resetting mission progress.
    /// </summary>
    public void ResetAllStatistics()
    {
        if (m_missionStatisticsData == null)
        {
            Debug.LogWarning("IndividualMissionStatisticsTracker: Cannot reset statistics - statistics data is null!");
            return;
        }
        
        m_missionStatisticsData.ResetAllStatistics();
        Debug.Log($"IndividualMissionStatisticsTracker: Reset all statistics for player '{m_playerCharacterReference.name}'");
    }
}

