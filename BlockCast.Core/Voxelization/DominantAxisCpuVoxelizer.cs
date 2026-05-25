using System.Numerics;
using BlockCast.Core.Models;

namespace BlockCast.Core.Voxelization;

public class DominantAxisCpuVoxelizer(VoxelizerOptions options) : Voxelizer(options)
{
    public override BlockScene Voxelize(Mesh mesh)
    {
        BlockScene blockScene = new("Main", "Me");
        BlockRegion region = new("Main", 0, 0, 0);

        foreach (var triangle in mesh.Faces)
        {
            Vector3 vertexA = mesh.GetVertex(triangle.A) - mesh.Min;
            Vector3 vertexB = mesh.GetVertex(triangle.B) - mesh.Min;
            Vector3 vertexC = mesh.GetVertex(triangle.C) - mesh.Min;

            Vector3 normal = Vector3.Normalize(Vector3.Cross(vertexB - vertexA, vertexC - vertexA));
            float d = -Vector3.Dot(normal, vertexA);
            float absX = Math.Abs(normal.X);
            float absY = Math.Abs(normal.Y);
            float absZ = Math.Abs(normal.Z);
            float minWidth, maxWidth;
            float minHeight, maxHeight;
            float minDepth, maxDepth;
            float aW, bW, cW, aH, bH, cH;
            char dominantAxis;

            if (absX >= absY && absX >= absZ) // X
            {
                dominantAxis = 'X';

                // Width = Z, Height = Y, Depth = X
                minWidth = Math.Min(vertexA.Z, Math.Min(vertexB.Z, vertexC.Z));
                maxWidth = Math.Max(vertexA.Z, Math.Max(vertexB.Z, vertexC.Z));

                minHeight = Math.Min(vertexA.Y, Math.Min(vertexB.Y, vertexC.Y));
                maxHeight = Math.Max(vertexA.Y, Math.Max(vertexB.Y, vertexC.Y));

                minDepth = Math.Min(vertexA.X, Math.Min(vertexB.X, vertexC.X));
                maxDepth = Math.Max(vertexA.X, Math.Max(vertexB.X, vertexC.X));

                aW = vertexA.Z; aH = vertexA.Y;
                bW = vertexB.Z; bH = vertexB.Y;
                cW = vertexC.Z; cH = vertexC.Y;
            }
            else if (absY >= absX && absY >= absZ) // Y
            {
                dominantAxis = 'Y';

                // Width = X, Height = Z, Depth = Y
                minWidth = Math.Min(vertexA.X, Math.Min(vertexB.X, vertexC.X));
                maxWidth = Math.Max(vertexA.X, Math.Max(vertexB.X, vertexC.X));
                
                minHeight = Math.Min(vertexA.Z, Math.Min(vertexB.Z, vertexC.Z));
                maxHeight = Math.Max(vertexA.Z, Math.Max(vertexB.Z, vertexC.Z));
                
                minDepth = Math.Min(vertexA.Y, Math.Min(vertexB.Y, vertexC.Y));
                maxDepth = Math.Max(vertexA.Y, Math.Max(vertexB.Y, vertexC.Y));

                aW = vertexA.X; aH = vertexA.Z;
                bW = vertexB.X; bH = vertexB.Z;
                cW = vertexC.X; cH = vertexC.Z;
            }
            else // Z
            {
                dominantAxis = 'Z';

                // Width = X, Height = Y, Depth = Z
                minWidth = Math.Min(vertexA.X, Math.Min(vertexB.X, vertexC.X));
                maxWidth = Math.Max(vertexA.X, Math.Max(vertexB.X, vertexC.X));
                
                minHeight = Math.Min(vertexA.Y, Math.Min(vertexB.Y, vertexC.Y));
                maxHeight = Math.Max(vertexA.Y, Math.Max(vertexB.Y, vertexC.Y));
                
                minDepth = Math.Min(vertexA.Z, Math.Min(vertexB.Z, vertexC.Z));
                maxDepth = Math.Max(vertexA.Z, Math.Max(vertexB.Z, vertexC.Z));
                
                aW = vertexA.X; aH = vertexA.Y;
                bW = vertexB.X; bH = vertexB.Y;
                cW = vertexC.X; cH = vertexC.Y;
            }
            int startWidth  = (int)Math.Floor(minWidth);
            int endWidth    = (int)Math.Ceiling(maxWidth);
            int startHeight = (int)Math.Floor(minHeight);
            int endHeight   = (int)Math.Ceiling(maxHeight);

            for (int h = startHeight; h <= endHeight; h++)
                for (int w = startWidth; w <= endWidth; w++)
                {
                    float pH = h + 0.5f;
                    float pW = w + 0.5f;

                    int blockX = 0, blockY = 0, blockZ = 0;
                    float depth = 0;
                    if (IsPointInTriangle(pW, pH, aW, aH, bW, bH, cW, cH))
                    {
                        //N * P - N * A
                        if (dominantAxis == 'X')
                        {
                            depth = -(normal.Y * pH + normal.Z * pW + d) / normal.X;
                            
                            blockX = (int)Math.Floor(depth);
                            blockY = h;
                            blockZ = w;
                        }
                        else if (dominantAxis == 'Y')
                        {
                            depth = -(normal.X * pW + normal.Z * pH + d) / normal.Y;
                            
                            blockX = w;
                            blockY = (int)Math.Floor(depth);
                            blockZ = h;
                        }
                        else if (dominantAxis == 'Z')
                        {
                            depth = -(normal.X * pW + normal.Y * pH + d) / normal.Z;
                            
                            blockX = w;
                            blockY = h;
                            blockZ = (int)Math.Floor(depth);
                        }
                    }
                }
        }
        blockScene.AddRegion(region);
        Console.WriteLine($"Region size: {region.SizeX} x {region.SizeY} x {region.SizeZ}");
        Console.WriteLine($"Block count: {region.Blocks.Count}");
        Console.WriteLine($"Total cells: {(long)region.SizeX * region.SizeY * region.SizeZ}");
        return blockScene;
    }

    //returns negative if the dot is on the right, positive if on the left
    private float CrossProduct(float pX, float pY, float v1X, float v1Y, float v2X, float v2Y)
    {
        return (pX - v1X) * (v2Y - v1Y) - (pY - v1Y) * (v2X - v1X);
    }

    private bool IsPointInTriangle(float pX, float pY, float v1X, float v1Y, float v2X, float v2Y, float v3X, float v3Y)
    {
        bool b1 = CrossProduct(pX, pY, v1X, v1Y, v2X, v2Y) < 0.0f;
        bool b2 = CrossProduct(pX, pY, v2X, v2Y, v3X, v3Y) < 0.0f;
        bool b3 = CrossProduct(pX, pY, v3X, v3Y, v1X, v1Y) < 0.0f;

        return b1 == b2 == b3;
    }
}