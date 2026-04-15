# if UNITY_EDITOR
using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

public class LevelConfigurationCreator
{
    [MenuItem("Assets/Create/Level Configuration Programmatically")]
    public static void CreateLevelConfigurationFromFile()
    {
        string fileName;
        // fileName = "enemy_top_left_routes_map_1.json";
        // fileName = "enemy_top_right_routes_map_1.json";
        // fileName = "enemy_bottom_right_routes_map_1.json";
        fileName = "enemy_bottom_left_routes_map_1.json";

        Guid guid = Guid.NewGuid();

        MMSaveLoadManager.SaveLoadMethod = new MMSaveLoadManagerMethodJson();

        Dictionary<string,List<Vector3DTO>> entityToPatrolRoutes = (Dictionary<string, List<Vector3DTO>>)MMSaveLoadManager.Load(typeof(Dictionary<string, List<Vector3DTO>>), fileName);

        LevelConfigurationSO levelConfiguration = ScriptableObject.CreateInstance<LevelConfigurationSO>();
        levelConfiguration.enemyProperties = new List<EnemyProperties>();
        
        foreach (KeyValuePair<string, List<Vector3DTO>> entityToRoute in entityToPatrolRoutes){
            
            Vector3 spawnPosition = new Vector3(entityToRoute.Value[0].x, 2, entityToRoute.Value[0].z);

            List <Vector3> routeVectorList = new List<Vector3>();
            foreach(Vector3DTO position in entityToRoute.Value)
            {
                routeVectorList.Add(new Vector3(position.x, 2, position.z));
            }
            
            EnemyProperties enemyProperties = new EnemyProperties(spawnPosition, routeVectorList);
            levelConfiguration.enemyProperties.Add(enemyProperties);
        }

        AssetDatabase.CreateAsset(levelConfiguration, $"Assets/ScriptableObjects/LevelConfiguration_{guid}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = levelConfiguration;
    }

}

#endif