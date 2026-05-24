using BlockCast.Core.Models;
using System.Numerics;

namespace BlockCast.Core.Parsers;

class OBJParser
{
    public static Mesh Parse(string obj)
    {
        Mesh mesh = new();
        foreach (string line in obj.Split('\n'))
        {
            if (line.StartsWith("v "))
            {
                float[] values = [.. line.Split(' ').Skip(1).Select(float.Parse)];
                mesh.Vertices.Add(new Vector3(values));
            }
            else if (line.StartsWith("vn "))
            {
                int[] values = [.. line.Split(' ').Skip(1).Select(int.Parse)];
                mesh.Normals.Add(new Triangle(values));
            }
        }
        return mesh;
    }
}