using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MeshGraphHalfEdgeTester : MonoBehaviour {
    private Mesh _mesh;
    private MeshGraphHalfEdge _meshGraphHE;

    [SerializeField] private bool _showVertexRay = false;
    [SerializeField] private bool _boundaries = false;

    public int iterations = 500;
    private Coroutine runningCoroutine = null;
    public bool DrawOnStart = true;
    private bool isDrawing = false;
    private bool firstDrawPass = true;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _meshGraphHE = new MeshGraphHalfEdge(_mesh);

        if (DrawOnStart)
            Draw();
    }

    void Update()
    {
        if (isDrawing)
        {
            DrawAllFaces(_meshGraphHE._faces);
            // DrawMeshTriangle(_meshGraphHE._faces[5]);
            // DrawAllMeshTriangleNeighbors(_meshGraphHE._faces[0]);
        }

        // if (_boundaries)
        //     DrawMeshGraphRepresentation(_meshGraphHE, _showVertexRay);
        // else
        //     DrawMeshGraphRepresentation(_meshGraphHE, false, Color.white);

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

    // [Button("PlaySubdivisionAnimation", EButtonEnableMode.Playmode)]
    // public void PlaySubdivisionAnimation()
    // {
    //     if (runningCoroutine == null)
    //         runningCoroutine = StartCoroutine(AnimateSubdivision());
    // }

    [Button("Draw", EButtonEnableMode.Playmode)]
    public void Draw()
    {
        isDrawing = !isDrawing;
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

    private void DrawAllFaces(List<FaceHE> faces)
    {
        int i = 1;
        foreach (FaceHE face in faces)
        {
            if (firstDrawPass)
                LogFromMethod($"Face #{i}");
            DrawMeshTriangle(face);
            i++;
        }
        firstDrawPass = false;
    }

    private void DrawMeshTriangle(FaceHE face)
    {
        EdgeHE start = face._halfEdge;
        EdgeHE halfEdge = start;
        Vector3 firstPosition = halfEdge._vertex._position;        
        do
        {
            if (firstDrawPass)
                _meshGraphHE.LogEdgePair(halfEdge);
            Vector3 secondPosition = halfEdge._next._vertex._position;
            Debug.DrawLine(firstPosition, secondPosition);
            firstPosition = secondPosition;
            halfEdge = halfEdge._next;
        } while (halfEdge._vertex._index != start._vertex._index);
    }

    private void DrawAllMeshTriangleNeighbors(FaceHE face)
    {
        EdgeHE start = face._halfEdge;
        EdgeHE edgeHE = start;
        for (int i = 0; i < 3; i++)
        {
            _meshGraphHE.LogEdgePair(edgeHE);
            if (edgeHE._opposite != null)
            {
                FaceHE neighboorFace = edgeHE._opposite._face;
                DrawMeshTriangle(neighboorFace);
            } else
            {
                LogFromMethod($"Is Null");
            }

            edgeHE = edgeHE._next;
        }
    }

    private void DrawEntireMesh(FaceHE face)
    {
        EdgeHE halfEdge = face._halfEdge;
        Vector3 firstPosition = halfEdge._vertex._position;
        EdgeHE nextEdge = halfEdge._next;


    }

    // private IEnumerator AnimateSubdivision()
    // {
    //     for (int i = 0; i < iterations; i++)
    //     {
    //         _meshGraph.SubdivideEdgeTester(i);
    //         yield return new WaitForSeconds(.02f);
    //     }
    // }

    private void LogVertexCount()
    {
        LogFromMethod($"Vertex Count: {_meshGraphHE._vertices.Count}");
    }

    private void LogTriangleCount()
    {
        LogFromMethod($"Triangle Count: {_meshGraphHE._faces.Count}");
    }

    private void LogEdgeCount()
    {
        LogFromMethod($"Edge Count: {_meshGraphHE._edgesHalf.Count}");
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"MeshGraphTesterMono: {message}");
    }
}