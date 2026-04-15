using System;
using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// Manages the player's lifecycle including health, stamina, and state management.
/// Handles state transitions based on health thresholds and manages regeneration systems.
/// </summary>

/*
Full player life flow is influenced by the IndividualPlayerManager.

Lifecycle Events must be subscribed to in order to give the changes.

OnHealthy
    Players MaxHealth at 100% of Possible Limit, MovementSpeed at 100% of Possible Movement Speed, StaminaRechargeRate at 100% of Possible Stamina Recharge Rate, MaxStamina at 100% of Possible Limit
OnHurt = 
    Players MaxHealth at 100% of Possible Limit, MovementSpeed at 90% of Possible Movement Speed, StaminaRechargeRate at 80% of Possible Stamina Recharge Rate, MaxStamina at 70% of Possible Limit (Post Processing, UI Changes, Audio)
OnCritical =
    Players MaxHealth at 100% of Possible Limit, MovementSpeed at 60% of Possible Movement Speed, StaminaRechargeRate at 50% of Possible Stamina Recharge Rate, MaxStamina at 40% of Possible Limit | (Post Processing, UI Changes, Audio)
OnDeath = 
    Player Movement and Input Disabled, Player Character / Avatar Despawned (Replaced with Dead Player Prefab), (User Interface, Audio, Player Spectate)

Create private members that power the actual IndividualPlayerManager for health using the serialized field as limits, On Lifecycle State changes

sf CurrentHealthLimit
sf MaxHealthLimit

sf CurrentHealthLimit
sf MaxStaminaLimit
*/
public class IndividualPlayerManager : MonoBehaviour
{
    // Interaction Related
    public int PlayerId = 1;

    public bool playerInProximityToInteractable = false;

    #region Mandatory
    [SerializeField] private float m_maximumHealth = 100f;
    [SerializeField] private float m_currentHealth = 100f;
    [SerializeField] private float m_maximumStamina = 100f;    
    [SerializeField] private float m_currentStamina = 100f;
    [SerializeField] private float m_staminaRegenerationRatePerSecond = 10f;
    [SerializeField] private float m_staminaRegenerationDelaySeconds = 2f;
    [SerializeField] private float m_healthyStateThreshold = 0.75f;
    [SerializeField] private float m_hurtStateThreshold = 0.5f;
    [SerializeField] private float m_criticalStateThreshold = 0.25f;
    #endregion

    #region Properties
    public float CurrentHealth => m_currentHealth;
    public float MaximumHealth => m_maximumHealth;
    public float HealthPercentage => m_maximumHealth > 0 ? m_currentHealth / m_maximumHealth : 0f;
    public float CurrentStamina => m_currentStamina;
    public float MaximumStamina => m_maximumStamina;
    public float StaminaPercentage => m_maximumStamina > 0 ? m_currentStamina / m_maximumStamina : 0f;
    public bool IsDead => m_playerLifecycleStateMachine.CurrentState == PlayerLifecycleStates.Dead;
    #endregion

    #region StateMachines
    private MMStateMachine<PlayerLifecycleStates> m_playerLifecycleStateMachine;
    // Player Interaction States
    private MMStateMachine<PlayerInteractionStates> m_playerInteractionStateMachine;
    #endregion
    
    #region AudioRelated
    private AudioSource m_currentAudibleSoundPlaying;
    private MMShufflebag<Tuple<int,AudioClip>> m_damageClipShuffleBag = new MMShufflebag<Tuple<int, AudioClip>>(5);
    private MMShufflebag<Tuple<int,AudioClip>> m_healClipShuffleBag = new MMShufflebag<Tuple<int, AudioClip>>(5);
    #endregion

    private bool m_isStaminaRegenerating = false;
    private float m_timeSinceLastStaminaUse = 0f;

    private void Start()
    {
        m_playerInteractionStateMachine = new MMStateMachine<PlayerInteractionStates>(gameObject, true);

        ChangeStateInteraction(PlayerInteractionStates.Idle);

        InitializeLifecycleStateMachine();
        InitializeHealthAndStamina();
    }
    
    private void Update()
    {
        UpdateStaminaRegeneration();
        UpdateLifecycleState();

        // I feel this needs to be moved somewhere where I can actually call GetInput from the NetInput
        if (playerInProximityToInteractable && Input.GetKeyDown(KeyCode.E)) // mapped interaction button
        {
            // m_playerInteractionStateMachine.ChangeState(PlayerInteractionStates.AttemptingInteraction);

            // if (interactable.m_playerInteractingId == -1)
            // {
            //     interactable.m_playerInteractingId = playerId;
            // }
            // else 
            //     m_playerInteractionStateMachine.ChangeState(PlayerInteractionStates.Idle);
        }
    
    }
    
    // OnLoseHealth
    public void LoseHealth(float damageAmount)
    {
        if (IsDead || damageAmount <= 0f)
        {
            return;
        }
        
        
        // MainNetManager.Instance.NetworkEventManager.AddActionToActionQueue(() =>
        // {
            Messenger.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_HURT}");

            m_currentHealth = Mathf.Max(0f, m_currentHealth - damageAmount);

            Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", m_currentHealth, m_maximumHealth);
        // });
        
        // Check if player died
        if (m_currentHealth <= 0f)
        {
            // MainNetManager.Instance.NetworkEventManager.AddActionToActionQueue(() =>
            // {
                ChangeStatePlayerLifecycle(PlayerLifecycleStates.Dead);
            // });
        }
    }
    
    // OnGainHealth
    public void GainHealth(float healAmount)
    {
        if (IsDead || healAmount <= 0f)
        {
            return;
        }
        
        m_currentHealth = Mathf.Min(m_maximumHealth, m_currentHealth + healAmount);

        Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", m_currentHealth, m_maximumHealth);
    }
    
    //OnLoseStamina
    public void LoseStamina(float staminaCost)
    {
        if (staminaCost <= 0f)
        {
            return;
        }
        
        m_currentStamina = Mathf.Max(0f, m_currentStamina - staminaCost);
        m_isStaminaRegenerating = false;

        Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", m_currentStamina, m_maximumStamina);
    }
    
    //OnRechargeStamina
    public void RechargeStamina(float rechargeAmount)
    {
        if (rechargeAmount <= 0f)
        {
            return;
        }
        
        m_currentStamina = Mathf.Min(m_maximumStamina, m_currentStamina + rechargeAmount);
        

        Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", m_currentStamina, m_maximumStamina);
    }

    public void UpdatePlayerProperties(PlayerLifecycleStates lifecycleState)
    {
        switch (lifecycleState)
        {
            case PlayerLifecycleStates.Healthy:
                m_staminaRegenerationRatePerSecond = 10f;
                m_staminaRegenerationDelaySeconds = 2f;
                break;
            case PlayerLifecycleStates.Hurt:
                m_maximumHealth = 100f * .75f;

                m_maximumStamina = 100f * .75f;
                m_currentStamina = Mathf.Max(m_currentStamina, m_maximumStamina);

                m_staminaRegenerationRatePerSecond = 10f * .8f;
                m_staminaRegenerationDelaySeconds = 2f * .75f ;
                break;
            case PlayerLifecycleStates.Critical:
                m_maximumHealth = 100f * .50f;

                m_maximumStamina = 100f * .50f;
                m_currentStamina = Mathf.Max(m_currentStamina, m_maximumStamina);

                m_staminaRegenerationRatePerSecond = 10f * .6f;
                m_staminaRegenerationDelaySeconds = 2f * .55f ;
                break;
            case PlayerLifecycleStates.Dead:
                m_currentHealth = 0f;
                m_maximumHealth = 0f;

                m_currentStamina = 0f;
                m_maximumStamina = 0f;
                break;
        }
    }
    
    public void PlayHealSound()
    {
        if (m_healClipShuffleBag.Size == 0)
            InitializeHealShuffleBag();

        if (m_currentAudibleSoundPlaying != null)
        {
            if (m_currentAudibleSoundPlaying.isPlaying)
            {
                MMSoundManager.Instance.StopSound(m_currentAudibleSoundPlaying);
                m_currentAudibleSoundPlaying = null;
            }
            else
            {
                m_currentAudibleSoundPlaying = null;
            }
        }

        Tuple<int,AudioClip> playerAudioIdHealClipTuple = m_healClipShuffleBag.Pick();

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.Location = transform.position;
        options.ID = playerAudioIdHealClipTuple.Item1;

        m_currentAudibleSoundPlaying = MMSoundManagerSoundPlayEvent.Trigger(playerAudioIdHealClipTuple.Item2, options);
    }
    public void PlayHurtSound()
    {
        if (m_damageClipShuffleBag.Size == 0)
            InitializeDamageShuffleBag();

        if (m_currentAudibleSoundPlaying != null)
        {
            if (m_currentAudibleSoundPlaying.isPlaying)
            {
                MMSoundManager.Instance.StopSound(m_currentAudibleSoundPlaying);
                m_currentAudibleSoundPlaying = null;                
            }
            else
            {
                m_currentAudibleSoundPlaying = null;
            }
        }

        Tuple<int, AudioClip> playerAudioIdHurtClipTuple = m_damageClipShuffleBag.Pick();

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.Location = transform.position;
        options.ID = playerAudioIdHurtClipTuple.Item1;

        m_currentAudibleSoundPlaying = MMSoundManagerSoundPlayEvent.Trigger(playerAudioIdHurtClipTuple.Item2, options);
    }
    public void PlayDeathSound()
    {
        if (m_currentAudibleSoundPlaying != null)
        {
            if (m_currentAudibleSoundPlaying.isPlaying)
            {
                MMSoundManager.Instance.StopSound(m_currentAudibleSoundPlaying);
                m_currentAudibleSoundPlaying = null;
            }
            else
            {
                m_currentAudibleSoundPlaying = null;
            }
        }

        Tuple<int, AudioClip> playerAudioIdDeathClipTuple = MainManager.GameSoundStore.GetAudioClipAndAudioId("PLAYER:MAN_DAMAGE_EXTREME_1");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.Location = transform.position;
        options.ID = playerAudioIdDeathClipTuple.Item1;

        print($"here: PlayDeathSound");


        m_currentAudibleSoundPlaying = MMSoundManagerSoundPlayEvent.Trigger(playerAudioIdDeathClipTuple.Item2, options);
    }

    public bool HasEnoughStamina(float requiredStamina)
    {
        return m_currentStamina >= requiredStamina;
    }
    
    
    private void UpdateStaminaRegeneration()
    {
        if (m_staminaRegenerationRatePerSecond <= 0f)
        {
            return;
        }
        
        if (m_currentStamina >= m_maximumStamina)
        {
            m_isStaminaRegenerating = false;
            return;
        }
        
        m_timeSinceLastStaminaUse += Time.deltaTime;
        
        if (m_timeSinceLastStaminaUse >= m_staminaRegenerationDelaySeconds)
        {
            if (!m_isStaminaRegenerating)
            {
                m_isStaminaRegenerating = true;
            }
            
            float regenerationAmount = m_staminaRegenerationRatePerSecond * Time.deltaTime;
            RechargeStamina(regenerationAmount);
        }
    }
    
    private void UpdateLifecycleState()
    {
        if (IsDead)
        {
            return;
        }
        
        float healthPercentage = HealthPercentage;
        PlayerLifecycleStates newState = DetermineLifecycleStateFromHealth(healthPercentage);
        
        if (m_playerLifecycleStateMachine.CurrentState != newState)
        {
            ChangeStatePlayerLifecycle(newState);
        }
    }
    private PlayerLifecycleStates DetermineLifecycleStateFromHealth(float healthPercentage)
    {
        if (healthPercentage >= m_healthyStateThreshold)
        {
            return PlayerLifecycleStates.Healthy;
        }
        else if (healthPercentage >= m_hurtStateThreshold)
        {
            return PlayerLifecycleStates.Hurt;
        }
        else if (healthPercentage > 0f)
        {
            return PlayerLifecycleStates.Critical;
        }
        else
        {
            return PlayerLifecycleStates.Dead;
        }
    }


    public PlayerLifecycleStates GetCurrentPlayerLifecycleState()
    {
        return m_playerLifecycleStateMachine.CurrentState;
    }
    public void ChangeStatePlayerLifecycle(PlayerLifecycleStates playerLifecycleState)
    {
        m_playerLifecycleStateMachine.ChangeState(playerLifecycleState);

        switch (playerLifecycleState)
        {
            case PlayerLifecycleStates.Healthy:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_HEALTHY}", gameObject);
                Messenger<PlayerLifecycleStates>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", playerLifecycleState);
                break;
            case PlayerLifecycleStates.Hurt:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_HURT}", gameObject);
                Messenger<PlayerLifecycleStates>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", playerLifecycleState);
                break;
            case PlayerLifecycleStates.Critical:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_CRITICAL}", gameObject);
                Messenger<PlayerLifecycleStates>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", playerLifecycleState);
                break;
            case PlayerLifecycleStates.Dead:
                    Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_DEATH}", gameObject);
                    Messenger<PlayerLifecycleStates>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", playerLifecycleState);
                break;
        }
    }

    public PlayerInteractionStates GetCurrentPlayerInteractionState()
    {
        return m_playerInteractionStateMachine.CurrentState;
    }
    public void ChangeStateInteraction(PlayerInteractionStates playerInteractionState)
    {
        m_playerInteractionStateMachine.ChangeState(playerInteractionState);

        switch (playerInteractionState)
        {
            case PlayerInteractionStates.Idle:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_IDLE}", gameObject);
                break;
            case PlayerInteractionStates.AttemptingInteraction:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_ATTEMPTING_INTERACTION}", gameObject);
                break;
            case PlayerInteractionStates.Interacting:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_INTERACTING}", gameObject);
                break;
            case PlayerInteractionStates.Busy:
                Messenger<GameObject>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_PLAYER_INTERACTION_BUSY}", gameObject);
                break;
        }
    }

    private void OnStateMachineStateChanged()
    {
        Messenger<PlayerLifecycleStates>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_LIFE_CYCLE_STATE_CHANGED}", m_playerLifecycleStateMachine.CurrentState);
    }
    
    public void ResetPlayerToFullHealthAndStamina()
    {
        m_currentHealth = m_maximumHealth;
        m_currentStamina = m_maximumStamina;
        m_timeSinceLastStaminaUse = 0f;
        m_isStaminaRegenerating = false;
        
        ChangeStatePlayerLifecycle(PlayerLifecycleStates.Healthy);
        
        Messenger<float,float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", m_currentHealth, m_maximumHealth);
        Messenger<float,float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", m_currentStamina, m_maximumStamina);
    }
    
    private void OnDestroy()
    {
        if (m_playerLifecycleStateMachine != null)
        {
            m_playerLifecycleStateMachine.OnStateChange -= OnStateMachineStateChanged;
        }
    }

    private void InitializeLifecycleStateMachine()
    {
        m_playerLifecycleStateMachine = new MMStateMachine<PlayerLifecycleStates>(gameObject, true);
        ChangeStatePlayerLifecycle(PlayerLifecycleStates.Healthy);
        m_playerLifecycleStateMachine.OnStateChange += OnStateMachineStateChanged;
    }

    private void InitializeHealthAndStamina()
    {
        m_currentHealth = m_maximumHealth;
        m_currentStamina = m_maximumStamina;
        
        Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_HEALTH_CHANGED}", m_currentHealth, m_maximumHealth);
        Messenger<float, float>.Broadcast($"{PlayerId}{PlayerIndividualEvent.ON_STAMINA_CHANGED}", m_currentStamina, m_maximumStamina);
    }

    private void InitializeDamageShuffleBag()
    {
        for (int i = 1; i <= 5; i++)
        {
            string damageClipKey = $"PLAYER:MAN_DAMAGE_{i}";
            print(damageClipKey);
            Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId(damageClipKey);

            m_damageClipShuffleBag.Add(idWithAudioClip, 1);
        }
    }

    private void InitializeHealShuffleBag()
    {
        for (int i = 1; i <= 5; i++)
        {
            string healClipKey = $"PLAYER:MAN_FRUSTRATEDEXHALE_{i}";
            print(healClipKey);
            Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId(healClipKey);
            m_healClipShuffleBag.Add(idWithAudioClip, 1);
        }
    }

}

