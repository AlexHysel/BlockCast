using BlockCast.Core;
using BlockCast.Core.Models;
using BlockCast.Formats.Litematica;

namespace BlockCast.Tests;

public class LitematicaWriterTests
{
    [Fact]
    public void SimpleCube()
    {
        var region = new BlockRegion("Test", 1, 1, 1);
        for (int x = 0; x < 1; x++)
            for (int y = 0; y < 1; y++)
                for (int z = 0; z < 1; z++)
                    region.Blocks.Add(new Block(x, y, z, "minecraft:stone"));

        var scene = new BlockScene("Test Scene", "Test Author");
        scene.Regions.Add(region);
        LitematicaWriter.WriteToFile(scene, "test.litematic");
        Assert.True(File.Exists("test.litematic"));
    }
}
