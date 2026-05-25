using BlockCast.Core.Models;
using System.Numerics;

namespace BlockCast.Core.Parsers;

public class OBJParser
{
    public static Mesh Parse(string obj)
    {
        Mesh mesh = new();
        foreach (string line in obj.Split('\n'))
        {
            if (line.StartsWith("v "))
            {
                float[] values = [
                    .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).Select(float.Parse)
                ];
                mesh.AddVertex(new Vector3(values));
            }
            else if (line.StartsWith("f "))
            {
                int[] values = [
                    .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).Select(str => int.Parse(str.Split('/')[0]) - 1)
                ];
                for (int i = 0; i < values.Length - 2; i++)
                    mesh.Faces.Add(new Triangle(values[0], values[i + 1], values[i + 2]));
            }
        }
        return mesh;
    }
}