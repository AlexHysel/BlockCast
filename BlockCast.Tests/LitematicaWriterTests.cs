using BlockCast.Core.Models;
using BlockCast.Core.Formats.Litematica;

namespace BlockCast.Tests;

public class LitematicaWriterTests
{
    [Fact]
    public void FileCreated()
    {
        var region = new BlockRegion("Test");
        for (int x = 0; x < 1; x++)
            for (int y = 0; y < 1; y++)
                for (int z = 0; z < 1; z++)
                    region.AddBlock(x, y, z, new Block("minecraft:stone"));

        var scene = new BlockScene("Test Scene", "Test Author");
        scene.Regions.Add(region);
        LitematicaWriter.WriteToFile("test.litematic", scene);
        Assert.True(File.Exists("test.litematic"));
    }
}
