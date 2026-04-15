using NaughtyAttributes;
using UnityEngine;

public class RunAllForward : MonoBehaviour
{
    
    [Button("RunAllForward", EButtonEnableMode.Playmode)]
    public void RunForward()
    {
        BroadcastMessage("SetRunForward");
    }

    [Button("RunAllBackward", EButtonEnableMode.Playmode)]
    public void RunBackward()
    {
        BroadcastMessage("SetRunBackward");
    }
    
    [Button("PauseAll", EButtonEnableMode.Playmode)]
    public void PauseAll()
    {
        BroadcastMessage("Pause");
    }
    [Button("Play", EButtonEnableMode.Playmode)]
    public void PlayAll()
    {
        BroadcastMessage("Play");
    }

    [Button("RunAllLoop", EButtonEnableMode.Playmode)]
    public void SetLooping()
    {
        BroadcastMessage("RunLooping");
    }
    
    [Button("RunAllLoop", EButtonEnableMode.Playmode)]
    public void ResetAll()
    {
        BroadcastMessage("Reset");
    }


}