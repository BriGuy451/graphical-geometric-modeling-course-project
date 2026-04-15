using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelConfigurationSO")]
public class LevelConfigurationSO : ScriptableObject
{    
    public List<EnemyProperties> enemyProperties;

}