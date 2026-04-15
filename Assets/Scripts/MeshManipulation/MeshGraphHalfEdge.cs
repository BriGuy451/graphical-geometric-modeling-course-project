using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VertexHE
{
    public int _index;
    public Vector3 _position;
    public EdgeHE _halfEdge; // outgoing
}

public class EdgeHE
{
    public VertexHE _vertex; // points to this vertex
    public EdgeHE _opposite; // reverse direction
    public EdgeHE _next; // next around face
    public FaceHE _face; // owning face

    public void SetMembers(EdgeHE opposite, EdgeHE next, FaceHE face)
    {
        _opposite = opposite;
        _next = next;
        _face = face;
    }
}
public class FaceHE
{
    public EdgeHE _halfEdge; // one of it's 3
}

public class MeshGraphHalfEdge {
    
    private int[] _unityMeshTriangles;

    public List<VertexHE> _vertices;
    public List<EdgeHE> _edgesHalf;
    public List<FaceHE> _faces;

    public MeshGraphHalfEdge()
    {
        _vertices = new List<VertexHE>();
        _edgesHalf = new List<EdgeHE>();
        _faces = new List<FaceHE>();
    }

    public MeshGraphHalfEdge(Mesh mesh)
    {
        _vertices = new List<VertexHE>();
        _edgesHalf = new List<EdgeHE>();
        _faces = new List<FaceHE>();

        InitializeGraph(mesh, out _vertices, out _edgesHalf, out _faces);
    }

    public void InitializeGraph(Mesh mesh, out List<VertexHE> meshVertices, out List<EdgeHE> meshEdgesHalf, out List<FaceHE> meshFaces)
    {
        Vector3[] unityMeshVertices = mesh.vertices;
        int[] unityMeshTriangles = mesh.triangles;
        _unityMeshTriangles = mesh.triangles;

        List<VertexHE> tempMeshVertices = new List<VertexHE>();
        List<EdgeHE> tempMeshEdges = new List<EdgeHE>();
        List<FaceHE> tempMeshFaces = new List<FaceHE>();

        Dictionary<int, VertexHE> indicesToVertexDict = new Dictionary<int, VertexHE>();
        for (int i = 0; i < unityMeshTriangles.Length - 1; i += 3)
        {
            int vertexAIndex = unityMeshTriangles[i];
            int vertexBIndex = unityMeshTriangles[i + 1];
            int vertexCIndex = unityMeshTriangles[i + 2];

            int[] triangleArr = new int[3]{vertexAIndex, vertexBIndex, vertexCIndex};
            LogTriangle(triangleArr, "Current Triangle To Process");

            Dictionary<int, int> triangleEdgeDict = new Dictionary<int, int>
            {
                { triangleArr[0], triangleArr[1] },
                { triangleArr[1], triangleArr[2] },
                { triangleArr[2], triangleArr[0] }
            };
            
            Vector3 vertexPositionA = unityMeshVertices[vertexAIndex];
            Vector3 vertexPositionB = unityMeshVertices[vertexBIndex];
            Vector3 vertexPositionC = unityMeshVertices[vertexCIndex];
            Dictionary<int, Vector3> triangleEdgeDictVector = new Dictionary<int, Vector3>
            {
                { triangleArr[0], vertexPositionB },
                { triangleArr[1], vertexPositionC },
                { triangleArr[2], vertexPositionA }
            };

            VertexHE vertexA = null;
            VertexHE vertexB = null;
            VertexHE vertexC = null;
            if (GetCorrelatingReverseEdgeVertex(indicesToVertexDict, triangleEdgeDict, triangleArr, out vertexA, out vertexB))
            {
                LogFromMethod($"Reverse Edge Block");
                VertexHE existingVertexB = vertexB;
                VertexHE existingVertexA = vertexA;
                VertexHE vertexD = new VertexHE()
                {
                    _index = triangleEdgeDict[existingVertexA._index],
                    _position = triangleEdgeDictVector[existingVertexA._index]
                };

                LogFromMethod($"VertexD Index: {vertexD._index} {vertexD._position.ToString()}");

                // BtoAtoD
                EdgeHE heAB = existingVertexA._halfEdge;

                EdgeHE heBA = new EdgeHE()
                {
                    _vertex = existingVertexA,
                };

                heAB._opposite = heBA;

                EdgeHE heAD = new EdgeHE()
                {
                    _vertex = vertexD
                };
                EdgeHE heDB = new EdgeHE()
                {
                    _vertex = existingVertexB
                };

                FaceHE faceABC = new FaceHE()
                {
                    _halfEdge = heBA
                };

                vertexD._halfEdge = heDB;

                heAD.SetMembers(null, heDB, faceABC);
                heDB.SetMembers(null, heBA, faceABC);
                heBA.SetMembers(heAB, heAD, faceABC);

                if (!indicesToVertexDict.ContainsKey(vertexD._index))
                {
                    indicesToVertexDict.Add(vertexD._index, vertexD);
                    tempMeshVertices.Add(vertexD);
                }
                
                tempMeshEdges.Add(heBA);
                tempMeshEdges.Add(heAD);
                tempMeshEdges.Add(heDB);
                tempMeshFaces.Add(faceABC);
            } else if (indicesToVertexDict.ContainsKey(vertexAIndex) || indicesToVertexDict.ContainsKey(vertexBIndex) || indicesToVertexDict.ContainsKey(vertexCIndex))
            {
                LogFromMethod($"Non-Reverse Existing Vertex Block");
                VertexHE existingVertexA = null;
                VertexHE existingVertexB = null;
                VertexHE existingVertexC = null;

                if (indicesToVertexDict.TryGetValue(vertexAIndex, out existingVertexA) && indicesToVertexDict.TryGetValue(vertexBIndex, out existingVertexB))
                {
                    // A B D
                    LogFromMethod($"Two Existing: A & B");
                    int vertexDIndex = triangleEdgeDict[vertexBIndex];

                    vertexA = existingVertexA;
                    vertexB = existingVertexB;
                    VertexHE vertexD = new VertexHE()
                    {
                        _index = vertexDIndex,
                        _position = triangleEdgeDictVector[vertexBIndex]
                    };

                    EdgeHE heAB = new EdgeHE()
                    {
                        _vertex = vertexB,
                    };
                    EdgeHE heBD = new EdgeHE()
                    {
                        _vertex = vertexD,
                    };
                    EdgeHE heDA = new EdgeHE()
                    {
                        _vertex = vertexA,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heAB
                    };

                    vertexA._halfEdge = heAB;
                    vertexB._halfEdge = heBD;
                    vertexD._halfEdge = heDA;

                    heAB.SetMembers(null, heBD, faceABC);
                    heBD.SetMembers(null, heDA, faceABC);
                    heDA.SetMembers(null, heAB, faceABC);

                    indicesToVertexDict.Add(vertexDIndex, vertexD);

                    tempMeshVertices.Add(vertexD);
                    tempMeshEdges.Add(heAB);
                    tempMeshEdges.Add(heBD);
                    tempMeshEdges.Add(heDA);
                    tempMeshFaces.Add(faceABC);
                } 
                else if (indicesToVertexDict.TryGetValue(vertexAIndex, out existingVertexA) && indicesToVertexDict.TryGetValue(vertexCIndex, out existingVertexC))
                {
                    // A D C
                    LogFromMethod($"Two Existing: A & C");
                    int vertexDIndex = triangleEdgeDict[vertexAIndex];

                    vertexA = existingVertexA;
                    vertexC = existingVertexC;
                    VertexHE vertexD = new VertexHE()
                    {
                        _index = vertexDIndex,
                        _position = triangleEdgeDictVector[vertexAIndex]
                    };

                    EdgeHE heAD = new EdgeHE()
                    {
                        _vertex = vertexD,
                    };
                    EdgeHE heDC = new EdgeHE()
                    {
                        _vertex = vertexC,
                    };
                    EdgeHE heCA = new EdgeHE()
                    {
                        _vertex = vertexA,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heAD
                    };

                    vertexA._halfEdge = heAD;
                    vertexD._halfEdge = heDC;
                    vertexC._halfEdge = heCA;

                    heAD.SetMembers(null, heDC, faceABC);
                    heDC.SetMembers(null, heCA, faceABC);
                    heCA.SetMembers(null, heAD, faceABC);

                    indicesToVertexDict.Add(vertexDIndex, vertexD);

                    tempMeshVertices.Add(vertexD);
                    tempMeshEdges.Add(heAD);
                    tempMeshEdges.Add(heDC);
                    tempMeshEdges.Add(heCA);
                    tempMeshFaces.Add(faceABC);
                }
                else if (indicesToVertexDict.TryGetValue(vertexBIndex, out existingVertexB) && indicesToVertexDict.TryGetValue(vertexCIndex, out existingVertexC))
                {
                    // D B C
                    LogFromMethod($"Two Existing: B & C");
                    int vertexDIndex = triangleEdgeDict[vertexCIndex];

                    vertexB = existingVertexB;
                    vertexC = existingVertexC;
                    VertexHE vertexD = new VertexHE()
                    {
                        _index = vertexDIndex,
                        _position = triangleEdgeDictVector[vertexCIndex]
                    };

                    EdgeHE heDB = new EdgeHE()
                    {
                        _vertex = vertexB,
                    };
                    EdgeHE heBC = new EdgeHE()
                    {
                        _vertex = vertexC,
                    };
                    EdgeHE heCD = new EdgeHE()
                    {
                        _vertex = vertexD,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heDB
                    };

                    vertexD._halfEdge = heDB;
                    vertexB._halfEdge = heBC;
                    vertexC._halfEdge = heCD;

                    heDB.SetMembers(null, heBC, faceABC);
                    heBC.SetMembers(null, heCD, faceABC);
                    heCD.SetMembers(null, heDB, faceABC);

                    indicesToVertexDict.Add(vertexDIndex, vertexD);

                    tempMeshVertices.Add(vertexD);
                    tempMeshEdges.Add(heDB);
                    tempMeshEdges.Add(heBC);
                    tempMeshEdges.Add(heCD);
                    tempMeshFaces.Add(faceABC);
                }
                else if (indicesToVertexDict.TryGetValue(vertexAIndex, out existingVertexA))
                {
                    LogFromMethod($"One Existing: A");
                    // AtoEtoF
                    int vertexEIndex = triangleEdgeDict[vertexAIndex];
                    int vertexFIndex = triangleEdgeDict[vertexBIndex];

                    vertexA = existingVertexA;
                    VertexHE vertexE = new VertexHE()
                    {
                      _index = vertexEIndex,
                      _position = vertexPositionB
                    };
                    VertexHE vertexF = new VertexHE()
                    {
                        _index = vertexFIndex,
                        _position = vertexPositionC
                    };

                    EdgeHE heAE = new EdgeHE()
                    {
                        _vertex = vertexE,
                    };
                    EdgeHE heEF = new EdgeHE()
                    {
                        _vertex = vertexF,
                    };
                    EdgeHE heFA = new EdgeHE()
                    {
                        _vertex = vertexA,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heAE
                    };

                    vertexA._halfEdge = heAE;
                    vertexE._halfEdge = heEF;
                    vertexF._halfEdge = heFA;

                    heAE.SetMembers(null, heEF, faceABC);
                    heEF.SetMembers(null, heFA, faceABC);
                    heFA.SetMembers(null, heAE, faceABC);

                    indicesToVertexDict.Add(vertexEIndex, vertexE);
                    indicesToVertexDict.Add(vertexFIndex, vertexF);

                    tempMeshVertices.Add(vertexE);
                    tempMeshVertices.Add(vertexF);
                    tempMeshEdges.Add(heAE);
                    tempMeshEdges.Add(heEF);
                    tempMeshEdges.Add(heFA);
                    tempMeshFaces.Add(faceABC);
                }
                else if (indicesToVertexDict.TryGetValue(vertexBIndex, out existingVertexB))
                {
                    LogFromMethod($"One Existing: B");
                    // EtoAtoF
                    int vertexFIndex = triangleEdgeDict[vertexCIndex];
                    int vertexEIndex = triangleEdgeDict[vertexBIndex];

                    vertexA = existingVertexB;
                    VertexHE vertexE = new VertexHE()
                    {
                        _index = vertexEIndex,
                        _position = vertexPositionA                        
                    };
                    VertexHE vertexF = new VertexHE()
                    {
                        _index = vertexFIndex,
                        _position = vertexPositionC

                    };

                    EdgeHE heEA = new EdgeHE()
                    {
                        _vertex = vertexA,
                    };
                    EdgeHE heAF = new EdgeHE()
                    {
                        _vertex = vertexF,
                    };
                    EdgeHE heFE = new EdgeHE()
                    {
                        _vertex = vertexE,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heEA
                    };

                    vertexA._halfEdge = heAF;
                    vertexE._halfEdge = heEA;
                    vertexF._halfEdge = heFE;

                    heAF.SetMembers(null, heFE, faceABC);
                    heEA.SetMembers(null, heAF, faceABC);
                    heFE.SetMembers(null, heEA, faceABC);

                    indicesToVertexDict.Add(vertexEIndex, vertexE);
                    indicesToVertexDict.Add(vertexFIndex, vertexF);

                    tempMeshVertices.Add(vertexE);
                    tempMeshVertices.Add(vertexF);
                    tempMeshEdges.Add(heAF);
                    tempMeshEdges.Add(heEA);
                    tempMeshEdges.Add(heFE);
                    tempMeshFaces.Add(faceABC);
                }
                else if (indicesToVertexDict.TryGetValue(vertexCIndex, out existingVertexC))
                {
                    LogFromMethod($"One Existing: C");
                    // EtoFtoA
                    int vertexFIndex = triangleEdgeDict[vertexAIndex];
                    int vertexEIndex = triangleEdgeDict[vertexCIndex];

                    vertexC = existingVertexC;
                    VertexHE vertexE = new VertexHE()
                    {
                        _index = vertexEIndex,
                        _position = vertexPositionA
                    };
                    VertexHE vertexF = new VertexHE()
                    {
                        _index = vertexFIndex,
                        _position = vertexPositionB
                    };

                    EdgeHE heEF = new EdgeHE()
                    {
                        _vertex = vertexF,
                    };
                    EdgeHE heFA = new EdgeHE()
                    {
                        _vertex = vertexC,
                    };
                    EdgeHE heAE = new EdgeHE()
                    {
                        _vertex = vertexE,
                    };

                    FaceHE faceABC = new FaceHE()
                    {
                        _halfEdge = heEF
                    };

                    vertexC._halfEdge = heAE;
                    vertexE._halfEdge = heEF;
                    vertexF._halfEdge = heFA;

                    heAE.SetMembers(null, heEF, faceABC);
                    heEF.SetMembers(null, heFA, faceABC);
                    heFA.SetMembers(null, heAE, faceABC);

                    indicesToVertexDict.Add(vertexFIndex, vertexE);
                    indicesToVertexDict.Add(vertexEIndex, vertexF);

                    tempMeshVertices.Add(vertexE);
                    tempMeshVertices.Add(vertexF);
                    tempMeshEdges.Add(heAE);
                    tempMeshEdges.Add(heEF);
                    tempMeshEdges.Add(heFA);
                    tempMeshFaces.Add(faceABC);
                }

            } else
            { 
                LogFromMethod($"Pure Triangle Block");
                vertexA = new VertexHE()
                {
                    _index = vertexAIndex,
                    _position = vertexPositionA
                };
                vertexB = new VertexHE()
                {
                    _index = vertexBIndex,
                    _position = vertexPositionB
                };
                vertexC = new VertexHE()
                {
                    _index = vertexCIndex,
                    _position = vertexPositionC
                };


                EdgeHE halfEdgeAtoB = new EdgeHE()
                {
                    _vertex = vertexB,
                };
                EdgeHE halfEdgeBtoC = new EdgeHE()
                {
                    _vertex = vertexC,
                };
                EdgeHE halfEdgeCtoA = new EdgeHE()
                {
                    _vertex = vertexA,
                };

                FaceHE faceABC = new FaceHE()
                {
                    _halfEdge = halfEdgeAtoB
                };

                vertexA._halfEdge = halfEdgeAtoB;
                vertexB._halfEdge = halfEdgeBtoC;
                vertexC._halfEdge = halfEdgeCtoA;

                halfEdgeAtoB.SetMembers(null, halfEdgeBtoC, faceABC);
                halfEdgeBtoC.SetMembers(null, halfEdgeCtoA, faceABC);
                halfEdgeCtoA.SetMembers(null, halfEdgeAtoB, faceABC);

                indicesToVertexDict.Add(vertexAIndex, vertexA);
                indicesToVertexDict.Add(vertexBIndex, vertexB);
                indicesToVertexDict.Add(vertexCIndex, vertexC);

                tempMeshVertices.Add(vertexA);
                tempMeshVertices.Add(vertexB);
                tempMeshVertices.Add(vertexC);
                tempMeshEdges.Add(halfEdgeAtoB);
                tempMeshEdges.Add(halfEdgeBtoC);
                tempMeshEdges.Add(halfEdgeCtoA);
                tempMeshFaces.Add(faceABC);
            }

        }

        LogFromMethod($"Face Count: {tempMeshFaces.Count}");

        meshVertices = tempMeshVertices;
        meshEdgesHalf = tempMeshEdges;
        meshFaces = tempMeshFaces;
    }

    private bool GetCorrelatingReverseEdgeVertex(Dictionary<int, VertexHE> vertexIndexDict, Dictionary<int, int> edgeDict, int[] triangleIndexes, out VertexHE vertexA, out VertexHE vertexB)
    {   
        bool reverseEdgeFound = false;

        vertexA = null;
        vertexB = null;

        // I need to look at each vertex for the triangle indices
        // Seeing which one, has is the inverse of a triangle pairing
        // triangle {A -> B -> C -> A}, one of these is the inverse of an existing triangle index
        // so one of my existing vertices has an inverted version of one of these, something like {B -> A}
        // existingVert.he.vertex.index tells me what a existingvert is pointing to
        // if I have the inverse of this with my triangle indexes, i found the reverse edge

        for (int i = 0; i < triangleIndexes.Length; i++)
        {
            if (vertexIndexDict.ContainsKey(triangleIndexes[i]))
            {
                VertexHE vertex = vertexIndexDict[triangleIndexes[i]];
                int vertexAIndex = vertex._index;
                int vertexBIndex = vertex._halfEdge._vertex._index;

                if (edgeDict.ContainsKey(vertexBIndex))
                {
                    int possibleEdgeVertexValue = edgeDict[vertexBIndex];
                    if (possibleEdgeVertexValue == vertexAIndex)
                    {
                        vertexA = vertex;
                        vertexB = vertex._halfEdge._vertex; ;

                        reverseEdgeFound = true;
                        break;
                    }
                }
            }
        }

        return reverseEdgeFound;
    }

    public void LogEdgePair(EdgeHE edge)
    {
        int edgeVertexAIndex = edge._vertex._index;
        Vector3 edgeVertexAPosition = edge._vertex._position;
        int edgeVertexBIndex = edge._next._vertex._index;
        Vector3 edgeVertexBPosition = edge._next._vertex._position;

        LogFromMethod($"Edge Pair: VertexA[{edgeVertexAIndex}] {edgeVertexAPosition.ToString()}  -> VertexB[{edgeVertexBIndex}] {edgeVertexBPosition.ToString()}");
    }

    private void LogTriangle(int[] triangle, string label = "Triangle")
    {
        LogFromMethod($"{label}: [{triangle[0]},{triangle[1]},{triangle[2]}]");
    }

    public void PrintLocalLoop()
    {
        FaceHE faceHE = _faces[0];
        
        for (int i = 0; i < 3; i++)
        {
            EdgeHE heAB = faceHE._halfEdge;
            LogFromMethod($"Vertex[{heAB._vertex._index}]");
            heAB = heAB._next;
        }
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"MeshGraphHalfEdge: {message}");
    }
}