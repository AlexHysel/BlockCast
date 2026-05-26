using BlockCast.Core.Models;
using BlockCast.Core.Formats.Litematica;

namespace BlockCast.Tests;

public class LitematicaWriterTests
{
    [Fact]
    public void Test1()
    {
        var region = new BlockRegion("Test");
        for (int x = 0; x < 1; x++)
            for (int y = 0; y < 1; y++)
                for (int z = 0; z < 1; z++)
                    region.AddBlock(x, y, z, new Block("minecraft:stone"));

        var scene = new BlockScene("Test Scene", "Test Author");
        scene.Regions.Add(region);

        LitematicWriter.WriteToFile("test.litematic", scene);
        
        BlockScene scene2 = LitematicReader.GetSceneFromLitematic("test.litematic");

        if (scene.Regions.Count != scene2.Regions.Count)
            Assert.Fail("Different number of regions");

        for (int i = 0; i < scene.Regions.Count; i++)
            if (!RegionsAreEqual(scene.Regions[i], scene2.Regions[i]))
                Assert.Fail("Regions are different");

        Assert.True(File.Exists("test.litematic"));
    }

    private bool RegionsAreEqual(BlockRegion regionA, BlockRegion regionB)
    {
        if (regionA.Min != regionB.Min || regionA.Max != regionB.Max)
            return false;
        for (int x = regionA.Min.X; x <= regionA.Max.X; x++)
            for (int y = regionA.Min.Y; y <= regionA.Max.Y; y++)
                for (int z = regionA.Min.Z; z <= regionA.Max.Z; z++)
                    if (regionA.GetBlock(x, y, z) != regionB.GetBlock(x, y, z))
                        return false;
        return true;
    }
}
