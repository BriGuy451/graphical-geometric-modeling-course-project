using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct LostCustomerProperties
{
    public Vector3 spawnPosition;
    public List<Vector3> wanderRoute;
}

[System.Serializable]
public struct EnemyPatrolProperties
{
    public Vector3 spawnPosition;
    public List<Vector3> patrolRoute;
}


[System.Serializable]
public class LabelToAudioClip
{
    public string label;
    public AudioClip audioClip;
}

[System.Serializable]
public class SoundGroup
{
    public string categoryLabel;
    public List<LabelToAudioClip> labelToAudioClips;
}

[System.Serializable]
public class SoundWithOptions
{
    public string soundKey = "NULL";
    public bool isLooping = false;
    public Vector3 location = Vector3.zero;
    [Range(0.0f, 1.0f)] public float volume = 1.0f;
    public bool isFade = false;
    [Range(0.0f,1.0f)] public float spatialBlend;
    [Range(0.0f,500.0f)] public float minDistance;
    [Range(0.0f,500.0f)] public float maxDistance;
    [Range(0, 256)] public int priority = 128;
}