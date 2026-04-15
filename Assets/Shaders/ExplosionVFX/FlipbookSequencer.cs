using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class FlipbookSequencer : MonoBehaviour {
    
    private FlipbookPlayer[] _flipbookPlayers = new FlipbookPlayer[0];
    public bool isComplete = false;

    void Start()
    {
        _flipbookPlayers = GetComponentsInChildren<FlipbookPlayer>();

        foreach (FlipbookPlayer flipbookPlayer in _flipbookPlayers)
            flipbookPlayer.DisableMeshRenderer();
    }

    [Button("PlayFlipbookSequence", EButtonEnableMode.Playmode)]
    private void PlayFlipbookSequence()
    {
        StartCoroutine(PlayFlipbooks());
    }

    private IEnumerator PlayFlipbooks()
    {
        foreach (FlipbookPlayer flipbookPlayer in _flipbookPlayers)
        {
            flipbookPlayer.EnableMeshRenderer();
            flipbookPlayer.PlayFlipbook();

            while (!flipbookPlayer._isComplete)
                yield return null;

            flipbookPlayer.DisableMeshRenderer();
        }

        isComplete = true;
    }
}