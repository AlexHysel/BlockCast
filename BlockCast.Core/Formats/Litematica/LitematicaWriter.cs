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
        var arraySize = (int) Math.Ceiling((float) region.Size.X * region.Size.Y * region.Size.Z / blocksPerLong);
        long[] blockStates = new long[arraySize];

        var minX = region.Blocks.Keys.Min(k => k.X);
        var minY = region.Blocks.Keys.Min(k => k.Y);
        var minZ = region.Blocks.Keys.Min(k => k.Z);
        Console.WriteLine($"Actual min coords: {minX}, {minY}, {minZ}");

        Block[,,] blockArray = new Block[region.Size.X, region.Size.Y, region.Size.Z];
        foreach (var (pos, block) in region.Blocks)
        {
            if (pos.X >= region.Size.X || pos.Y >= region.Size.Y || pos.Z >= region.Size.Z)
            {
                Console.WriteLine($"Out of bounds: {pos.X}, {pos.Y}, {pos.Z} vs {region.Size.X}, {region.Size.Y}, {region.Size.Z}");
                continue;
            }
            blockArray[pos.X, pos.Y, pos.Z] = block;
        }
    
        int i = 0;
        for (int y = 0; y < region.Size.Y; y++)
            for (int z = 0; z < region.Size.Z; z++)
                for (int x = 0; x < region.Size.X; x++)
                {
                    Block block = blockArray[x, y, z] ?? new Block("minecraft:air");

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
                new IntTag("x", region.Max.X - region.Size.X),
                new IntTag("y", region.Max.Y - region.Size.Y),
                new IntTag("z", region.Max.Z - region.Size.Z)
            },
            new CompoundTag("Size")
            {
                new IntTag("x", region.Size.X),
                new IntTag("y", region.Size.Y),
                new IntTag("z", region.Size.Z)
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