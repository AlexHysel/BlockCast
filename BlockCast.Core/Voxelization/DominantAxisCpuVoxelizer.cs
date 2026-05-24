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
            Vector3 a = mesh.GetVertex(triangle.A);
            Vector3 b = mesh.GetVertex(triangle.B);
            Vector3 c = mesh.GetVertex(triangle.C);

            Vector3 normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            float d = -Vector3.Dot(normal, a);
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
                minWidth = Math.Min(a.Z, Math.Min(b.Z, c.Z));
                maxWidth = Math.Max(a.Z, Math.Max(b.Z, c.Z));
                
                minHeight = Math.Min(a.Y, Math.Min(b.Y, c.Y));
                maxHeight = Math.Max(a.Y, Math.Max(b.Y, c.Y));
                
                minDepth = Math.Min(a.X, Math.Min(b.X, c.X));
                maxDepth = Math.Max(a.X, Math.Max(b.X, c.X));

                aW = a.Z; aH = a.Y;
                bW = b.Z; bH = b.Y;
                cW = c.Z; cH = c.Y;
            }
            else if (absY >= absX && absY >= absZ) // Y
            {
                dominantAxis = 'Y';

                // Width = X, Height = Z, Depth = Y
                minWidth = Math.Min(a.X, Math.Min(b.X, c.X));
                maxWidth = Math.Max(a.X, Math.Max(b.X, c.X));
                
                minHeight = Math.Min(a.Z, Math.Min(b.Z, c.Z));
                maxHeight = Math.Max(a.Z, Math.Max(b.Z, c.Z));
                
                minDepth = Math.Min(a.Y, Math.Min(b.Y, c.Y));
                maxDepth = Math.Max(a.Y, Math.Max(b.Y, c.Y));

                aW = a.X; aH = a.Z;
                bW = b.X; bH = b.Z;
                cW = c.X; cH = c.Z;
            }
            else // Z
            {
                dominantAxis = 'Z';

                // Width = X, Height = Y, Depth = Z
                minWidth = Math.Min(a.X, Math.Min(b.X, c.X));
                maxWidth = Math.Max(a.X, Math.Max(b.X, c.X));
                
                minHeight = Math.Min(a.Y, Math.Min(b.Y, c.Y));
                maxHeight = Math.Max(a.Y, Math.Max(b.Y, c.Y));
                
                minDepth = Math.Min(a.Z, Math.Min(b.Z, c.Z));
                maxDepth = Math.Max(a.Z, Math.Max(b.Z, c.Z));
                
                aW = a.X; aH = a.Y;
                bW = b.X; bH = b.Y;
                cW = c.X; cH = c.Y;
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

                        region.Blocks[(blockX, blockY, blockZ)] = new Block("minecraft:stone");
                    }
                }
        }
        blockScene.AddRegion(region);
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