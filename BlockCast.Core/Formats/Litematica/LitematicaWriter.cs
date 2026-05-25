using System.IO.Compression;
using BlockCast.Core.Models;
using SharpNBT;

namespace BlockCast.Core.Formats.Litematica;

public class LitematicaWriter
{
    public static void WriteToFile(string path, BlockScene scene)
    {        
        CompoundTag root = new("BlockCast")
        {
            new IntTag("Version", 6),
            new IntTag("MinecraftDataVersion", 3953),
            CreateMetadataTag(scene)
        };

        CompoundTag regions = new("Regions");
        
        foreach (var r in scene.Regions)
            regions.Add(CreateRegionTag(r));
        
        root.Add(regions);

        using var fileStream = File.Open(path, FileMode.Create);
        using var gzip = new GZipStream(fileStream, CompressionMode.Compress);
        using var writer = new TagWriter(gzip, FormatOptions.Java);
        writer.WriteTag(root);
    }

    private static CompoundTag CreateRegionTag(BlockRegion region)
    {
        var palette = region.GetPalette();
        int bitsPerEntry = (int) Math.Max(2, Math.Ceiling(Math.Log2(palette.Count)));
        int blocksPerLong = 64 / bitsPerEntry;
        var arraySize = (int) Math.Ceiling((float) region.SizeX * region.SizeY * region.SizeZ / blocksPerLong);
        long[] blockStates = new long[arraySize];

        var minX = region.Blocks.Keys.Min(k => k.Item1);
        var minY = region.Blocks.Keys.Min(k => k.Item2);
        var minZ = region.Blocks.Keys.Min(k => k.Item3);
        Console.WriteLine($"Actual min coords: {minX}, {minY}, {minZ}");

        Block[,,] blockArray = new Block[region.SizeX, region.SizeY, region.SizeZ];
        foreach (var (pos, block) in region.Blocks)
            blockArray[pos.Item1, pos.Item2, pos.Item3] = block;
        
        int i = 0;
        for (int y = 0; y < region.SizeY; y++)
            for (int z = 0; z < region.SizeZ; z++)
                for (int x = 0; x < region.SizeX; x++)
                {
                    Block? block = blockArray[x, y, z];

                    int longIndex = i / blocksPerLong;
                    int bitShift = i % blocksPerLong * bitsPerEntry;
                    int paletteIndex = palette.FindIndex(b => b.Name == block.Name);
                    if (paletteIndex == -1) paletteIndex = 0;
                    blockStates[longIndex] |= (long) paletteIndex << bitShift;
                    i++;
                }
        return new CompoundTag(region.Name)
        {
            new CompoundTag("Position")
            {
                new IntTag("x", region.PosX),
                new IntTag("y", region.PosY),
                new IntTag("z", region.PosZ)
            },
            new CompoundTag("Size")
            {
                new IntTag("x", region.SizeX),
                new IntTag("y", region.SizeY),
                new IntTag("z", region.SizeZ)
            },
            CreatePaletteTag(palette),
            new LongArrayTag("BlockStates", blockStates),
            new ListTag("TileEntities", TagType.Compound),
            new ListTag("Entities", TagType.Compound),
            new ListTag("PendingFluidTicks", TagType.Compound),
            new ListTag("PendingBlockTicks", TagType.Compound)
        };
    }

    private static CompoundTag CreateMetadataTag(BlockScene scene)
    {
        return new CompoundTag("Metadata")
        {
            new StringTag("Name", scene.Name),
            new StringTag("Author", scene.Author),
            new IntTag("TimeCreated", (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            new IntTag("TimeModified", (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };
    }

    private static ListTag CreatePaletteTag(List<Block> palette)
    {
        ListTag tag = new ListTag("BlockStatePalette", TagType.Compound);
        foreach (Block block in palette)
        {
            tag.Add(new CompoundTag(null)
            {
                new StringTag("Name", block.Name)
            });
        }
        return tag;
    }
}