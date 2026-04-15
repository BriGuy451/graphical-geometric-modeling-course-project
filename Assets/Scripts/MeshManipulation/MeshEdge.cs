public class MeshEdge {
    
    public MeshVertex _vertexA;
    public MeshVertex _vertexB;
    public MeshTriangle _triangle1;
    public MeshTriangle _triangle2;

    public void SetVertexA(MeshVertex vertex)
    {
        _vertexA = vertex;
    }
    
    public void SetVertexB(MeshVertex vertex)
    {
        _vertexB = vertex;
    }
    
    public void SetTriangle1(MeshTriangle triangle)
    {
        _triangle1 = triangle;
    }
    
    public void SetTriangle2(MeshTriangle triangle)
    {
        _triangle2 = triangle;
    }

}