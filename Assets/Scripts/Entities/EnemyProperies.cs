using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class EnemyProperties
{
    public Mesh m_mesh;
    public int m_enemySpeed;
    public Material m_material;
    public Vector3 m_spawnPoint;
    public List<Vector3> m_patrolRoute;

    public EnemyProperties(Vector3 spawnPoint, List<Vector3> patrolRoute)
    {
        m_spawnPoint = spawnPoint;
        m_patrolRoute = patrolRoute;
    }
}