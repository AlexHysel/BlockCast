using BlockCast.Core.Models;
using System.Numerics;

namespace BlockCast.Core.Parsers;

public class OBJParser
{
    public static MeshScene Parse(string obj, IProgress<float>? progress = null)
    {
        
        MeshScene scene = new();
        scene.Meshes.Add(new Mesh("Unnamed"));
        int meshIndex = 0;
        string[] lines = obj.Split('\n');
        for (int l = 0; l < lines.Length; l++)
        {
            string line = lines[l];
            if (line.StartsWith("o "))
            {
                scene.Meshes.Add(new Mesh(line[2..].Trim()));
                meshIndex++;
                progress?.Report(100f / lines.Length * l);
            }
            else if (line.StartsWith("v "))
            {
                float[] values = [
                    .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).Select(float.Parse)
                ];
                scene.AddVertex(new Vector3(values));
            }
            else if (line.StartsWith("f "))
            {
                int[] values = [
                    .. line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).Select(str => int.Parse(str.Split('/')[0]) - 1)
                ];
                for (int i = 0; i < values.Length - 2; i++)
                    scene.Meshes[meshIndex].Faces.Add(new Triangle(values[0], values[i + 1], values[i + 2]));
            }
        }
        progress?.Report(100f);
        return scene;
    }
}