using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class ImplosionShaderSimple : MonoBehaviour {
    private Mesh _meshToImplode;
    private Material _implosionMaterial;
    private Vector3 _convergencePoint = Vector3.zero;

    [SerializeField] private float Duration = 1f;

    private bool _isStarted = false;
    private float _elapsedTime = 0f;
    private float _interpolator = 0f;
    private float _timeEachAnimation = 0f;
    
    void Start()
    {
        MeshFilter meshFilter = null;
        SkinnedMeshRenderer skinnedMeshRenderer = null;
        if (TryGetComponent<MeshFilter>(out meshFilter))
        {
            _meshToImplode = meshFilter.mesh;
            _implosionMaterial = GetComponent<MeshRenderer>().material;
        } else if (TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
        {
            _meshToImplode = skinnedMeshRenderer.sharedMesh;
            _implosionMaterial = skinnedMeshRenderer.material;
        }

        if (_meshToImplode == null)
        {
            _meshToImplode = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            _implosionMaterial = GetComponent<SkinnedMeshRenderer>().material;
        }

        Vector2[] implosionUVs = new Vector2[_meshToImplode.vertexCount];
        foreach (int vertexIndex in _meshToImplode.GetIndices(0))
        {
            implosionUVs[vertexIndex] = new Vector2(GetRandomWholeNumberBetweenOneAndThree(), 0f);
        }
        _meshToImplode.SetUVs(2, implosionUVs);

        Bounds bounds = _meshToImplode.bounds;

        if (skinnedMeshRenderer != null)
        {
            _convergencePoint = new Vector3(0f, 0.8f, 0);
        } else
        {    
            _convergencePoint = bounds.center;
            // _convergencePoint = Vector3.zero;
            // _convergencePoint = new Vector3(0f, 0.5f, 0);
        }


        _implosionMaterial.SetVector("_ConvergencePoint", _convergencePoint);
        _implosionMaterial.SetFloat("_AnimationThreshold", 0f);

        _timeEachAnimation = Duration / 3f;
    }

    void Update()
    {
        if (_isStarted)
        {
            _elapsedTime += Time.deltaTime;
        }
    }

    [Button("Play Implosion", EButtonEnableMode.Playmode)]
    public void PlayImplosion()
    {
        _isStarted = true;
        // StartCoroutine(PlayImplosionSequential());
        StartCoroutine(PlayImplosionEven());

    }

    public void Play()
    {
        PlayImplosion();
    }
    
    [Button("Reset", EButtonEnableMode.Playmode)]
    public void Reset()
    {
        _elapsedTime = 0f;
        _isStarted = false;
        _interpolator = 0f;

        _implosionMaterial.SetFloat("_ChannelOneAnimation", 0);
        _implosionMaterial.SetFloat("_ChannelTwoAnimation", 0);
        _implosionMaterial.SetFloat("_ChannelThreeAnimation", 0);
    }

    private IEnumerator PlayImplosionSequential()
    {

        
        while (_implosionMaterial.GetFloat("_ChannelOneAnimation") < 1)
        {
            _interpolator = _elapsedTime / _timeEachAnimation;
            float newAnimationValue = Mathf.Lerp(0, 1, _interpolator);
            _implosionMaterial.SetFloat("_ChannelOneAnimation", newAnimationValue);
            yield return null;
        }
        
        _elapsedTime = 0f;
        _interpolator = 0f;
        while (_implosionMaterial.GetFloat("_ChannelTwoAnimation") < 1)
        {
            _interpolator = _elapsedTime / _timeEachAnimation;
            float newAnimationValue = Mathf.Lerp(0, 1, _interpolator);
            _implosionMaterial.SetFloat("_ChannelTwoAnimation", newAnimationValue);
            yield return null;
        }
        
        _elapsedTime = 0f;
        _interpolator = 0f;
        while (_implosionMaterial.GetFloat("_ChannelThreeAnimation") < 1)
        {
            _interpolator = _elapsedTime / _timeEachAnimation;
            float newAnimationValue = Mathf.Lerp(0, 1, _interpolator);
            _implosionMaterial.SetFloat("_ChannelThreeAnimation", newAnimationValue);
            yield return null;
        }

        _isStarted = false;
    }
    
    private IEnumerator PlayImplosionEven()
    {

        float interpolatorOne;
        float interpolatorTwo;
        float interpolatorThree;
        while (_implosionMaterial.GetFloat("_ChannelOneAnimation") < 1)
        {
            interpolatorOne = _elapsedTime / _timeEachAnimation;
            float newAnimationValue = Mathf.Lerp(0, 1, interpolatorOne);
            _implosionMaterial.SetFloat("_ChannelOneAnimation", newAnimationValue);

            yield return new WaitForSeconds(.05f);

            interpolatorTwo = _elapsedTime / _timeEachAnimation;
            newAnimationValue = Mathf.Lerp(0, 1, interpolatorTwo);
            _implosionMaterial.SetFloat("_ChannelTwoAnimation", newAnimationValue);
            
            yield return new WaitForSeconds(.05f);

            interpolatorThree = _elapsedTime / _timeEachAnimation;
            newAnimationValue = Mathf.Lerp(0, 1, interpolatorThree);
            _implosionMaterial.SetFloat("_ChannelThreeAnimation", newAnimationValue);
            
            yield return new WaitForSeconds(.05f);
        }
        
        _isStarted = false;
    }

    private int GetRandomWholeNumberBetweenOneAndThree()
    {
        return Random.Range(1, 4);
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"ImplosionShaderSimple: {message}");
    }
}