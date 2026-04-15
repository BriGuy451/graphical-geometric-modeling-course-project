using System.Collections.Generic;

[System.Serializable]
public class TeamPointsData
{
    public int m_overallScore;
    public Stack<int> m_teamPointsStack;

    public int m_customersSaved;
    public int m_artifactsRetrieved;
}