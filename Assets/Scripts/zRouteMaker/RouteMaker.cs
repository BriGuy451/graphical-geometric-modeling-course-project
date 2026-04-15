using UnityEngine;
using System.Collections.Generic;
using MoreMountains.Tools;

/// <summary>
/// Attach to a empty game object, each mouse click adds the world position clicked to an array which keeps track of the current route. Press 0, when satisfied with the route being made, will complete that route and create space for a new one. If finished creating routes press 3 to save them to a file for future use.
/// </summary>
[RequireComponent(typeof(Camera))]
public class RouteMaker : MonoBehaviour
{

    [SerializeField] public int mapId;
    [SerializeField] public string fileName = "default_routes";
    private Camera m_camera;

    private int m_currentRouteIndex = 0;

    Dictionary<string, List<Vector3DTO>> currentRoutePositions = new Dictionary<string, List<Vector3DTO>>();

    private List<Vector3DTO> routePositions = new List<Vector3DTO>();
    private List<Vector3DTO> allRoutePositions = new List<Vector3DTO>();

    void Start()
    {
        // MMDebug.InstantiateOnScreenConsole();
        // saveloadmanagermethodjson needs to be set for newtonsoft, download package, and update code in managermethodjson
        MMSaveLoadManager.SaveLoadMethod = new MMSaveLoadManagerMethodJson();
        m_camera = transform.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3DTO enemyPos = GetClickedMousePosition();
            routePositions.Add(enemyPos);
            allRoutePositions.Add(enemyPos);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            MoveToNextRoute();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SaveRoutes();
        }
    }

    private Vector3DTO GetClickedMousePosition()
    {
        Vector3DTO enemyPos = new Vector3DTO();
        Ray mouseClickRay = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseClickRay, out RaycastHit mouseHit))
        {   
            Vector3 hitPosition = mouseHit.point;
            MMDebug.DrawPoint(hitPosition, Color.red, 10);
            print($"{hitPosition}");

            enemyPos = new Vector3DTO(hitPosition);
        }

        return enemyPos;
    }

    private void MoveToNextRoute()
    {
        print("Current Route Positions");
        foreach (Vector3DTO routePosition in routePositions)
        {
            print($"{routePosition}");
        }

        // Saving Patrol Routes
        // enemyRoutePositions[$"enemy_{m_currentRouteIndex}"] = routePositions;
        currentRoutePositions[$"{fileName}_{m_currentRouteIndex}_map_{mapId}"] = routePositions;

        print("Move to Next Route Position");
        routePositions = new List<Vector3DTO>();
        m_currentRouteIndex++;
    }

    private void SaveRoutes()
    {
        if (routePositions.Count > 0)
        {
            MoveToNextRoute();
        }

        print("Saving Routes Position");
        print(currentRoutePositions);
        MMSaveLoadManager.Save(currentRoutePositions, $"{fileName}_map_{mapId}.json");
        SaveAllRoutes();

        m_currentRouteIndex = 0;
        currentRoutePositions = new Dictionary<string, List<Vector3DTO>>();
    }

    private void SaveAllRoutes()
    {
        MMSaveLoadManager.Save(allRoutePositions, $"all_routes_{fileName}_map_{mapId}.json");
    }
}