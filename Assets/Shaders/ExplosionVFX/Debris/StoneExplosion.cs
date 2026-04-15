using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class StoneExplosion : MonoBehaviour {
    
    [SerializeField][Range(0, 100)] private float scalingFactor = 10f;
    List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    private List<ResetPosition> _resetPositions = new List<ResetPosition>();

    [SerializeField] private bool _isInverted = false;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);

            Rigidbody[] rigidbodies = childTransform.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in rigidbodies)
                _rigidbodies.Add(rigidbody);

            ResetPosition[] resetPositions = childTransform.GetComponentsInChildren<ResetPosition>();
            foreach (ResetPosition resetPosition in resetPositions)
                _resetPositions.Add(resetPosition);
        }
    }

    [Button("ApplyDirectionalForce", EButtonEnableMode.Playmode)]
    public void ApplyDirectionalForce()
    {
        foreach (Rigidbody rigidbody in _rigidbodies)
        {
            Transform objectTransform = rigidbody.transform;
            Vector3 forwardDirectionVec = objectTransform.forward;

            Vector3 inBetweenVect;
            if (_isInverted)
                inBetweenVect = Vector3.Normalize(-objectTransform.up + forwardDirectionVec);
            else
                inBetweenVect = Vector3.Normalize(objectTransform.up + forwardDirectionVec);

            // rigidbody.mass = 1f;
            rigidbody.mass = 0.035f;
            rigidbody.useGravity = true;
            rigidbody.AddForce((inBetweenVect + GenerateRandomVector()) * scalingFactor);
        }    
    }

    [Button("ResetPosition", EButtonEnableMode.Playmode)]
    public void ResetAllPositions()
    {
        RigidbodyReset();
        foreach (ResetPosition resetPosition in _resetPositions)
        {
            resetPosition.ResetTransform();
        }
    }

    private void RigidbodyReset()
    {
        foreach (Rigidbody rigidbody in _rigidbodies)
        {
            // rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private Vector3 GenerateRandomVector()
    {
        float x, y, z;

        x = Random.Range(-1, 1);
        y = Random.Range(-1, 1);
        z = Random.Range(-1, 1);

        return new Vector3(x, y, z);
    }

}