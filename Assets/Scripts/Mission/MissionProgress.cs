using System;

[Serializable]
public class MissionProgress
{
    public int m_pillarsActivated = 0;
    public int m_customersSaved = 0;
    public int m_artifactsRetrieved = 0;
    private MissionObjective m_missionObjective;

    public bool IsAllPillarsActivated => m_pillarsActivated == m_missionObjective.m_pillarsToActivate;
    public bool IsAllCustomersSaved => m_customersSaved == m_missionObjective.m_customersToSave;
    public bool IsAllArtifactsRetrieved => m_artifactsRetrieved == m_missionObjective.m_artifactsToRetrieve;

    public Action OnPillarActivated;
    public Action OnAllPillarsActivated;
    public Action OnCustomerSaved;
    public Action OnAllCustomerSaved;
    public Action OnArtifactRetrieved;
    public Action OnAllArtifactRetrieved;

    public MissionProgress(MissionObjective missionObjective)
    {
        m_pillarsActivated = 0;
        m_customersSaved = 0;
        m_artifactsRetrieved = 0;
        m_missionObjective = missionObjective;
    }

    public void IncrementPillarsActivated()
    {
        m_pillarsActivated++;

        if (m_pillarsActivated == m_missionObjective.m_pillarsToActivate)
        {
            OnPillarActivated?.Invoke();
            OnAllPillarsActivated?.Invoke();
        } else
        {
            OnPillarActivated?.Invoke();
        }
    }
    public void IncrementCustomerSaved()
    {
        m_customersSaved++;

        if (m_customersSaved == m_missionObjective.m_customersToSave)
        {
            OnCustomerSaved?.Invoke();
            OnAllCustomerSaved?.Invoke();
        } else
        {
            OnCustomerSaved?.Invoke();
        }
    }
    public void IncrementArtifactRetrieved()
    {
        m_artifactsRetrieved++;

        if (m_artifactsRetrieved == m_missionObjective.m_artifactsToRetrieve)
        {
            OnArtifactRetrieved?.Invoke();
            OnAllArtifactRetrieved?.Invoke();
        } else
        {
            OnArtifactRetrieved?.Invoke();
        }
    }

    public float GetCurrentPillarActivatedProgress()
    {
        return m_pillarsActivated / m_missionObjective.m_pillarsToActivate;
    }
    public float GetCurrentCustomerSavedProgress()
    {
        return m_customersSaved / m_missionObjective.m_customersToSave;
    }
    public float GetCurrentArtifactRetrievedProgress()
    {
        return m_artifactsRetrieved / m_missionObjective.m_artifactsToRetrieve;
    }

}