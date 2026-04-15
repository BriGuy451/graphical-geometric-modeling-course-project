using NaughtyAttributes;
using UnityEngine;

public class Revolver : MonoBehaviour {
    
    [SerializeField] private float _secondsPerRevolution = 1f;
    [SerializeField] private float _radius = 1f;
    [SerializeField] private Transform _center;
    private Vector3 _centerVec = Vector3.zero;

    private bool _isRevolutionStarted = false;
    void Start()
    {
        if (_center != null)
            _centerVec = _center.position;
    }

    void Update()
    {
        if (_isRevolutionStarted)
            PerformRevolutions();
    }

    [Button("PerformRevolutions", EButtonEnableMode.Playmode)]
    public void SetRevolutionOn()
    {
        _isRevolutionStarted = true;
    }
    [Button("StopRevolutions", EButtonEnableMode.Playmode)]
    public void SetRevolutionOff()
    {
        _isRevolutionStarted = false;
    }

    private void PerformRevolutions()
    {
        float period = _secondsPerRevolution;
        float angularSpeed = 2f * Mathf.PI / period;

        float angle = Time.time * angularSpeed; // time * speed

        float x = Mathf.Cos(angle) * _radius;
        float z = Mathf.Sin(angle) * _radius;

        transform.position = _centerVec + new Vector3(x, 0f, z);
    }

}