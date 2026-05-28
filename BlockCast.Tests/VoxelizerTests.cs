using BlockCast.Core.Models;
using BlockCast.Core.Voxelization;
using BlockCast.Core.Parsers;

namespace BlockCast.Tests;

public class VoxelizerTests
{
    [Fact]
    public void Voxelizer_SimpleCube_HasBlocks()
    {
        string obj = """
            v 0 0 0
            v 1 0 0
            v 1 1 0
            v 0 1 0
            v 0 0 1
            v 1 0 1
            v 1 1 1
            v 0 1 1
            f 1 2 3 4
            f 5 6 7 8
            f 1 2 6 5
            f 2 3 7 6
            f 3 4 8 7
            f 4 1 5 8
            """;

        MeshScene meshScene = OBJParser.Parse(obj);
        Console.WriteLine($"Faces: {meshScene.Meshes[0].Faces.Count}");
        Console.WriteLine($"Min: {meshScene.Min}, Max: {meshScene.Max}");
        var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());
        BlockScene scene = voxelizer.Voxelize(meshScene);
        BlockRegion region = scene.Regions[0];
        Assert.True(region.Blocks.Count > 0);
    }
}