using System.Collections.Generic;
using UnityEngine;

public class MeshVertex {
    
    public int _index;
    public Vector3 _position;
    public Vector3 _normal;
    public List<MeshEdge> _meshEdges;
    public List<MeshTriangle> _meshTriangles;
    public List<Vector2> _uvChannels;

    public void SetEdges(List<MeshEdge> meshEdges)
    {
        _meshEdges = meshEdges;
    }
    
    public void SetVertex(Vector3 meshVertex)
    {
        _position = meshVertex;
    }
    
    public void SetNormal(Vector3 normal)
    {
        _position = normal;
    }
    
    public void SetIndex(int index)
    {
        _index = index;
    }
    
    public void SetTriangles(List<MeshTriangle> triangles)
    {
        _meshTriangles = triangles;
    }
    
    public void AddEdge(MeshEdge meshEdge)
    {
        _meshEdges.Add(meshEdge);
    }

    public void AddTriangle(MeshTriangle meshTriangle)
    {
        _meshTriangles.Add(meshTriangle);
    }

    public void SetUVChannel(int uvChannel, Vector2 value)
    {
    }

}