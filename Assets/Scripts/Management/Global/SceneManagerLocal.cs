using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerLocal : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set;}
    public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.SceneManagerLocal;

    public int CurrentSceneIndex {get; private set;}

    public void StartUp()
    {
        LogFromMethod("StartUp");
        status = ManagerStatus.Initializing;

        status = ManagerStatus.Started;
    }

    // Will need to be moved to a Persistent SceneManager
    #region Scene Change Methods
    public void MoveToMainMenuScene(bool isInitial = false)
    {
        MoveToScene("StarterTemplateMainMenuScene", isInitial);
    }

    public void MoveToMainMenuSceneGeom(bool isInitial = false)
    {
        MoveToScene("MainMenuSceneGeom", isInitial);
    }
    
    public void MoveToVertexDissolveGeom()
    {
        MoveToScene("VertexDissolveSceneGeom");
    }
    
    public void MoveToFragmentDissolveGeom()
    {
        MoveToScene("VertexFragmentSceneGeom");
    }

    public void MoveToMaterialDissolveGeom()
    {
        MoveToScene("MaterialDissolveFragmentSceneGeom");
    }
    
    public void MoveToLightningStrikeGeom()
    {
        MoveToScene("LightningStrikeSceneGeom");
    }
    
    public void MoveToExplosionGeom()
    {
        MoveToScene("ExplosionSceneGeom");
    }
    
    public void MoveToImplosionGeom()
    {
        MoveToScene("ImplosionSceneGeom");
    }
    
    public void MoveToAuraGeom()
    {
        MoveToScene("AuraSceneGeom");
    }

    private void MoveToScene(string sceneName, bool isInitial = false)
    {
        if (IsSceneValidToLoad(sceneName))
        {    

            LogFromMethod($"{sceneName} is valid, loading now");

            if (isInitial)
            {
                StartCoroutine(CoroutineUtils.LoadSceneAndPerformAction(sceneName, () =>
                {
                    GeomManager.Instance.SceneChangeSetup();
                }));
            } else
            {
                GeomManager.UIManager.ChangeStateLoadingState(LoadingStates.BlockInputOverlay);
                StartCoroutine(CoroutineUtils.LoadSceneAndPerformAction(sceneName, () =>
                {
                    GeomManager.Instance.SceneChangeSetup();
                    GeomManager.UIManager.ChangeStateLoadingState(LoadingStates.None);
                }));
            }

        } else
        {
            LogFromMethod($"{sceneName} is not a valid scene, it may not be in the build profile");
        }
    }

    public bool IsSceneValidToLoad(string sceneName)
    {
        LogFromMethod($"IsSceneValidToLoad");
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCountInBuildSettings; sceneIndex++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                CurrentSceneIndex = sceneIndex;
                return true;
            }
        }

        return false;
    }
    
    #endregion

    #region IGameManager Methods

    public void Enable()
    {
        LogFromMethod("Enable");
        enabled = true;
    }
    
    public void Disable()
    {
        LogFromMethod("Disable");
        enabled = false;
    }

    public void SceneSetup(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetup");
    }

    public void SceneSetupAdditive(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetupAdditive");
    }

    #endregion

    public void OnDestroy()
    {
        LogFromMethod("OnDestroy");
    }

    public void LogFromMethod(string message)
    {
        Debug.Log($"SceneManagerLocal:{message}");
    }
}