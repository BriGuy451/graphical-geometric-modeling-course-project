using System.Numerics;

public class MeshTriangle {
    
    public int[] _vertIndices = new int[3];
    public MeshEdge[] _edgeReferences;
    public Vector3 _normal;
    public float _centroid;
    public float _area;
    public float[,] barycentricBias;

    public void SetVertexIndices(int[] indices)
    {
        _vertIndices = indices;
    }
    
    public void SetEdgeReferences(MeshEdge[] edgeReferences)
    {
        _edgeReferences = edgeReferences;
    }
    
    public void SetNormal(Vector3 normal)
    {
        _normal = normal;
    }

}