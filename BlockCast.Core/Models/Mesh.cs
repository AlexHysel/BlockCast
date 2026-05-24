using System.Numerics;

namespace BlockCast.Core.Models;

public class Mesh
{
    public List<Vector3> Vertices { get; set; } = [];
    public List<Triangle> Normals { get; set; } = [];
}

public record Triangle(int A, int B, int C);