using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGraph
{
    
    private int[] _unityMeshTriangles;

    public List<MeshVertex> _vertices;
    public List<MeshEdge> _edges;
    public List<MeshTriangle> _triangles;


    public MeshGraph()
    {
        _vertices = new List<MeshVertex>();
        _edges = new List<MeshEdge>();
        _triangles = new List<MeshTriangle>();
    }
    
    public MeshGraph(Mesh mesh)
    {
        _vertices = new List<MeshVertex>();
        _edges = new List<MeshEdge>();
        _triangles = new List<MeshTriangle>();
        
        InitializeGraph(mesh, out _vertices, out _edges, out _triangles);

        // completeUVs.Add(mesh.uv);
        // completeUVs.Add(mesh.uv2);
        // completeUVs.Add(mesh.uv3);
        // completeUVs.Add(mesh.uv4);
        // completeUVs.Add(mesh.uv5);
        // completeUVs.Add(mesh.uv6);
        // completeUVs.Add(mesh.uv7);
        // completeUVs.Add(mesh.uv8);
        

    }
    
    public void InitializeGraph(Mesh mesh, out List<MeshVertex> meshVertices, out List<MeshEdge> meshEdges, out List<MeshTriangle> meshTriangles)
    {
        Vector3[] unityMeshVertices = mesh.vertices;
        int[] unityMeshTriangles = mesh.triangles;
        _unityMeshTriangles = mesh.triangles;

        List<MeshVertex> tempMeshVertices = new List<MeshVertex>();
        List<MeshEdge> tempMeshEdges = new List<MeshEdge>();
        List<MeshTriangle> tempMeshTriangles = new List<MeshTriangle>();

        Dictionary<int, MeshVertex> indicesToVertexDict = new Dictionary<int, MeshVertex>();
        for (int i = 0; i < unityMeshTriangles.Length - 1; i += 3)
        {
            int vertexAIndex = unityMeshTriangles[i];
            int vertexBIndex = unityMeshTriangles[i+1];
            int vertexCIndex = unityMeshTriangles[i+2];

            MeshVertex meshVertexA;
            if (indicesToVertexDict.ContainsKey(vertexAIndex))
            {
                LogFromMethod($"Existing Vertex A: Index: {vertexAIndex}");
                meshVertexA = indicesToVertexDict[vertexAIndex];
            } else
            {
                LogFromMethod($"New Vertex A: Index: {vertexAIndex}");
                Vector3 vertexA = unityMeshVertices[vertexAIndex];
                meshVertexA = new MeshVertex()
                {
                    _index = vertexAIndex,
                    _position = vertexA,
                    _normal = vertexA.normalized,
                    _meshEdges = new List<MeshEdge>(),
                    _meshTriangles = new List<MeshTriangle>()
                };

                indicesToVertexDict.Add(vertexAIndex, meshVertexA);
            }
            
            MeshVertex meshVertexB;
            if (indicesToVertexDict.ContainsKey(vertexBIndex))
            {
                LogFromMethod($"Existing Vertex B: Index: {vertexBIndex}");
                meshVertexB = indicesToVertexDict[vertexBIndex];
            } else
            {
                LogFromMethod($"New Vertex B: Index: {vertexBIndex}");
                Vector3 vertexB = unityMeshVertices[vertexBIndex];
                meshVertexB = new MeshVertex()
                {
                    _index = vertexBIndex,
                    _position = vertexB,
                    _normal = vertexB.normalized,
                    _meshEdges = new List<MeshEdge>(),
                    _meshTriangles = new List<MeshTriangle>()
                };

                indicesToVertexDict.Add(vertexBIndex, meshVertexB);
            }

            MeshVertex meshVertexC;
            if (indicesToVertexDict.ContainsKey(vertexCIndex))
            {
                // LogFromMethod($"Existing Vertex C: Index: {vertexCIndex}");
                meshVertexC = indicesToVertexDict[vertexCIndex];
            } else
            {
                // LogFromMethod($"New Vertex C: Index: {vertexCIndex}");
                Vector3 vertexC = unityMeshVertices[vertexCIndex];
                meshVertexC = new MeshVertex()
                {
                    _index = vertexCIndex,
                    _position = vertexC,
                    _normal = vertexC.normalized,
                    _meshEdges = new List<MeshEdge>(),
                    _meshTriangles = new List<MeshTriangle>()
                };

                indicesToVertexDict.Add(vertexCIndex, meshVertexC);
            }

            MeshEdge meshEdgeAB = new MeshEdge()
            {
                _vertexA = meshVertexA,
                _vertexB = meshVertexB
            };
            MeshEdge meshEdgeAC = new MeshEdge()
            {
                _vertexA = meshVertexA,
                _vertexB = meshVertexC
            };

            MeshEdge meshEdgeBC = new MeshEdge()
            {
                _vertexA = meshVertexB,
                _vertexB = meshVertexC
            };
            MeshEdge meshEdgeBA = new MeshEdge()
            {
                _vertexA = meshVertexB,
                _vertexB = meshVertexA
            };

            MeshEdge meshEdgeCA = new MeshEdge()
            {
                _vertexA = meshVertexC,
                _vertexB = meshVertexA
            };
            MeshEdge meshEdgeCB = new MeshEdge()
            {
                _vertexA = meshVertexC,
                _vertexB = meshVertexB
            };

            // gets a new triangle
            // new edge doesn't matter
            // but I need the existing edge
            MeshEdge[] edgeReferences = new MeshEdge[6]; 
            MeshEdge existingEdgeAB = GetExistingEdge(meshVertexA, meshVertexB);
            if (existingEdgeAB != null)
                edgeReferences[0] = existingEdgeAB;
            else 
                edgeReferences[0] = meshEdgeAB;
            
            MeshEdge existingEdgeAC = GetExistingEdge(meshVertexA, meshVertexC);
            if (existingEdgeAC != null)
                edgeReferences[1] = existingEdgeAC;
            else 
                edgeReferences[1] = meshEdgeAC;
            

            MeshEdge existingEdgeBC = GetExistingEdge(meshVertexB, meshVertexC);
            if (existingEdgeBC != null)
                edgeReferences[2] = existingEdgeBC;
            else 
                edgeReferences[2] = meshEdgeBC;
            
            MeshEdge existingEdgeBA = GetExistingEdge(meshVertexB, meshVertexA);
            if (existingEdgeBA != null)
                edgeReferences[3] = existingEdgeBA;
            else 
                edgeReferences[3] = meshEdgeBA;


            MeshEdge existingEdgeCA = GetExistingEdge(meshVertexC, meshVertexA);
            if (existingEdgeCA != null)
                edgeReferences[4] = existingEdgeCA;
            else 
                edgeReferences[4] = meshEdgeCA;
            
            MeshEdge existingEdgeCB = GetExistingEdge(meshVertexC, meshVertexB);
            if (existingEdgeCB != null)
                edgeReferences[5] = existingEdgeCB;
            else 
                edgeReferences[5] = meshEdgeCB;


            MeshTriangle meshTriangle = new MeshTriangle()
            {
                _vertIndices = new int[3]{vertexAIndex, vertexBIndex, vertexCIndex},
                _edgeReferences = edgeReferences
            };

            if (existingEdgeAB != null)
            {
                // add triangle
                // don't add vertex A or B vertex
                // LogFromMethod($"ExistingEdgeAB");
                existingEdgeAB._triangle2 = meshTriangle;
                existingEdgeAB._vertexA.AddTriangle(meshTriangle);
                existingEdgeAB._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexA.AddEdge(meshEdgeAB);
                meshVertexA.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexA);

                meshEdgeAB._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeAB._vertexA._index} -> {meshEdgeAB._vertexB._index}");
                tempMeshEdges.Add(meshEdgeAB);
            }
            
            if (existingEdgeAC != null)
            {
                // add triangle
                // don't add vertex A or B vertex
                // LogFromMethod($"ExistingEdgeAC");
                existingEdgeAC._triangle2 = meshTriangle;
                existingEdgeAC._vertexA.AddTriangle(meshTriangle);
                existingEdgeAC._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexA.AddEdge(meshEdgeAC);
                meshVertexA.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexA);

                meshEdgeAC._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeAC._vertexA._index} -> {meshEdgeAC._vertexB._index}");
                tempMeshEdges.Add(meshEdgeAC);
            }


            if (existingEdgeBC != null)
            {
                // LogFromMethod($"ExistingEdgeBC");
                existingEdgeBC._triangle2 = meshTriangle;
                existingEdgeBC._vertexA.AddTriangle(meshTriangle);
                existingEdgeBC._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexB.AddEdge(meshEdgeBC);
                meshVertexB.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexB);

                meshEdgeBC._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeBC._vertexA._index} -> {meshEdgeBC._vertexB._index}");
                tempMeshEdges.Add(meshEdgeBC);
            }
            
            if (existingEdgeBA != null)
            {
                // LogFromMethod($"ExistingEdgeBA");
                existingEdgeBA._triangle2 = meshTriangle;
                existingEdgeBA._vertexA.AddTriangle(meshTriangle);
                existingEdgeBA._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexB.AddEdge(meshEdgeBA);
                meshVertexB.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexB);

                meshEdgeBA._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeBA._vertexA._index} -> {meshEdgeBA._vertexB._index}");
                tempMeshEdges.Add(meshEdgeBA);
            }


            if (existingEdgeCA != null)
            {
                // LogFromMethod($"ExistingEdgeCA");
                existingEdgeCA._triangle2 = meshTriangle;
                existingEdgeCA._vertexA.AddTriangle(meshTriangle);
                existingEdgeCA._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexC.AddEdge(meshEdgeCA);
                meshVertexC.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexC);

                meshEdgeCA._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeCA._vertexA._index} -> {meshEdgeCA._vertexB._index}");
                tempMeshEdges.Add(meshEdgeCA);
            }
            
            if (existingEdgeCB != null)
            {
                // LogFromMethod($"ExistingEdgeCB");
                existingEdgeCB._triangle2 = meshTriangle;
                existingEdgeCB._vertexA.AddTriangle(meshTriangle);
                existingEdgeCB._vertexB.AddTriangle(meshTriangle);
            } else
            {
                meshVertexC.AddEdge(meshEdgeCB);
                meshVertexC.AddTriangle(meshTriangle);
                tempMeshVertices.Add(meshVertexC);

                meshEdgeCB._triangle1 = meshTriangle;
                LogFromMethod($"Added Edge: {meshEdgeCB._vertexA._index} -> {meshEdgeCB._vertexB._index}");
                tempMeshEdges.Add(meshEdgeCB);
            }

            tempMeshTriangles.Add(meshTriangle);
        }

        meshVertices = tempMeshVertices;
        meshEdges = tempMeshEdges;
        meshTriangles = tempMeshTriangles;
    }

    public List<MeshEdge> GetBoundaryEdges()
    {
        List<MeshEdge> boundaryEdges = new List<MeshEdge>();

        foreach (MeshEdge meshEdge in _edges)
        {
            if (meshEdge._triangle1 != null && meshEdge._triangle2 == null)
                boundaryEdges.Add(meshEdge);
        }

        return boundaryEdges;
    }

    public List<MeshEdge> GetRegularEdges()
    {
        List<MeshEdge> boundaryEdges = new List<MeshEdge>();

        foreach (MeshEdge meshEdge in _edges)
        {
            if (meshEdge._triangle1 != null && meshEdge._triangle2 != null)
                boundaryEdges.Add(meshEdge);
        }

        return boundaryEdges;
    }

    public void SubdivideEdgeTester(int index = 0)
    {
        // for (int i = 0; i < 1; i++)
        // {
        //     MeshEdge edgeToDivide = _edges[i];
        //     SubdivideEdge(edgeToDivide);
        // }
        LogFromMethod($"SubdivideEdgeTester");
        // LogFromMethod($"Vertex Count At Beginning: {_vertices.Count}");
        // for (int i = 0; i < iterations; i++)
        // {
        //     LogFromMethod($"Split #{i+1}");
        //     MeshEdge edgeToDivide = _edges[i];
        //     SubdivideEdge(edgeToDivide);
        //     // LogFromMethod($"Vertex Count Continuous: {_vertices.Count}");
        // }

        // LogAllEdges(true);
        LogFromMethod($"Index: {index}");
        MeshEdge edgeToDivide = _edges[index];
        SubdivideEdge(edgeToDivide);
        // LogAllEdges(true);
    }

    /// Subdividing an edge, requires an edge loop to be made.
    /// The new vertex must create a perpendicular edge connecting to adjacent triangles
    /// EdgeAB -> Triangle1 and Triangle2 -> The new edge connects to which ever vertex is not in the current EdgeAB, so Vertex C
    /// Triangle1 GetVertexNotAB -> Create Edge from new Vertex to NotABVertex
    /// Triangle2 GetVertexNotAB -> Create Edge from new Vertex to NotABVertex
    /// After splitting of EdgeAB it must be replaced with two new Edges Edge_Aa and Edge_aB
    /// 
    /// Replacing a single existing edge in my list of edges (just update it and add one?)
    /// Replacing a existing triangle after it gets changed with new perpendicular edge (update existing and add one?)
    /// Add new Vertex from Separation
    

    /// Mesh Edge and Opposite MeshEdge after subdivision hold old data for triangles
    private void SubdivideEdge(MeshEdge meshEdge)
    {
        MeshVertex vertexA = meshEdge._vertexA;
        MeshVertex vertexB = meshEdge._vertexB;
        MeshTriangle originalTriangle1 = meshEdge._triangle1;
        MeshTriangle originalTriangle2 = meshEdge._triangle2;

        MeshEdge inverseEdge = GetInverseEdge(originalTriangle1, meshEdge);

        LogFromMethod($"--SubdivideEdge Start--");

        // LogEdge(meshEdge, "MeshEdgeStart");
        // LogTriangle(meshEdge._triangle1, "MeshEdgeTriangleOne");

        // if (meshEdge._triangle2 != null)
            // LogTriangle(meshEdge._triangle2, "MeshEdge Triangle Two");

        // LogTriangle(originalTriangle1, "OriginalTriangleOne");

        // LogEdge(inverseEdge, "InverseEdgeStart");
        // LogTriangle(inverseEdge._triangle1, "InverseEdgeTriangleOneStart");
        // LogFromMethod($"MeshEdgeStart: VertexA[{vertexA._index}] to VertexB[{vertexB._index}]");
        
        // if (inverseEdge == null)
        //     LogFromMethod("ITS NULL");
        // else 
        //     LogFromMethod($"InverseEdgeStart: VertexA[{inverseEdge._vertexA._index}] to VertexB[{inverseEdge._vertexB._index}]");
        
        // LogFromMethod($"Original Triangle One: [{originalTriangle1._vertIndices[0]}, {originalTriangle1._vertIndices[1]}, {originalTriangle1._vertIndices[2]}]");

        MeshVertex vertexBisector = new MeshVertex()
        {
          _index = _vertices.Count + 1,
          _position = (vertexB._position + vertexA._position) * .5f,
          _normal = Vector3.Normalize(vertexB._position - vertexA._position),
        };

        // LogFromMethod($"VertexBisector Index: {vertexBisector._index}");


        MeshEdge meshEdgeAToBisect = new MeshEdge()
        {
            _vertexA = vertexA,
            _vertexB = vertexBisector
        };
        MeshEdge meshEdgeBisectToA = new MeshEdge()
        {
            _vertexA = vertexBisector,
            _vertexB = vertexA
        };
        
        MeshEdge meshEdgeBisectToB = new MeshEdge()
        {
            _vertexA = vertexBisector,
            _vertexB = vertexB
        };
        MeshEdge meshEdgeBToBisect = new MeshEdge()
        {
            _vertexA = vertexB,
            _vertexB = vertexBisector
        };

        if (originalTriangle1 != null)
        {
            LogFromMethod("In Triangle1");
            MeshVertex vertexC = GetThirdVertex(originalTriangle1, vertexA, vertexB);

            MeshEdge meshEdgeBisectToTriangle1C = new MeshEdge()
            {
               _vertexA = vertexBisector,
               _vertexB = vertexC,
            };
            MeshEdge meshEdgeTriangle1CToBisect = new MeshEdge()
            {
               _vertexA = vertexC,
               _vertexB = vertexBisector,
            };

            // An edge(s) between B and C (C and B) already exist and is unchanged
            List<MeshEdge> edgesBCandCB = GetMeshEdgesBetweenVerticesPure(vertexB, vertexC);
            MeshEdge meshEdgeBC = edgesBCandCB[0];
            MeshEdge meshEdgeCB = edgesBCandCB[1];

            // An edge(s) between A and C (C and A) already exist and is unchanged
            List<MeshEdge> edgesACandCA = GetMeshEdgesBetweenVerticesPure(vertexA, vertexC);
            MeshEdge meshEdgeAC = edgesACandCA[0];
            MeshEdge meshEdgeCA = edgesACandCA[1];

            /// Both new triangles here
            /// Triangle One: VertexA, VertexBisect, VertexC
            /// Triangle Two: VertexBisect, VertexB, VertexC
            MeshTriangle triangle1 = new MeshTriangle()
            {
              _vertIndices = new int[3]{vertexA._index, vertexBisector._index, vertexC._index},
              _edgeReferences = new MeshEdge[6]{meshEdgeAToBisect, meshEdgeAC, meshEdgeBisectToTriangle1C, meshEdgeBisectToA, meshEdgeCA, meshEdgeTriangle1CToBisect}
            };

            MeshTriangle triangle2 = new MeshTriangle()
            {
                _vertIndices = new int[3]{vertexBisector._index, vertexB._index, vertexC._index},
                _edgeReferences = new MeshEdge[6]{meshEdgeBisectToB, meshEdgeBisectToTriangle1C, meshEdgeBC, meshEdgeBToBisect, meshEdgeTriangle1CToBisect, meshEdgeCB}
            };

            /// EdgeBisectToA, EdgeAToBisect           
            meshEdgeBisectToA._triangle1 = triangle1;
            meshEdgeBisectToA._triangle2 = originalTriangle2;

            meshEdgeAToBisect._triangle1 = triangle1;
            meshEdgeAToBisect._triangle2 = originalTriangle2; 

            /// EdgeBisectToB, EdgeBToBisect
            meshEdgeBisectToB._triangle1 = triangle2;
            meshEdgeBisectToB._triangle2 = originalTriangle2;

            meshEdgeBToBisect._triangle1 = triangle2;
            meshEdgeBToBisect._triangle2 = originalTriangle2;

            /// EdgeBisectToC, EdgeCToBisect
            meshEdgeBisectToTriangle1C._triangle1 = triangle1;
            meshEdgeBisectToTriangle1C._triangle2 = triangle2;

            meshEdgeTriangle1CToBisect._triangle1 = triangle1;
            meshEdgeTriangle1CToBisect._triangle2 = triangle2;

            /// EdgeAToC, EdgeCToA            
            // LogEdge(meshEdgeAC, "MeshEdgeAC IsMatching Call");
            // LogTriangle(originalTriangle1, "Original Triangle One");
            // LogTriangle(meshEdgeAC._triangle1, "MeshEdgeAC Triangle1");
            if (IsMatchingTriangles(meshEdgeAC._triangle1, originalTriangle1, "AC"))
                meshEdgeAC._triangle1 = triangle1;
            else
                meshEdgeAC._triangle2 = triangle1;
            // LogTriangle(meshEdgeAC._triangle1, "MeshEdgeAC After Triangle1");

            // LogEdge(meshEdgeCA, "MeshEdgeCA IsMatching Call");
            // LogTriangle(originalTriangle1, "Original Triangle One");
            // LogTriangle(meshEdgeCA._triangle1, "MeshEdgeCA Triangle1");
            if (IsMatchingTriangles(meshEdgeCA._triangle1, originalTriangle1, "CA"))
                meshEdgeCA._triangle1 = triangle1;
            else
                meshEdgeCA._triangle2 = triangle1;
            // LogTriangle(meshEdgeCA._triangle1, "MeshEdgeCA After Triangle1");

            /// EdgeBToC, EdgeCtoB
            // LogEdge(meshEdgeBC, "MeshEdgeBC IsMatching Call");
            // LogTriangle(originalTriangle1, "Original Triangle One");
            // LogTriangle(meshEdgeBC._triangle1, "MeshEdgeBC Triangle1");
            if (IsMatchingTriangles(meshEdgeBC._triangle1, originalTriangle1, "BC"))
                meshEdgeBC._triangle1 = triangle2;
            else
                meshEdgeBC._triangle2 = triangle1;
            // LogTriangle(meshEdgeBC._triangle1, "MeshEdgeBC After Triangle1");

            // LogEdge(meshEdgeCB, "MeshEdgeCB IsMatching Call");
            // LogTriangle(originalTriangle1, "Original Triangle One");
            // LogTriangle(meshEdgeCB._triangle1, "MeshEdgeCB Triangle1");
            if (IsMatchingTriangles(meshEdgeCB._triangle1, originalTriangle1, "CB"))
                meshEdgeCB._triangle1 = triangle2;
            else
                meshEdgeCB._triangle2 = triangle1;
            // LogTriangle(meshEdgeCB._triangle1, "MeshEdgeCB After Triangle1");

            

            // triangles
            // originalTriangle1 = triangle1;
            // LogFromMethod($"Triangle1: [{triangle1._vertIndices[0]}, {triangle1._vertIndices[1]}, {triangle1._vertIndices[2]}]");
            // LogFromMethod($"Triangle2: [{triangle2._vertIndices[0]}, {triangle2._vertIndices[1]}, {triangle2._vertIndices[2]}]");
            // LogFromMethod($"InverseEdgeTriangle: [{inverseEdge._triangle1._vertIndices[0]}, {inverseEdge._triangle1._vertIndices[1]}, {inverseEdge._triangle1._vertIndices[2]}]");
            // LogFromMethod($"ActualInverseEdgeTriangle: [{_edges[3]._triangle1._vertIndices[0]}, {_edges[3]._triangle1._vertIndices[1]}, {_edges[3]._triangle1._vertIndices[2]}]");
            meshEdge._triangle1 = triangle1;
            inverseEdge._triangle1 = triangle2;
            _triangles.Add(triangle2);
            
            // edges
            // meshEdge = meshEdgeAToBisect;
            // inverseEdge = meshEdgeBToBisect;

            _edges.Add(meshEdgeBisectToA);
            _edges.Add(meshEdgeBisectToB);
            
            _edges.Add(meshEdgeBisectToTriangle1C);
            _edges.Add(meshEdgeTriangle1CToBisect);
            
            // _edges.Add(meshEdgeBToBisect);

            // vertices
            _vertices.Add(vertexBisector);

            // LogTriangle(meshEdgeAC._triangle1, "MeshEdgeAC After Triangle1 End");
            // LogTriangle(meshEdgeCA._triangle1, "MeshEdgeCA After Triangle1 End");
        }

        if (originalTriangle2 != null)
        {
            LogFromMethod("In Triangle2");
            /// New edge with bisector to vertex C
            MeshVertex vertexC = GetThirdVertex(originalTriangle2, vertexA, vertexB);
            MeshEdge meshEdgeBisectToC = new MeshEdge()
            {
                _vertexA = vertexBisector,
                _vertexB = vertexC
            };
            MeshEdge meshEdgeCtoBisect = new MeshEdge()
            {
                _vertexA = vertexC,
                _vertexB = vertexBisector
            };

            /// Need edges AtoC|CtoA and BtoC|CtoB with new triangle
            List<MeshEdge> edgesACandCA = GetMeshEdgesBetweenVerticesPure(vertexA, vertexC);
            MeshEdge meshEdgeAC = edgesACandCA[0];
            MeshEdge meshEdgeCA = edgesACandCA[1];
            
            List<MeshEdge> edgesBCandCB = GetMeshEdgesBetweenVerticesPure(vertexB, vertexC);
            MeshEdge meshEdgeBC = edgesBCandCB[0];
            MeshEdge meshEdgeCB = edgesBCandCB[1];
            
            /// Create the new triangles
            /// Triangle1 {VertexA, VertexBisect, VertexC}
            /// Triangle2 {VertexBisect, VertexB, VertexC}
            MeshTriangle triangle1 = new MeshTriangle()
            {
                _vertIndices = new int[]{vertexA._index, vertexBisector._index, vertexC._index },
                _edgeReferences = new MeshEdge[6]{meshEdgeAToBisect, meshEdgeAC, meshEdgeBisectToC, meshEdgeBisectToA, meshEdgeCA, meshEdgeCtoBisect}
            };
            MeshTriangle triangle2 = new MeshTriangle()
            {
                _vertIndices = new int[]{vertexBisector._index, vertexB._index, vertexC._index },
                _edgeReferences = new MeshEdge[6]{meshEdgeBisectToB, meshEdgeBisectToC, meshEdgeBC, meshEdgeBToBisect, meshEdgeCtoBisect, meshEdgeCB}
            };

            /// Need edges AtoBisect|BisectToA and BisectToB|BtoBisect to update their triangle2 values after splitting
            meshEdgeAToBisect._triangle2 = triangle1;             

            meshEdgeBisectToA._triangle2 = triangle1;

            meshEdgeBisectToB._triangle2 = triangle2;

            meshEdgeBToBisect._triangle2 = triangle2;

            meshEdgeBisectToC._triangle1 = triangle1;
            meshEdgeBisectToC._triangle2 = triangle2;
            
            meshEdgeCtoBisect._triangle1 = triangle1;
            meshEdgeCtoBisect._triangle2 = triangle2;

            // Modify edges to their new triangle, making sure not to overwrite wrong triangle.
            if (IsMatchingTriangles(meshEdgeAC._triangle1, originalTriangle2))
                meshEdgeAC._triangle1 = triangle1;
            else
                meshEdgeAC._triangle2 = triangle1;
            if (IsMatchingTriangles(meshEdgeCA._triangle1, originalTriangle2))
                meshEdgeCA._triangle1 = triangle1;
            else
                meshEdgeCA._triangle2 = triangle1;
            

            if (IsMatchingTriangles(meshEdgeBC._triangle1, originalTriangle2))
                meshEdgeBC._triangle1 = triangle2;
            else
                meshEdgeBC._triangle2 = triangle2;
            
            if (IsMatchingTriangles(meshEdgeCB._triangle1, originalTriangle2))
                meshEdgeCB._triangle1 = triangle2;
            else
                meshEdgeCB._triangle2 = triangle2;

            // LogTriangle(originalTriangle2, "Original Triangle Two");
            // LogEdge(meshEdgeAC, "MeshEdgeAC");
            // LogTriangle(meshEdgeAC._triangle1, "MeshEdgeAC Triangle1");
            // LogEdge(meshEdgeCA, "MeshEdgeCA");
            // LogTriangle(meshEdgeCA._triangle1, "MeshEdgeCA Triangle1");
            // LogEdge(meshEdgeBC, "MeshEdgeBC");
            // LogTriangle(meshEdgeBC._triangle1, "MeshEdgeBC Triangle1");
            // LogEdge(meshEdgeCB, "MeshEdgeCB");
            // LogTriangle(meshEdgeCB._triangle1, "MeshEdgeCB Triangle1");

            // Add new edges, triangles
            // Overwrite existing originalTriangle2 with one of the new triangles

            // originalTriangle2 = triangle1;
            meshEdge._triangle2 = triangle1;
            inverseEdge._triangle2 = triangle2;
            _triangles.Add(triangle2);

            _edges.Add(meshEdgeBisectToC);
            _edges.Add(meshEdgeCtoBisect);
        }

        meshEdge._vertexA = meshEdgeAToBisect._vertexA;
        meshEdge._vertexB = meshEdgeAToBisect._vertexB;
        inverseEdge._vertexA = meshEdgeBToBisect._vertexA;
        inverseEdge._vertexB = meshEdgeBToBisect._vertexB;

        // LogEdge(meshEdge, "MeshEdgeEnd");
        // LogTriangle(meshEdge._triangle1, "MeshEdgeTriangleOneEnd");

        // LogEdge(inverseEdge, "InverseEdgeEnd");
        // LogTriangle(inverseEdge._triangle1, "InverseEdgeTriangleOneEnd");

        LogFromMethod($"--SubdivideEdge End--");

    }

    private MeshVertex GetThirdVertex(MeshTriangle meshTriangle, MeshVertex vertexOne, MeshVertex vertexTwo)
    {
        MeshVertex vertexThree = null;
        
        foreach (MeshEdge meshEdge in meshTriangle._edgeReferences)
        {
            if (!(meshEdge._vertexA._index == vertexOne._index || meshEdge._vertexA._index == vertexTwo._index))
            {
                vertexThree = meshEdge._vertexA;
                break;
            }
            
            if (!(meshEdge._vertexB._index == vertexOne._index || meshEdge._vertexB._index == vertexTwo._index))
            {
                vertexThree = meshEdge._vertexB;
                break;
            }
        }

        return vertexThree;
    }

    private List<MeshEdge> GetMeshEdgesBetweenVertices(MeshTriangle triangle, MeshVertex vertexOne, MeshVertex vertexTwo)
    {
        List<MeshEdge> meshEdges = new List<MeshEdge>();

        MeshEdge edgeAB = null;
        MeshEdge edgeBA = null;
        foreach (MeshEdge meshEdge in triangle._edgeReferences)
        {
            // LogEdge(meshEdge, "Triangle Edge Reference");
            if (meshEdge._vertexA._index == vertexOne._index && meshEdge._vertexB._index == vertexTwo._index)
                edgeAB = meshEdge;
            if (meshEdge._vertexA._index == vertexTwo._index && meshEdge._vertexB._index == vertexOne._index)
                edgeBA = meshEdge;
        }

        meshEdges.Add(edgeAB);
        meshEdges.Add(edgeBA);

        return meshEdges;
    }
    
    private List<MeshEdge> GetMeshEdgesBetweenVerticesPure(MeshVertex vertexOne, MeshVertex vertexTwo)
    {
        List<MeshEdge> meshEdges = new List<MeshEdge>();

        MeshEdge edgeAB = null;
        MeshEdge edgeBA = null;
        foreach (MeshEdge meshEdge in _edges)
        {
            // LogEdge(meshEdge, "Triangle Edge Reference");
            if (meshEdge._vertexA._index == vertexOne._index && meshEdge._vertexB._index == vertexTwo._index)
                edgeAB = meshEdge;
            if (meshEdge._vertexA._index == vertexTwo._index && meshEdge._vertexB._index == vertexOne._index)
                edgeBA = meshEdge;
        }

        meshEdges.Add(edgeAB);
        meshEdges.Add(edgeBA);

        return meshEdges;
    }

    private MeshEdge GetInverseEdge(MeshTriangle triangle, MeshEdge meshEdge)
    {
        MeshEdge inverseEdge = null;

        foreach (MeshEdge interimMeshEdge in triangle._edgeReferences)
        {
            if (interimMeshEdge._vertexA._index == meshEdge._vertexB._index && interimMeshEdge._vertexB._index == meshEdge._vertexA._index)
                inverseEdge = interimMeshEdge;
        }

        return inverseEdge;
    }

    private MeshEdge GetExistingEdge(MeshVertex vertexA, MeshVertex vertexB)
    {
        MeshEdge existingEdge = null;

        foreach (MeshEdge meshEdge in vertexA._meshEdges)
        {
            if (meshEdge._vertexB._index == vertexB._index)
            {
                existingEdge = meshEdge;
                break;
            }
        }

        return existingEdge;
    }

    private bool IsMatchingTriangles(MeshTriangle triangleOne, MeshTriangle triangleTwo, string label = null)
    {
        // LogFromMethod($"IsMatchingTriangles");

        // if (label != null)
        //     LogFromMethod($"Edge: {label}");

        // LogFromMethod($"TriangleOne: {triangleOne._vertIndices[0]}, {triangleOne._vertIndices[1]}, {triangleOne._vertIndices[2]}");
        // LogFromMethod($"TriangleTwo: {triangleTwo._vertIndices[0]}, {triangleTwo._vertIndices[1]}, {triangleTwo._vertIndices[2]}");

        bool isMatchingTriangles = false;

        int matchesCounter = 0;
        foreach (int vertexIndexTriangleOne in triangleOne._vertIndices)
        {
            foreach (int vertexIndexTriangleTwo in triangleTwo._vertIndices)
            {
                if (vertexIndexTriangleOne == vertexIndexTriangleTwo)
                {
                    matchesCounter++;
                    break;
                }

            }

            if (matchesCounter == 3)
            {
                isMatchingTriangles = true;
                break;
            }
        }
        
        return isMatchingTriangles;
    }

    private void LogAllEdges(bool withTriangles = false)
    {
        int i = 1;
        foreach (MeshEdge meshEdge in _edges)
        {
            LogEdge(meshEdge, $"MeshEdge{i}");
            if (withTriangles)
            {
                LogTriangle(meshEdge._triangle1, $"MeshEdge{i}:TriangleOne");
                if (meshEdge._triangle2 != null)
                    LogTriangle(meshEdge._triangle2, $"MeshEdge{i}:TriangleTwo");
            }
            i++;
        }
    }

    // Edge #4 is 3 - 11
    // AND Edge #15 is 3 - 11
    // Either the array edge isn't being updated correctly
    // Or I am adding duplicates
    // This could exist for _vertices, _triangles, _edges

    private void LogVertex(MeshVertex vertex, string label = "Vertex")
    {
        LogFromMethod($"{label}: Index - {vertex._index} | Pos - {vertex._position.ToString()}");
    }

    private void LogEdge(MeshEdge edge, string label = "Edge")
    {
        LogFromMethod($"{label}: VertexA[{edge._vertexA._index}] {edge._vertexA._position.ToString()} -> VertexB[{edge._vertexB._index}] {edge._vertexB._position.ToString()}");
    }
    
    private void LogTriangle(MeshTriangle triangle, string label = "Triangle")
    {
        LogFromMethod($"{label}: {triangle._vertIndices[0]}, {triangle._vertIndices[1]}, {triangle._vertIndices[2]}");
    }

    public void LogGraphConfiguration()
    {
        foreach (MeshVertex meshVertex in _vertices)
        {
            LogFromMethod($"MeshVertex: Index: {meshVertex._index}, Position: {meshVertex._position}");
        }

        foreach (MeshEdge meshEdge in _edges)
        {
            LogFromMethod($"MeshEdge: Vertex A Index: {meshEdge._vertexA._index}, Vertex B Index: {meshEdge._vertexB._index}");
        }

        foreach (MeshTriangle meshTriangle in _triangles)
        {
            LogFromMethod($"MeshTriangle: Vertex A Index: {meshTriangle._vertIndices.ToString()}, Vertex B Index: {meshTriangle._edgeReferences.Length}");
        }
    }

    public void LogUnityMeshTriangles()
    {
        for (int i = 0; i < _unityMeshTriangles.Length - 1; i += 3)
        {
            LogFromMethod($"Unity Mesh Triangles: [{_unityMeshTriangles[i]}, {_unityMeshTriangles[i + 1]}, {_unityMeshTriangles[i + 2]}]");
        }
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"MeshGraph: {message}");
    }
}