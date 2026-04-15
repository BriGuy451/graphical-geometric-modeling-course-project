using MoreMountains.Tools;
using UnityEngine;

public class CameraManager : MonoBehaviour, IGameManager {

    [SerializeField] private SceneCameraMap sceneCameraMap;

    [SerializeField] private MMToggleActive mainCameraToggle;
    [SerializeField] private MMToggleActive sceneCameraToggle;

    public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.CameraManager;

    public ManagerStatus status { get; }

    public void StartUp()
    {
        LogFromMethod("StartUp");
    }

    public void DisableSceneCamera()
    {
        sceneCameraToggle.ToggleActive();
    }

    public void DisableMainCamera()
    {
        mainCameraToggle.ToggleActive();
    }

    public void SceneSetup(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetup");
    }
    public void SceneSetupAdditive(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetupAdditive");
    }

    public void Enable()
    {
        enabled = true;
    }
    public void Disable()
    {
        enabled = false;
    }

    private void LogFromMethod(string logMessage)
    {
        Debug.Log($"CameraManager: {logMessage}");
    }
}