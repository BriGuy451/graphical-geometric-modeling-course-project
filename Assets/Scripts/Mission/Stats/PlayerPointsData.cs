using System.Collections.Generic;

[System.Serializable]
public class PlayerPointsData
{
    public int m_overallScore;
    public Stack<int> m_playerPointsStack;
    
    public int m_customersFound;
    public int m_customersSaved;

    public int m_artifactsFound;
    public int m_artifactsRetrieved;
    
    public int m_enemyEncounters;
    public int m_trapsFound;
    
}