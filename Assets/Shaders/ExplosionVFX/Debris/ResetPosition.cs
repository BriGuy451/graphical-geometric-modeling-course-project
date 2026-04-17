using UnityEngine;

public class ResetPosition : MonoBehaviour {
    
    private Vector3 _initialTransform;
    private Quaternion _initialRotation;
    private Vector3 _initialScale;
    private Rigidbody _rigidbody;

    void Start()
    {
        _initialTransform = transform.position;
        _initialRotation = transform.rotation;
        _initialScale = transform.localScale;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetTransform()
    {
        transform.position = _initialTransform;
        transform.rotation = _initialRotation;
        transform.localScale = _initialScale;
    }

}