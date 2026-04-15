[System.Serializable]
public class MissionObjective {

    public int m_pillarsToActivate = 0;
    public int m_customersToSave = 0;
    public int m_artifactsToRetrieve = 0;

    public MissionObjective(int pillarsToActivate, int customersToSave, int artifactsToRetrieve)
    {
        m_pillarsToActivate = pillarsToActivate;
        m_customersToSave = customersToSave;
        m_artifactsToRetrieve = artifactsToRetrieve;
    }
}