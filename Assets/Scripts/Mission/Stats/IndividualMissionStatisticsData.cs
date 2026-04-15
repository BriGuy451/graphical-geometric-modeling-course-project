using UnityEngine;

/// <summary>
/// Data structure that holds all statistics collected during a mission for a single player character.
/// This class stores the raw numerical data that is tracked by the IndividualMissionStatisticsTracker.
/// </summary>
[System.Serializable]
public class IndividualMissionStatisticsData
{
    [Header("Enemy Encounter Statistics")]
    [Tooltip("Total number of enemies encountered during the mission")]
    [SerializeField] private int m_totalEnemiesEncountered = 0;
    
    [Header("Customer Statistics")]
    [Tooltip("Total number of customers found during the mission")]
    [SerializeField] private int m_totalCustomersFound = 0;
    
    [Tooltip("Total number of customers successfully saved during the mission")]
    [SerializeField] private int m_totalCustomersSaved = 0;
    
    [Header("Artifact Statistics")]
    [Tooltip("Total number of artifacts found during the mission")]
    [SerializeField] private int m_totalArtifactsFound = 0;
    
    [Tooltip("Total number of artifacts successfully retrieved during the mission")]
    [SerializeField] private int m_totalArtifactsRetrieved = 0;
    
    /// <summary>
    /// Gets the total number of enemies encountered
    /// </summary>
    public int TotalEnemiesEncountered => m_totalEnemiesEncountered;
    
    /// <summary>
    /// Gets the total number of customers found
    /// </summary>
    public int TotalCustomersFound => m_totalCustomersFound;
    
    /// <summary>
    /// Gets the total number of customers saved
    /// </summary>
    public int TotalCustomersSaved => m_totalCustomersSaved;
    
    /// <summary>
    /// Gets the total number of artifacts found
    /// </summary>
    public int TotalArtifactsFound => m_totalArtifactsFound;
    
    /// <summary>
    /// Gets the total number of artifacts retrieved
    /// </summary>
    public int TotalArtifactsRetrieved => m_totalArtifactsRetrieved;
    
    /// <summary>
    /// Increments the enemy encounter counter
    /// </summary>
    public void IncrementEnemyEncounterCount()
    {
        m_totalEnemiesEncountered++;
    }
    
    /// <summary>
    /// Increments the customer found counter
    /// </summary>
    public void IncrementCustomerFoundCount()
    {
        m_totalCustomersFound++;
    }
    
    /// <summary>
    /// Increments the customer saved counter
    /// </summary>
    public void IncrementCustomerSavedCount()
    {
        m_totalCustomersSaved++;
    }
    
    /// <summary>
    /// Increments the artifact found counter
    /// </summary>
    public void IncrementArtifactFoundCount()
    {
        m_totalArtifactsFound++;
    }
    
    /// <summary>
    /// Increments the artifact retrieved counter
    /// </summary>
    public void IncrementArtifactRetrievedCount()
    {
        m_totalArtifactsRetrieved++;
    }
    
    /// <summary>
    /// Resets all statistics to zero
    /// </summary>
    public void ResetAllStatistics()
    {
        m_totalEnemiesEncountered = 0;
        m_totalCustomersFound = 0;
        m_totalCustomersSaved = 0;
        m_totalArtifactsFound = 0;
        m_totalArtifactsRetrieved = 0;
    }
    
    /// <summary>
    /// Gets a formatted string summary of all statistics
    /// </summary>
    /// <returns>A string containing all mission statistics</returns>
    public string GetStatisticsSummary()
    {
        return $"Enemies Encountered: {m_totalEnemiesEncountered}\n" +
               $"Customers Found: {m_totalCustomersFound}\n" +
               $"Customers Saved: {m_totalCustomersSaved}\n" +
               $"Artifacts Found: {m_totalArtifactsFound}\n" +
               $"Artifacts Retrieved: {m_totalArtifactsRetrieved}";
    }
}

