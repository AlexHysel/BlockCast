using System.Numerics;

namespace BlockCast.Core.Models;

public class Mesh(string name = "UnnamedMesh")
{
    public string Name { get; private set; } = name;
    public List<Triangle> Faces { get; set; } = [];
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