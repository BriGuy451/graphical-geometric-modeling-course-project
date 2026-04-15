using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using NaughtyAttributes;
using UnityEngine;

//Bring PersistentSingletonMainManager Script with This.
public class GeomManager : MMSingleton<GeomManager>
{    
    [SerializeField] private bool displayLogs = false;

    public static SceneManagerLocal SceneManagerLocal {get; private set;}
    public static UIManager UIManager {get; private set;}
    public static CameraManager CameraManager {get; private set;}

    private SceneConfiguration m_currentSceneConfiguration;
    public SceneConfiguration SceneConfiguration => m_currentSceneConfiguration;
    
    private List<IGameManager> managerStartSequence;
    private List<IGameManager> allManagersList;
    private Dictionary<ManagerIdentifier, bool> managerToEnabledStatusDict = new Dictionary<ManagerIdentifier, bool>();

    protected override void Awake()
    {
        LogFromMethod($"Awake");

        UIManager = GetComponent<UIManager>();
        SceneManagerLocal = GetComponent<SceneManagerLocal>();
        CameraManager = GetComponent<CameraManager>();

        allManagersList = new List<IGameManager>()
        {
            CameraManager,
            UIManager,
            SceneManagerLocal,
        };

        InitializeManagerEnabledDictionary();

        UpdateSceneConfiguration();

        managerStartSequence = GetStartSequence(m_currentSceneConfiguration.currentScene);

        StartCoroutine(StartupManagers());
    }

    public void OnDestroy()
    {
        LogFromMethod("OnDestroy");
    }

    public void InitializeManagerEnabledDictionary()
    {
        foreach (IGameManager gameManager in allManagersList)
        {
            managerToEnabledStatusDict.Add(gameManager.managerIdentifier, false);
        }
    }

    private void ChangeToStartingUI(SceneCurrent currentScene)
    {
        LogFromMethod("ChangeToStartingUI");
        switch (currentScene)
        {
            case SceneCurrent.MainSceneGeom:
                UIManager.ChangeStateComplexView(UIComplexStates.MainMenuScreenGeom);
                UIManager.ChangeStateLoadingState(LoadingStates.None);
                break;
        }
    }

    public List<IGameManager> GetStartSequence(SceneCurrent currentScene)
    {
        List<IGameManager> gameManagers;

        switch (currentScene)
        {
            // Startup Scene must only have the SceneManagerLocal
            case SceneCurrent.GeomStartUpScene:
                gameManagers = new List<IGameManager>()
                {
                  SceneManagerLocal,
                };
                break;
            case SceneCurrent.MainSceneGeom:
                gameManagers = new List<IGameManager>()
                {
                  SceneManagerLocal,
                  CameraManager,
                  UIManager,
                };
                break;
            case SceneCurrent.VertexDissolveSceneGeom:
                gameManagers = new List<IGameManager>()
                {
                  SceneManagerLocal,
                  CameraManager,
                  UIManager,
                };
                break;
            default:
                gameManagers = new List<IGameManager>()
                {
                  SceneManagerLocal,
                  CameraManager,
                  UIManager,
                };
                break;
        }

        LogFromMethod($"GetStartSequence: {gameManagers.Count}");

        return gameManagers;
    }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in managerStartSequence)
        {
            manager.Enable();
            managerToEnabledStatusDict[manager.managerIdentifier] = true;
            manager.StartUp();
        }

        yield return null;

        int numModules = managerStartSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in managerStartSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }

            if (numReady > lastReady)
            {
                // Have an event for this?
                print($"Progress: {numReady}/{numModules}");
                print($"MainManager:All Managers Started");
            }

            yield return null;
        }

        SceneManagerLocal.MoveToMainMenuSceneGeom(true);
    }

    public void UpdateSceneConfiguration()
    {
        LogFromMethod("UpdateSceneConfiguration");
        m_currentSceneConfiguration = FindFirstObjectByType<SceneConfiguration>();
        LogFromMethod($"CurrentScene {m_currentSceneConfiguration.currentScene}");
    }

    [Button("Find Scene Configs", EButtonEnableMode.Playmode)]
    public void FindSceneConfigs()
    {
        m_currentSceneConfiguration = FindFirstObjectByType<SceneConfiguration>();
        LogFromMethod($"CurrentScene {m_currentSceneConfiguration.currentScene}");
    }

    public void SceneChangeSetup()
    {
        UpdateSceneConfiguration();

        LogFromMethod("SceneChangeSetup");
        LogFromMethod($"{m_currentSceneConfiguration.uiStateEntityToggleLists.ToString()}");


        SceneConfiguration sceneConfiguration = m_currentSceneConfiguration;

        List<IGameManager> startUpSequence = GetStartSequence(sceneConfiguration.currentScene);

        if (startUpSequence.Count != allManagersList.Count)
        {
            // means some managers need to be disabled
            List<IGameManager> disabledManagers = new List<IGameManager>();
            foreach (IGameManager gameManager in allManagersList)
            {
                bool shouldDisable = false;
                
                foreach (IGameManager startUpSequenceGameManager in startUpSequence)
                {
                    if (startUpSequenceGameManager.managerIdentifier == gameManager.managerIdentifier)
                    {
                        shouldDisable = false;
                        break;
                    }

                    shouldDisable = true;
                }

                if (shouldDisable)
                {
                    managerToEnabledStatusDict[gameManager.managerIdentifier] = false;
                    gameManager.Disable();
                    disabledManagers.Add(gameManager);
                }
            }

            LogFromMethod($"DisabledManagers: {disabledManagers.Count}");
        }

        List<IGameManager> enabledManagers = new List<IGameManager>();
        foreach (IGameManager gameManager in startUpSequence)
        {
            if (managerToEnabledStatusDict.ContainsKey(gameManager.managerIdentifier))
            {
                bool isEnabled = managerToEnabledStatusDict[gameManager.managerIdentifier];
                if (isEnabled)
                {
                    // Call the managers scene change method
                    gameManager.SceneSetup(sceneConfiguration);
                } else
                {
                    gameManager.Enable();
                    gameManager.StartUp();
                    managerToEnabledStatusDict[gameManager.managerIdentifier] = true;

                    enabledManagers.Add(gameManager);
                }
            }
        }

        LogFromMethod($"Newly EnabledManagers: {enabledManagers.Count}");

        ChangeToStartingUI(sceneConfiguration.currentScene);
    }

    private void LogFromMethod(string message)
    {
        if (displayLogs)
            print($"GeomManager:{message}");
    }

}