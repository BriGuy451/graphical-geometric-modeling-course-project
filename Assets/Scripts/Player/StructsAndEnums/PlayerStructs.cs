using System;

[System.Serializable]
public struct PlayerSpecifications
{

    public Guid playerId;

    public string name;
    public Skin skin;

    public int currentHealth;
    public int maxHealth;
    public int currentStamina;
    public int maxStamina;

    public PlayerLifecycleStates lifecycleStates;
    public PlayerLocomotionStates locomotionStates;
    public PlayerInteractionStates interactionStates;

}