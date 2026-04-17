using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class FlipbookPlayer : MonoBehaviour {
    
    [SerializeField] public float _flipbookDuration = 1f;
    [SerializeField] public bool _looping;
    [SerializeField] public bool _startAtRandomIndex = false;
    private float _elapsedTime = 0f;
    private float _interpolator = 0f;

    public bool _isStarted = false;
    public bool _isComplete = false;
    private float _flipbookLimit = 0f;

    private MeshRenderer _meshRenderer;

    private Coroutine _runningCoroutine;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        Vector2 _flipbookDimensions = _meshRenderer.material.GetVector("_FlipbookDimensions");
        _flipbookLimit = Mathf.Floor(_flipbookDimensions.x * _flipbookDimensions.y);
    }

    void Update()
    {
        if (_isStarted)
        {
            _elapsedTime += Time.deltaTime;
        }   
    }

    [Button("PlayFlipbook", EButtonEnableMode.Playmode)]
    public void PlayFlipbook()
    {
        if (_looping)
        {
            _runningCoroutine = StartCoroutine(LoopFrames());
        } else
        {
            _runningCoroutine = StartCoroutine(LerpFrames());
        }

    }

    public void StopFlipbook()
    {
        if (_runningCoroutine != null)
            StopCoroutine(_runningCoroutine);
        
        _elapsedTime = 0f;
        _interpolator = 0f;
        _isStarted = false;
        _isComplete = false;
    }

    private IEnumerator LerpFrames()
    {
        if (_startAtRandomIndex)
        {
            float startIndex = Random.Range(0f, _flipbookLimit);
            _meshRenderer.material.SetFloat("_FrameIndex", startIndex);

            float interpolatedStart = startIndex / (_flipbookLimit - 1.0f);
            _elapsedTime = interpolatedStart * _flipbookDuration;
        }
        else 
            _meshRenderer.material.SetFloat("_FrameIndex", 0);

        _isComplete = false;
        _isStarted = true;


        while (_meshRenderer.material.GetFloat("_FrameIndex") < _flipbookLimit)
        {
            _interpolator = _elapsedTime / _flipbookDuration;
            float flipbookLerpValue = Mathf.Lerp(0f, _flipbookLimit, _interpolator);
            _meshRenderer.material.SetFloat("_FrameIndex", flipbookLerpValue);
            yield return null;
        }
        
        _isStarted = false;
        _isComplete = true;

        _elapsedTime = 0f;
        _meshRenderer.material.SetFloat("_FrameIndex", 0);
    }
    
    private IEnumerator LoopFrames()
    {
        if (_startAtRandomIndex)
        {
            float startIndex = Random.Range(0f, _flipbookLimit);
            _meshRenderer.material.SetFloat("_FrameIndex", startIndex);

            float interpolatedStart = startIndex / (_flipbookLimit - 1.0f);
            _elapsedTime = interpolatedStart * _flipbookDuration;
        }
        else
            _meshRenderer.material.SetFloat("_FrameIndex", 0);

        _isStarted = true;

        while (_looping)
        {
            while (_meshRenderer.material.GetFloat("_FrameIndex") < _flipbookLimit)
            {
                _interpolator = _elapsedTime / _flipbookDuration;
                float flipbookLerpValue = Mathf.Lerp(0f, _flipbookLimit, _interpolator);
                _meshRenderer.material.SetFloat("_FrameIndex", flipbookLerpValue);
                yield return null;
            }
            
            _meshRenderer.material.SetFloat("_FrameIndex", 0);
            _elapsedTime = 0f;
        }
    }

    public void DisableMeshRenderer()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.enabled = false;
    }
    public void EnableMeshRenderer()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.enabled = true;
    }

}