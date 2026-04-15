using NaughtyAttributes;
using UnityEngine;

public class StoneEffectSequencer : MonoBehaviour {
    

    private Vector3 _initialPos = Vector3.zero;
    private StoneExplosion[] _stoneExplosion;
    private MeshRenderer[] _meshRenderers;
    private Collider[] _colliders;

    void Start()
    {
        _stoneExplosion = GetComponentsInChildren<StoneExplosion>();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _colliders = GetComponentsInChildren<Collider>();

        DisableColliders();
    }

    [Button("PlayExplosions", EButtonEnableMode.Playmode)]
    public void PlayExplosions()
    {
        foreach (StoneExplosion stoneExplosion in _stoneExplosion)
            stoneExplosion.ApplyDirectionalForce();
    }
    
    [Button("ResetExplosions", EButtonEnableMode.Playmode)]
    public void ResetExplosions()
    {
        DisableColliders();
        DisableMeshRenderers();
        foreach (StoneExplosion stoneExplosion in _stoneExplosion)
            stoneExplosion.ResetAllPositions();
    }

    public void EnableMeshRenderers()
    {
        if (_meshRenderers == null)
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in _meshRenderers)
            meshRenderer.enabled = true;
    }
    public void DisableMeshRenderers()
    {
        if (_meshRenderers == null)
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in _meshRenderers)
            meshRenderer.enabled = false;
    }

    public void DisableColliders()
    {
        if (_colliders == null)
            _colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in _colliders)
            collider.enabled = false;
    }
    
    public void EnableColliders()
    {
        if (_colliders == null)
            _colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in _colliders)
            collider.enabled = true;
    }

}