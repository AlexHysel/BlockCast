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

        Mesh mesh = OBJParser.Parse(obj);
        Console.WriteLine($"Faces: {mesh.Faces.Count}");
        Console.WriteLine($"Min: {mesh.Min}, Max: {mesh.Max}");
        var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());
        BlockScene scene = voxelizer.Voxelize(mesh);
        BlockRegion region = scene.Regions[0];

        Console.WriteLine($"Block count: {region.Blocks.Count}");
        Console.WriteLine($"Size: {region.SizeX} x {region.SizeY} x {region.SizeZ}");

        Assert.True(region.Blocks.Count > 0);
    }
}