using System.Numerics;

namespace BlockCast.Core.Models;

public class MeshScene
{
    public List<Mesh> Meshes { get; set; } = [];
    public List<Vector3> _vertices { get; set; } = [];
    public Vector3 Min {get; private set;} = new(float.MaxValue, float.MaxValue, float.MaxValue);
    public Vector3 Max {get; private set;} = new(float.MinValue, float.MinValue, float.MinValue);
    public Vector3 Size => Max - Min;

    public void AddVertex(Vector3 vertex)
    {
        _vertices.Add(vertex);
        Min = new Vector3(
            Math.Min(Min.X, vertex.X),
            Math.Min(Min.Y, vertex.Y),
            Math.Min(Min.Z, vertex.Z)
        );
        Max = new Vector3(
            Math.Max(Max.X, vertex.X),
            Math.Max(Max.Y, vertex.Y),
            Math.Max(Max.Z, vertex.Z)
        );
    }
    
    public void SetVertices(List<Vector3> vertices)
    {
        _vertices = vertices;
        Min = new Vector3(
            vertices.Min(v => v.X),
            vertices.Min(v => v.Y),
            vertices.Min(v => v.Z)
        );
        Max = new Vector3(
            vertices.Max(v => v.X),
            vertices.Max(v => v.Y),
            vertices.Max(v => v.Z)
        );
    }

    public Vector3 GetVertex(int index)
    {
        return _vertices[index];
    }
}