using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MeshGraphTesterMono : MonoBehaviour {
    private Mesh _mesh;
    private MeshGraph _meshGraph;

    [SerializeField] private bool _showVertexRay = false;
    [SerializeField] private bool _boundaries = false;

    public int iterations = 500;
    private Coroutine runningCoroutine = null;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;   
        _meshGraph = new MeshGraph(_mesh);

        // _meshGraph.SubdivideEdgeTester();
        // _meshGraph.LogGraphConfiguration();
        // _meshGraph.LogUnityMeshTriangles();
    }

    int currentIndex = 0;
    void Update()
    {
        DrawMeshTriangle(_meshGraph._triangles);

        if (_boundaries)
            DrawMeshGraphRepresentation(_meshGraph.GetBoundaryEdges(), _showVertexRay);
        else
            DrawMeshGraphRepresentation(_meshGraph._edges, false, Color.white);

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     // LogFromMethod($"Update: Here");
        //     // _meshGraph.SubdivideEdgeTester(iterations);
        //     // currentIndex++;
        //     // LogVertexCount();
        //     if (runningCoroutine == null)
        //         runningCoroutine = StartCoroutine(AnimateSubdivision());
        // }

        // LogTriangleCount();
        // LogEdgeCount();
    }

    [Button("PlaySubdivisionAnimation", EButtonEnableMode.Playmode)]
    public void PlaySubdivisionAnimation()
    {
        if (runningCoroutine == null)
            runningCoroutine = StartCoroutine(AnimateSubdivision());
    }

    private void DrawMeshGraphRepresentation(List<MeshEdge> edges, bool showVertexRay = false, Color? color = null)
    {
        if (color == null)
            color = Color.red;

        List<MeshEdge> meshEdges = edges;
        foreach (MeshEdge meshEdge in meshEdges)
        {
            Vector3 vertexA = meshEdge._vertexA._position;
            Vector3 vertexB = meshEdge._vertexB._position;
            
            if (_showVertexRay)
            {
                Debug.DrawRay(vertexA, meshEdge._vertexA._normal, Color.black);
                Debug.DrawRay(vertexB, meshEdge._vertexB._normal, Color.black);
            }

            Debug.DrawLine(vertexA, vertexB, (Color)color);
        }
    }

    private void DrawMeshTriangle(List<MeshTriangle> meshTriangles)
    {
        int trianglePrintedCount = 0;
        foreach (MeshTriangle meshTriangle in meshTriangles)
        {
            foreach (MeshEdge meshEdge in meshTriangle._edgeReferences)
            {
                Debug.DrawLine(meshEdge._vertexA._position, meshEdge._vertexB._position, Color.blue);
            }

            trianglePrintedCount++;

            // if (trianglePrintedCount > 10)
            //     break;
        }
    }

    private IEnumerator AnimateSubdivision()
    {
        for (int i = 0; i < iterations; i++)
        {
            _meshGraph.SubdivideEdgeTester(i);
            yield return new WaitForSeconds(.02f);
        }
    }

    private void LogVertexCount()
    {
        LogFromMethod($"Vertex Count: {_meshGraph._vertices.Count}");
    }

    private void LogTriangleCount()
    {
        LogFromMethod($"Triangle Count: {_meshGraph._triangles.Count}");
    }
    
    private void LogEdgeCount()
    {
        LogFromMethod($"Edge Count: {_meshGraph._edges.Count}");
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"MeshGraphTesterMono: {message}");
    }
}