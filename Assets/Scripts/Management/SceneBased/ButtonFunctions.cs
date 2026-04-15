using Cinemachine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vertexSpotcam;
    [SerializeField] private RunAllForward runAllForward;
    [SerializeField] private MMToggleActive playToggle;
    [SerializeField] private MMToggleActive pauseToggle;
    [SerializeField] private ExplosionVFX explosionVFX;
    [SerializeField] private LightningVFXSequenceShader lightningVFX;
    [SerializeField] private ChangeSmokeColor _ChangeSmokeColor;
    [SerializeField] private MMToggleActive _DirectionalLightToggle;

    #region Application Functions

    public void QuitApplication()
    {
        LogFromMethod("QuitApplication");
        Application.Quit();
    }

    public void ReplayGame()
    {
        LogFromMethod("ReplayGame");
        SceneManager.LoadScene("Scenes/MazeExperimentScene");
    }

    public void StartGame()
    {
        LogFromMethod("StartGame");
    }

    #endregion

    #region User Interface Functions
    public void SwitchToCreateHostRoomFormScreen()
    {
        MainManager.UIManager.ChangeStateComplexView(UIComplexStates.PlayerCreateHostRoomFormScreen);
    }
    public void SwitchToClientRoomListScreen()
    {
        MainManager.UIManager.ChangeStateComplexView(UIComplexStates.PlayerClientRoomListScreen);
    }
    public void SwitchToLobbyHostScreen()
    {
        MainManager.UIManager.ChangeStateComplexView(UIComplexStates.PlayerLobbyHostRoomScreen);
    }
    public void SwitchToLobbyClientScreen()
    {
        MainManager.UIManager.ChangeStateComplexView(UIComplexStates.PlayerLobbyClientRoomScreen);
    }
    public void SwitchBackToMainMenuScreen()
    {
        MainManager.UIManager.ChangeStateComplexView(UIComplexStates.MainMenuScreen);
    }
    #endregion

    #region Scene Related

    public void MoveToMainMenuScene()
    {
        MainManager.SceneManagerLocal.MoveToMainMenuScene();
    }

    public void MoveToMainMenuSceneGeom()
    {
        GeomManager.SceneManagerLocal.MoveToMainMenuSceneGeom();
    }
    
    public void MoveToVertexDissolveGeom()
    {
        GeomManager.SceneManagerLocal.MoveToVertexDissolveGeom();
    }
    
    public void MoveToFragmentDissolveGeom()
    {
        GeomManager.SceneManagerLocal.MoveToFragmentDissolveGeom();
    }
    
    public void MoveToMaterialDissolveGeom()
    {
        GeomManager.SceneManagerLocal.MoveToMaterialDissolveGeom();
    }
    
    public void MoveToLightningStrikeGeom()
    {
        GeomManager.SceneManagerLocal.MoveToLightningStrikeGeom();
    }
    
    public void MoveToExplosionGeom()
    {
        GeomManager.SceneManagerLocal.MoveToExplosionGeom();
    }

    public void MoveToImplosionScene()
    {
        GeomManager.SceneManagerLocal.MoveToImplosionGeom();        
    }
    
    public void MoveToAuraScene()
    {
        GeomManager.SceneManagerLocal.MoveToAuraGeom();        
    }

    #endregion

    public void ToggleSpotCam()
    {
        if (vertexSpotcam != null)
            vertexSpotcam.enabled = !vertexSpotcam.enabled;
    }

    public void PlayExplosion()
    {
        if (explosionVFX != null)
            explosionVFX.PlayExplosionVFX();
    }
    
    public void ResetExplosion()
    {
        if (explosionVFX != null)
            explosionVFX.ResetExplosionVFX();
    }

    public void PlayLightningVFX()
    {
        if (lightningVFX != null)
            lightningVFX.PlayLightningEffect();
    }

    public void RunAllForward()
    {
        if (runAllForward == null || playToggle == null || pauseToggle == null)
            return;
        
        runAllForward.PlayAll();
        
        playToggle.ToggleActive();
        pauseToggle.ToggleActive();

        runAllForward.RunForward();
    }
    public void RunAllBackward()
    {
        if (runAllForward == null)
            return;
        
        runAllForward.RunBackward();
    }
    public void SetLooping()
    {
        if (runAllForward == null)
            return;
        
        runAllForward.SetLooping();
    }

    public void PlayImplosions()
    {
        runAllForward.PlayAll();
    }
    
    public void ResetImplosions()
    {
        runAllForward.ResetAll();
    }

    public void SetPause()
    {
        if (runAllForward == null || playToggle == null || pauseToggle == null)
            return;
        
        pauseToggle.ToggleActive();
        playToggle.ToggleActive();

        runAllForward.PauseAll();
    }

    public void ChangeColorAura()
    {
        if (_ChangeSmokeColor != null)
        {
            _ChangeSmokeColor.ChangeColor();
        }
    }

    public void ToggleDirectionalLight()
    {
        if (_DirectionalLightToggle != null)
        {
            _DirectionalLightToggle.ToggleActive();
        }
    }

    public void LobbyStartMissionButton()
    {
        MainManager.PlayersManager.SpawnAvailablePlayers();
    }

    public void DoNothingPlaceholder()
    {
        print($"Doing nothing, placeholder button function");
    }

    private void LogFromMethod(string message)
    {
        print($"ButtonFunctions:{message}");
    }
}