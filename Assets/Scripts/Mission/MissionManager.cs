using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class MissionManager : MonoBehaviour, IGameManager
{
    private MMStateMachine<MissionState> m_missionStateMachine;

    [SerializeField] private List<GameObject> m_playerCharacters = new List<GameObject>();

    [Header("Statistics Tracker Configuration")]
    [SerializeField] private Transform m_statisticsTrackerParent = null;

    public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.MissionManager;

    public MissionObjective m_missionObjective;
    public MissionProgress m_missionProgress;


    // Dictionary mapping player GameObjects to their statistics trackers
    private Dictionary<GameObject, IndividualMissionStatisticsTracker> m_playerToTrackerDictionary = new Dictionary<GameObject, IndividualMissionStatisticsTracker>();
    // Dictionary mapping player GameObjects to their statistics data
    private Dictionary<GameObject, IndividualMissionStatisticsData> m_playerToStatisticsDataDictionary = new Dictionary<GameObject, IndividualMissionStatisticsData>();

    // List of all active statistics trackers
    private List<IndividualMissionStatisticsTracker> m_activeStatisticsTrackers = new List<IndividualMissionStatisticsTracker>();

    // Points Data Structures to be used for UI and Other Mission Review related functions
    public TeamPointsData m_teamPointsData;
    public Dictionary<GameObject, PlayerPointsData> m_playerToPointsDataDictionary;

    public MissionState CurrentMissionState => m_missionStateMachine != null ? m_missionStateMachine.CurrentState : MissionState.NotStarted;

    public bool IsMissionInProgress => CurrentMissionState == MissionState.InProgress;
    public bool IsMissionComplete => CurrentMissionState == MissionState.Complete;
    public bool IsMissionFailed => CurrentMissionState == MissionState.Failed;

    public IReadOnlyList<GameObject> PlayerCharacters => m_playerCharacters.AsReadOnly();
    public IReadOnlyList<IndividualMissionStatisticsTracker> ActiveStatisticsTrackers => m_activeStatisticsTrackers.AsReadOnly();

    public ManagerStatus status { get; private set; } = ManagerStatus.Shutdown;

    // private IEnumerator Start()
    // {
    //     while (MainNetManager.Instance.MissionStateNet == null || MainNetManager.Instance.MissionStateNet.status != ManagerStatus.Started)
    //     {
    //         yield return null;
    //     }

    //     m_missionManagerRole = MainNetManager.Instance.MissionStateNet.missionManagerRole;
    //     print($"MissionManager:MainNetManager:Event:MissionManagerRole: {m_missionManagerRole}");
    // }

    public void StartUp()
    {
        status = ManagerStatus.Initializing;

        LogFromMethod("StartUp");
        InitializeStateMachine();

        status = ManagerStatus.Shutdown;
    }

    public void SceneSetup(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetup");
    }
    public void SceneSetupAdditive(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetupAdditive");
    }

    public void Enable()
    {
        enabled = true;
    }
    public void Disable()
    {
        enabled = false;
    }

    private void LogFromMethod(string message)
    {
        print($"MissionManager:{message}");
    }

    public void ChangeStateMissionState(MissionState missionState)
    {
        m_missionStateMachine.ChangeState(missionState);

        switch (missionState)
        {
            case MissionState.NotStarted:
                Messenger.Broadcast("MISSION_NOT_STARTED", MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case MissionState.InProgress:
                Messenger.Broadcast("MISSION_IN_PROGRESS", MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case MissionState.Failed:
                Messenger.Broadcast("MISSION_FAILED", MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case MissionState.Complete:
                Messenger.Broadcast("MISSION_COMPLETED", MessengerMode.DONT_REQUIRE_LISTENER);
                break;
        }
    }

    public void StartMission()
    {

    }

    private void InitializeStateMachine()
    {
        m_missionStateMachine = new MMStateMachine<MissionState>(gameObject, true);
        ChangeStateMissionState(MissionState.NotStarted);

        Debug.Log("MissionManager: State machine initialized. Mission state: NotStarted");
    }

    private void CreateStatisticsTrackerForPlayer(GameObject playerCharacter)
    {
        GameObject trackerGameObject = new GameObject($"StatisticsTracker_{playerCharacter.name}");

        if (m_statisticsTrackerParent != null)
        {
            trackerGameObject.transform.SetParent(m_statisticsTrackerParent);
        }

        IndividualMissionStatisticsTracker tracker = trackerGameObject.AddComponent<IndividualMissionStatisticsTracker>();

        IndividualMissionStatisticsData statisticsData = new IndividualMissionStatisticsData();

        tracker.InitializeTracker(playerCharacter, statisticsData);

        m_playerToTrackerDictionary[playerCharacter] = tracker;
        m_playerToStatisticsDataDictionary[playerCharacter] = statisticsData;
        m_activeStatisticsTrackers.Add(tracker);

        Debug.Log($"MissionManager: Created statistics tracker for player '{playerCharacter.name}'");
    }

    public void CompleteMission()
    {
        if (m_missionStateMachine.CurrentState != MissionState.InProgress)
        {
            Debug.LogWarning($"MissionManager: Cannot complete mission - mission is not in progress! Current state: {m_missionStateMachine.CurrentState}");
            return;
        }

        ChangeStateMissionState(MissionState.Complete);
        Debug.Log("MissionManager: Mission completed successfully!");
    }

    public void FailMission()
    {
        if (m_missionStateMachine.CurrentState != MissionState.InProgress)
        {
            Debug.LogWarning($"MissionManager: Cannot fail mission - mission is not in progress! Current state: {m_missionStateMachine.CurrentState}");
            return;
        }

        ChangeStateMissionState(MissionState.Failed);
        Debug.Log("MissionManager: Mission failed!");
    }

    private void ClearAllStatisticsTrackers()
    {
        // Destroy all tracker GameObjects
        foreach (IndividualMissionStatisticsTracker tracker in m_activeStatisticsTrackers)
        {
            if (tracker != null && tracker.gameObject != null)
            {
                Destroy(tracker.gameObject);
            }
        }

        // Clear all collections
        m_playerToTrackerDictionary.Clear();
        m_playerToStatisticsDataDictionary.Clear();
        m_activeStatisticsTrackers.Clear();

        Debug.Log("MissionManager: Cleared all statistics trackers.");
    }

    public IndividualMissionStatisticsTracker GetStatisticsTrackerForPlayer(GameObject playerCharacter)
    {
        if (playerCharacter == null)
        {
            Debug.LogWarning("MissionManager: Cannot get statistics tracker - player character is null!");
            return null;
        }

        if (m_playerToTrackerDictionary.TryGetValue(playerCharacter, out IndividualMissionStatisticsTracker tracker))
        {
            return tracker;
        }

        Debug.LogWarning($"MissionManager: No statistics tracker found for player '{playerCharacter.name}'");
        return null;
    }

    public IndividualMissionStatisticsData GetStatisticsDataForPlayer(GameObject playerCharacter)
    {
        if (playerCharacter == null)
        {
            Debug.LogWarning("MissionManager: Cannot get statistics data - player character is null!");
            return null;
        }

        if (m_playerToStatisticsDataDictionary.TryGetValue(playerCharacter, out IndividualMissionStatisticsData data))
        {
            return data;
        }

        Debug.LogWarning($"MissionManager: No statistics data found for player '{playerCharacter.name}'");
        return null;
    }

    public void ResetMission()
    {
        ClearAllStatisticsTrackers();
        ChangeStateMissionState(MissionState.NotStarted);

        Debug.Log("MissionManager: Mission reset to NotStarted state.");
    }

    public void AddPlayerCharacter(GameObject playerCharacter)
    {
        Debug.Log($"MissionManager: Added player character '{playerCharacter.name}' to mission.");
    }

    public void RemovePlayerCharacter(GameObject playerCharacter)
    {
        Debug.Log($"MissionManager: Removed player character '{playerCharacter.name}' from mission.");
    }

}
