using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class ExplosionVFX : MonoBehaviour {
    
    private MeshRenderer[] _meshRenderers;

    private MeshRenderer _barrelMR;
    private Collider _barrelCollider;
    private StoneEffectSequencer _stonesEffectSequencer;
    
    private FlipbookPlayer _flameFlipbookPlayer;
    private FlipbookPlayer _explosionFlipbookPlayer;

    void Start()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        FlipbookPlayer[] flipbooks = GetComponentsInChildren<FlipbookPlayer>();

        _barrelMR = _meshRenderers[1];
        _barrelCollider = GetComponentsInChildren<Collider>()[0];
        _stonesEffectSequencer = GetComponentInChildren<StoneEffectSequencer>();
        _stonesEffectSequencer.DisableMeshRenderers();

        _flameFlipbookPlayer = flipbooks[0];
        _explosionFlipbookPlayer = flipbooks[1];
    }

    [Button("PlayExplosionVFX", EButtonEnableMode.Playmode)]
    public void PlayExplosionVFX()
    {
        StartCoroutine(PlayExplosionVFXSequence());        
    }
    
    [Button("ResetExplosionVFX", EButtonEnableMode.Playmode)]
    public void ResetExplosionVFX()
    {
        _barrelMR.enabled = true;
        _barrelCollider.enabled = true;
        _stonesEffectSequencer.ResetExplosions();
    }

    private IEnumerator PlayExplosionVFXSequence()
    {
        _flameFlipbookPlayer._looping = true;
        _flameFlipbookPlayer.EnableMeshRenderer();
        _flameFlipbookPlayer.PlayFlipbook();

        yield return new WaitForSeconds(3f);

        _flameFlipbookPlayer._looping = false;
        _flameFlipbookPlayer.DisableMeshRenderer();

        _barrelCollider.enabled = false;
        _barrelMR.enabled = false;

        _explosionFlipbookPlayer.EnableMeshRenderer();
        _explosionFlipbookPlayer.PlayFlipbook();

        _stonesEffectSequencer.EnableMeshRenderers();
        _stonesEffectSequencer.EnableColliders();
        _stonesEffectSequencer.PlayExplosions();

        yield return new WaitForSeconds(.5f);

        while (!_explosionFlipbookPlayer._isComplete)
            yield return null;
        
        _explosionFlipbookPlayer.DisableMeshRenderer();
    }

}