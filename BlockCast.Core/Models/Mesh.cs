using System.Numerics;

namespace BlockCast.Core.Models;

public class Mesh
{
    private List<Vector3> _vertices = [];
    public List<Triangle> Faces { get; set; } = [];
    private Vector3 _min = new(float.MaxValue, float.MaxValue, float.MaxValue);
    private Vector3 _max = new(float.MinValue, float.MinValue, float.MinValue);
    public Vector3 Size => _max - _min;

    public void AddVertex(Vector3 vertex)
    {
        _vertices.Add(vertex);
        _min = new Vector3(
            Math.Min(_min.X, vertex.X),
            Math.Min(_min.Y, vertex.Y),
            Math.Min(_min.Z, vertex.Z)
        );
        _max = new Vector3(
            Math.Max(_max.X, vertex.X),
            Math.Max(_max.Y, vertex.Y),
            Math.Max(_max.Z, vertex.Z)
        );
    }
    public void SetVertices(List<Vector3> vertices)
    {
        _vertices = vertices;
        _min = new Vector3(
            vertices.Min(v => v.X),
            vertices.Min(v => v.Y),
            vertices.Min(v => v.Z)
        );
        _max = new Vector3(
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

public record Triangle
{
    public int A {get; init;}
    public int B {get; init;}
    public int C {get; init;}

    public Triangle(int a, int b, int c)
    {
        A = a;
        B = b;
        C = c;
    }

    public Triangle(int[] vertices)
    {
        A = vertices[0];
        B = vertices[1];
        C = vertices[2];
    }
}