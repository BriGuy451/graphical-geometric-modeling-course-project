public interface IGameManager
{
    ManagerIdentifier managerIdentifier {get;}
    ManagerStatus status {get;}

    void Enable();
    void Disable();
    void StartUp();
    void SceneSetup(SceneConfiguration sceneConfiguration);
    void SceneSetupAdditive(SceneConfiguration sceneConfiguration);
}