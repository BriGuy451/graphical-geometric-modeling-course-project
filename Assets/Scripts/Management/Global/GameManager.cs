using MoreMountains.Tools;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{	
	// Maze related properties
	private MMStateMachine<GameStateMaze> m_GameManagerStateMachine;

	public ManagerStatus status { get; private set; } = ManagerStatus.Shutdown;
	public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.GameManager;

  	// Use this for initialization
	public void StartUp()
	{
		LogFromMethod("StartUp");
		
		status = ManagerStatus.Initializing;
		
		m_GameManagerStateMachine = new MMStateMachine<GameStateMaze>(gameObject, false);
		
		status = ManagerStatus.Started;
	}

	public void StartGame()
	{

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

	private void LogFromMethod(string message)
	{
		print($"GameManager:{message}");
	}

}
