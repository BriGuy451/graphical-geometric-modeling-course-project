using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VolumeMatrixInit : MonoBehaviour {
    private Material _material;

    void Start()
    {
        Matrix4x4 volumeToWorld = transform.localToWorldMatrix;
        Matrix4x4 worldToVolume = volumeToWorld.inverse;

        _material = GetComponent<MeshRenderer>().material;

        _material.SetMatrix("_WorldToVolume", worldToVolume);
    }

}