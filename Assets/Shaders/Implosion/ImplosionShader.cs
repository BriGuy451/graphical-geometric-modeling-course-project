using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ImplosionShader : MonoBehaviour {
    
    [SerializeField] private Mesh _implosionShape;
    private Mesh _mesh;
    private Material _implosionMaterial;

    private Vector3 _convergencePoint = Vector3.zero;
    [SerializeField][Range(0f, 1f)] private float _meshPercentage = .5f;

    private Vector3[] _sourceToTargetFlatMap;
    private Vector3[] _endMap;
    
    private Vector3[] _scaledLocalMesh;
    private Vector3[] _translatedMesh;
    private Vector3[] _meshWorld;

    private Vector3[] _bolsteredMesh;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _implosionMaterial = GetComponent<MeshRenderer>().material;

        if (_mesh.vertexCount < _implosionShape.vertexCount)
        {
            // _bolsteredMesh = BolsterVertices(_mesh.vertices);
            // _mesh.SetVertices(_bolsteredMesh);
            // BolsterVerticesWithTriangles(_mesh);
        }

        Bounds bounds = _mesh.bounds;

        _sourceToTargetFlatMap = new Vector3[_mesh.vertices.Length * 2];
        
        Vector3 worldCenter = transform.TransformPoint(bounds.center);
        // LogFromMethod($"World Center: {worldCenter}");

        Vector3[] meshWorldVertices = _mesh.vertices;
        transform.TransformPoints(meshWorldVertices);

        _meshWorld = meshWorldVertices;

        Vector3[] scaledLocalImplosionMesh = ScaleMeshLocal(.3f);
        transform.TransformPoints(scaledLocalImplosionMesh);

        _scaledLocalMesh = scaledLocalImplosionMesh;

        Vector3[] translatedImplosionMesh = MoveMeshToPosition(scaledLocalImplosionMesh, worldCenter);
        // Vector3[] translatedImplosionMesh = MoveMeshToPosition(scaledLocalImplosionMesh, bounds.center);
        _translatedMesh = translatedImplosionMesh;
        
        int i = 0;
        foreach (Vector3 sourceVertex in meshWorldVertices)
        {
            _sourceToTargetFlatMap[i] = sourceVertex;
            // LogFromMethod($"SourceVertex: {sourceVertex}");
            i++;
            
            Vector3 nearestPoint = Vector3.positiveInfinity;
            foreach (Vector3 targetVertex in scaledLocalImplosionMesh)
            {
                if (Vector3.Distance(sourceVertex, targetVertex) < Vector3.Distance(sourceVertex, nearestPoint))
                    nearestPoint = targetVertex;
            }
            
            // LogFromMethod($"MappedVertex: {nearestPoint}");
            _sourceToTargetFlatMap[i] = nearestPoint;
            i++;
        }

        ComputeBuffer computeBuffer = new ComputeBuffer(_sourceToTargetFlatMap.Length, sizeof(float) * 3);
        computeBuffer.SetData(_sourceToTargetFlatMap);

        LogFromMethod($"map length: {_sourceToTargetFlatMap.Length}");
        _endMap = _sourceToTargetFlatMap;

        _implosionMaterial.SetInt("_VertCount", _sourceToTargetFlatMap.Length);
        _implosionMaterial.SetBuffer("_Data", computeBuffer);

        _convergencePoint = bounds.center;

        _implosionMaterial.SetVector("_ConvergencePoint", _convergencePoint);
        _implosionMaterial.SetFloat("_MeshPercentage", _meshPercentage);
        _implosionMaterial.SetFloat("_AnimationThreshold", 0f);
    }

    void Update()
    {
        // Cubes
        // if (_mesh != null)
        //     DebugDrawMeshWireframe(_mesh.vertices, Color.black);
        // if (_meshWorld != null)
        //     DebugDrawMeshWireframeSingles(_meshWorld);

        // // Spheres
        // if (_scaledLocalMesh != null)
        //     DebugDrawMeshWireframe(_scaledLocalMesh, Color.red);      
        // if (_translatedMesh != null)
        //     DebugDrawMeshWireframe(_translatedMesh, Color.yellow);
        
        // if (_endMap != null)
        //     DebugDrawMeshWireframeSingles(_endMap, Color.green);

        // if (_bolsteredMesh != null)
        //     DebugDrawMeshWireframe(_bolsteredMesh, Color.cyan);
            
    }

  // ImplosionShape must be within center of mesh. [move vertices in relation to mesh center]
    private Vector3[] MoveMeshToPosition(Vector3[] meshVerticesToTranslate, Vector3 worldMeshBoundsCenter)
    {
        Vector3[] repositionedMesh = new Vector3[meshVerticesToTranslate.Length];
        
        for (int i = 0; i < repositionedMesh.Length; i++)
        {
            repositionedMesh[i] = worldMeshBoundsCenter - meshVerticesToTranslate[i];
        }

        return repositionedMesh;
    }

    // ImplosionShape must be scaled down[move vertices in relation to local space]
    private Vector3[] ScaleMeshLocal(float scalingFactor = .2f)
    {
        Vector3[] meshVerticesLocalSpace = _implosionShape.vertices;

        Vector3[] scaledVerticesLocalSpace = new Vector3[meshVerticesLocalSpace.Length];
        
        for (int i = 0; i < meshVerticesLocalSpace.Length; i++)
        {
            scaledVerticesLocalSpace[i] = meshVerticesLocalSpace[i] * scalingFactor;
        }

        return scaledVerticesLocalSpace;
    }

    private Vector3[] BolsterVertices(Vector3[] shallowMeshVectors)
    {
        Vector3[] deepMeshVectors = new Vector3[shallowMeshVectors.Length * 2];
        
        for (int i = 0, j = 0; i < shallowMeshVectors.Length - 1; i += 2, j += 3)
        {
            Vector3 startPoint = shallowMeshVectors[i];
            Vector3 endPoint = shallowMeshVectors[i+1];
            
            Vector3 displacementVector = endPoint - startPoint;
            Vector3 newVertex = displacementVector / 2f;

            deepMeshVectors[j] = startPoint;
            deepMeshVectors[j+1] = newVertex;
            deepMeshVectors[j+2] = endPoint;
        }

        return deepMeshVectors;
    }

    private Vector3[] BolsterVerticesWithTriangles(Mesh mesh)
    {
        Vector3[] verticesByIndex = mesh.vertices;
        
        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            LogFromMethod($"vertices: {verticesByIndex.Length}");
            LogFromMethod($"triangles: {triangles.Length}");

            for (int j = 0, k = 0; j < triangles.Length; j += 3, k+= 6)
            {

                // if (j > 6)
                //     break;

                LogFromMethod($"triangles: {triangles[j]}, {triangles[j+1]}, {triangles[j+2]}");

                Vector3 vertexA = verticesByIndex[triangles[j]];
                Vector3 vertexB = verticesByIndex[triangles[j+1]];
                Vector3 vertexC = verticesByIndex[triangles[j+2]];

                Vector3 newVertexOne = (vertexB - vertexA) / 2f;
                Vector3 newVertexTwo = (vertexC - vertexB) / 2f;
                Vector3 newVertexThree = (vertexA - vertexC) / 2f;
                
                newVertices.Add(vertexA);
                newVertices.Add(newVertexOne);
                newVertices.Add(vertexB);
                newVertices.Add(newVertexTwo);
                newVertices.Add(vertexC);
                newVertices.Add(newVertexThree);

                newTriangles.Add(k);
                newTriangles.Add(k+1);
                newTriangles.Add(k+5);
                
                newTriangles.Add(k+1);
                newTriangles.Add(k+3);
                newTriangles.Add(k+2);

                newTriangles.Add(k+4);
                newTriangles.Add(k+3);
                newTriangles.Add(k+5);
            }
        }

        LogFromMethod($"newVertices: {newVertices.Count}");
        LogFromMethod($"newTriangles: {newTriangles.Count}");

        mesh.SetVertices(newVertices.ToArray());
        mesh.SetTriangles(newTriangles.ToArray(), 0);

        return newVertices.ToArray();
    }

    private void DebugDrawMeshWireframe(Vector3[] points, Color? color = null)
    {
        if (color == null)
            color = Color.white;

        Vector3 previousPoint = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 nextPoint = points[i];
            Debug.DrawLine(previousPoint, nextPoint, (Color)color);
            previousPoint = nextPoint;
        }
    }
    
    private void DebugDrawMeshWireframeSingles(Vector3[] points, Color? color = null)
    {
        if (color == null)
            color = Color.white;

        Vector3 point = points[0];
        for (int i = 1; i < points.Length - 1; i += 2)
        {
            Vector3 nextPoint = points[i];
            Debug.DrawLine(point, nextPoint, (Color)color);
            point = points[i+1];
        }
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"ImplosionShader: {message}");
    }

}