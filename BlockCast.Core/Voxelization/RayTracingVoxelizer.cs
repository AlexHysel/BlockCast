using System.Numerics;
using BlockCast.Core.Models;
using BlockCast.Core.Voxelization;

public class RayTracingVoxelizer(VoxelizerOptions options): Voxelizer(options)
{
    public override BlockScene Voxelize(MeshScene meshScene, IProgress<float>? progress)
    {
        BlockScene scene = new("scene", "BlockCast");
        BlockRegion region = new("region");
        int minX = (int) Math.Floor(meshScene.Min.X);
        int minZ = (int) Math.Floor(meshScene.Min.Z);
        int maxX = (int) Math.Ceiling(meshScene.Max.X);
        int maxZ = (int) Math.Ceiling(meshScene.Max.Z);

        Triangle[] faces = meshScene.Meshes.SelectMany(m => m.Faces).ToArray();
        for (int x = minX; x < maxX; x++)
        {
            for (int z = minZ; z < maxZ; z++)
            {
                List<float> intersectionsY = [];
                Vector3 point = new(x + 0.5f, 0, z + 0.5f);

                foreach (Triangle face in faces)
                {
                    Vector3 faceA = meshScene.GetVertex(face.A);
                    Vector3 faceB = meshScene.GetVertex(face.B);
                    Vector3 faceC = meshScene.GetVertex(face.C);

                    if (Utils.IsPointInTriangleXZ(faceA, faceB, faceC, point))
                    {
                        float areaABC = Utils.SignedArea(faceB, faceC, faceA);
                        if (areaABC != 0)
                        {
                            float weightC = Utils.SignedArea(faceB, point, faceA) / areaABC;
                            float weightB = Utils.SignedArea(point, faceC, faceA) / areaABC;
                            float weightA = Utils.SignedArea(faceC, point, faceB) / areaABC;
                            float intersection = weightA * faceA.Y + weightB * faceB.Y + weightC * faceC.Y;
                            intersection = (float) Math.Round(intersection);
                            intersectionsY.Add(intersection);
                        }
                    }
                }
                intersectionsY = intersectionsY.Order().Distinct().Reverse().ToList();
                if (intersectionsY.Count == 0) continue;
                for (int i1 = intersectionsY.Count - 1; i1 >= 1; i1 -= 2)
                {
                    for (float y = intersectionsY[i1]; y < intersectionsY[i1 - 1]; y++)
                    {
                        region.AddBlock(x, (int) y, z, new Block("minecraft:stone"));
                    }
                }
            }
            progress?.Report(100f / maxX * x);
        }
        scene.AddRegion(region);
        return scene;     
    }
}