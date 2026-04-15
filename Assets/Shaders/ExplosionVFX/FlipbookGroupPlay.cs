using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FlipbookGroupPlay : MonoBehaviour {
    private List<FlipbookPlayer> _flipbookPlayers = new List<FlipbookPlayer>();
    private MeshRenderer[] _meshRenderers;

    [SerializeField] private bool PlayOnStart = true;

    void Start()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        DisableMeshRenderers();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);

            FlipbookPlayer childFlipbookPlayer = childTransform.GetComponent<FlipbookPlayer>();

            if (childFlipbookPlayer == null)
            {
                childFlipbookPlayer = childTransform.gameObject.AddComponent<FlipbookPlayer>();
            }

            _flipbookPlayers.Add(childFlipbookPlayer);
        }

        // if (PlayOnStart)
        //     PlayFlipbooks();
    }

    private bool isPlaying = false;
    void Update()
    {
        if (PlayOnStart && !isPlaying)
        {
            PlayFlipbooks();
            isPlaying = true;
        }
    }

    [Button("PlayFlipbooks", EButtonEnableMode.Playmode)]
    public void PlayFlipbooks()
    {
        EnableMeshRenderers();
        foreach (FlipbookPlayer flipbookPlayer in _flipbookPlayers)
        {
            flipbookPlayer.PlayFlipbook();
        }
    }

    [Button("StopFlipbooks", EButtonEnableMode.Playmode)]
    public void StopFlipbooks()
    {
        DisableMeshRenderers();
        foreach (FlipbookPlayer flipbookPlayer in _flipbookPlayers)
        {
            flipbookPlayer.StopFlipbook();
        }
    }

    public List<FlipbookPlayer> GetFlipbookPlayers()
    {
        return _flipbookPlayers;
    }

    [Button("SetGroupRandomIndex", EButtonEnableMode.Playmode)]
    public void SetGroupToRandomStartIndex()
    {
        foreach (FlipbookPlayer flipbookPlayer in _flipbookPlayers)
            flipbookPlayer._startAtRandomIndex = true;
    }

    public void DisableMeshRenderers()
    {
        if (_meshRenderers == null)
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in _meshRenderers)
            meshRenderer.enabled = false;
    }
    public void EnableMeshRenderers()
    {
        if (_meshRenderers == null)
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in _meshRenderers)
            meshRenderer.enabled = true;
    }

}