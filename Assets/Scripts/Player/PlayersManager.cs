using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using NaughtyAttributes;
using UnityEngine;

// Deeply Connected to the PlayerManagerStateNet
public class PlayersManager : MonoBehaviour ,IGameManager
{
    [SerializeField] private Transform playerUIParentTransform;
    [Required]
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField] private GameObject playerUIPrefab;

    public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.PlayersManager;
    private List<PlayerSpecifications> m_playersWithDetails = new List<PlayerSpecifications>();
    
    private List<GameObject> m_activePlayers = new List<GameObject>();
    private Dictionary<Guid, GameObject> m_playerRefToPlayerGameObject = new Dictionary<Guid, GameObject>();

    private Dictionary<GameObject, IndividualPlayerManager> m_playerToIndividualPlayerManagerComponent = new Dictionary<GameObject, IndividualPlayerManager>();
    private List<IndividualPlayerManager> m_individualPlayerManagerList = new List<IndividualPlayerManager>();

    private Dictionary<GameObject, GameObject> m_playerGameObjectToPlayerUIGO = new Dictionary<GameObject, GameObject>();

    public ManagerStatus status { get; private set; } = ManagerStatus.Shutdown;

    private MMShufflebag<Transform> m_spawnPointShuffleBag = new MMShufflebag<Transform>(4);

    public void PlayersSpawnedHandler() { }

    public void SetupPlayer() { }

    public void StartUp()
    {
        status = ManagerStatus.Initializing;

        LogFromMethod("StartUp");
        InitializeShufflebag();

        status = ManagerStatus.Started;
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
        print($"PlayersManager:{message}");
    }

    public void SpawnAvailablePlayers()
    {
        foreach (PlayerSpecifications playerSpecs in m_playersWithDetails)
        {
            SpawnPlayer(playerSpecs);
        }

    }

    public void SpawnPlayer(PlayerSpecifications player) { }
    
    public void AddPlayerToPlayerList(PlayerSpecifications playerSpecs)
    {
        m_playersWithDetails.Add(playerSpecs);
    }

    public void SetUpPlayerManagerIndvidualPlayerManagerEvents(IndividualPlayerManager individualPlayerManager) {}

    public List<GameObject> GetActivePlayers() {  return m_activePlayers; }
    public IndividualPlayerManager GetIndividualPlayerManagerComponent(GameObject player)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            return individualPlayerManager;
        }

        Debug.Log($"PlayerManager: No individual player manager found for player {player.name}");
        return null;
    }
    public void DestroyPlayer() { }

    public GameObject GetPlayerUIGO(GameObject playerGameObject)
    {
        if (!m_playerGameObjectToPlayerUIGO.ContainsKey(playerGameObject)) return null;

        return m_playerGameObjectToPlayerUIGO[playerGameObject];
    }

    public void AddListenerToAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<GameObject> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {

            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnPlayerHealthy:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HEALTHY}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerHurt:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HURT}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerDeath:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerReborn:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionIdle:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_IDLE}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionAttemptingInteraction:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_ATTEMPTING_INTERACTION}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionInteracting:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_INTERACTING}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionBusy:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_BUSY}", action);
                    break;
            }
        }
    }
    public void AddListenerToAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<PlayerLifecycleStates> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {

            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnLifecycleStateChanged:
                    Messenger<PlayerLifecycleStates>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", action);
                    break;
            }
        }
    }
    public void AddListenerToAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<float, float> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {

            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnHealthChanged:
                    Messenger<float, float>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", action);
                    break;
                case IndividualPlayerEvents.OnStaminaChanged:
                    Messenger<float, float>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", action);
                    break;
            }
        }
    }

    public void AddListenerToSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<PlayerLifecycleStates> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnLifecycleStateChanged:
                    Messenger<PlayerLifecycleStates>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", action);
                    break;
            }
        }
    }
    public void AddListenerToSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<float, float> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnHealthChanged:
                    Messenger<float, float>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", action);
                    break;
                case IndividualPlayerEvents.OnStaminaChanged:
                    Messenger<float, float>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", action);
                    break;
            }
        }
    }
    public void AddListenerToSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<GameObject> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnPlayerHealthy:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HEALTHY}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerHurt:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HURT}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerDeath:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerReborn:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionIdle:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_IDLE}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionAttemptingInteraction:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_ATTEMPTING_INTERACTION}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionInteracting:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_INTERACTING}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionBusy:
                    Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_BUSY}", action);
                    break;
            }
        }
    }

    public void RemoveListenerFromAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<GameObject> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnPlayerHealthy:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HEALTHY}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerHurt:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HURT}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerDeath:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerReborn:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionIdle:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_IDLE}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionAttemptingInteraction:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_ATTEMPTING_INTERACTION}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionInteracting:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_INTERACTING}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionBusy:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_BUSY}", action);
                    break;
            }
        }
    }
    public void RemoveListenerFromAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<PlayerLifecycleStates> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {

            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnLifecycleStateChanged:
                    Messenger<PlayerLifecycleStates>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", action);
                    break;
            }
        }
    }
    public void RemoveListenerFromAllPlayers(IndividualPlayerEvents individualPlayerEvent, Action<float, float> action)
    {
        foreach (IndividualPlayerManager individualPlayerManager in m_individualPlayerManagerList)
        {

            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnHealthChanged:
                    Messenger<float,float>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", action);
                    break;
                case IndividualPlayerEvents.OnStaminaChanged:
                    Messenger<float,float>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", action);
                    break;
            }
        }
    }

    public void RemoveListenerFromSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<PlayerLifecycleStates> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnLifecycleStateChanged:
                    Messenger<PlayerLifecycleStates>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", action);
                    break;
            }
        }
    }
    public void RemoveListenerFromSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<float, float> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnHealthChanged:
                    Messenger<float, float>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", action);
                    break;
                case IndividualPlayerEvents.OnStaminaChanged:
                    Messenger<float, float>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", action);
                    break;
            }
        }
    }
    public void RemoveListenerFromSpecificPlayer(GameObject player, IndividualPlayerEvents individualPlayerEvent, Action<GameObject> action)
    {
        if (m_playerToIndividualPlayerManagerComponent.TryGetValue(player, out IndividualPlayerManager individualPlayerManager))
        {
            switch (individualPlayerEvent)
            {
                case IndividualPlayerEvents.OnPlayerHealthy:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HEALTHY}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerHurt:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_HURT}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerDeath:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerReborn:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionIdle:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_IDLE}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionAttemptingInteraction:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_ATTEMPTING_INTERACTION}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionInteracting:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_INTERACTING}", action);
                    break;
                case IndividualPlayerEvents.OnPlayerInteractionBusy:
                    Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_BUSY}", action);
                    break;
            }
        }
    }


    private void SetPlayerEventListeners(IndividualPlayerManager individualPlayerManager)
    { 
        Messenger<float,float>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", (currentHealth, maxHealth) => print("nothing"));
        Messenger<PlayerLifecycleStates>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", (PlayerLifecycleStates) => print("nothing"));
        Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", (playerGO) => print("nothing"));
        Messenger<GameObject>.AddListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", (playerGO) => print("nothing"));
    }
    
    private void RemovePlayerEventListeners(IndividualPlayerManager individualPlayerManager)
    {
        Messenger<float,float>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", (currentHealth, maxHealth) => print("nothing"));
        Messenger<PlayerLifecycleStates>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", (PlayerLifecycleStates) => print("nothing"));
        Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", (playerGO) => print("nothing"));
        Messenger<GameObject>.RemoveListener($"{individualPlayerManager.PlayerId}{PlayerIndividualEvent.ON_PLAYER_REBORN}", (playerGO) => print("nothing"));
    }

    public void InitializeShufflebag()
    {
        if (spawnPoints.Count > 0)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                m_spawnPointShuffleBag.Add(spawnPoint, 1);
            }
        }
    }



}