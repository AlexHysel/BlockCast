using System.Drawing;
using System.IO.Compression;
using BlockCast.Core.Models;
using SharpNBT;

namespace BlockCast.Core.Formats.Litematica;

public class LitematicaWriter
{
    public static void WriteToFile(string path, BlockScene scene)
    {        
        var root = new CompoundTag("BlockCast")
        {
            new IntTag("Version", 6),
            new IntTag("MinecraftDataVersion", 3953)
        };

        var metadata = new CompoundTag("Metadata")
        {
            new StringTag("Name", scene.Name),
            new StringTag("Author", scene.Author),
            new IntTag("RegionCount", 1),
            new CompoundTag("EnclosingSize")
            {
                new IntTag("x", scene.MaxX - scene.MinX),
                new IntTag("y", scene.MaxY - scene.MinY),
                new IntTag("z", scene.MaxZ - scene.MinZ)
            }
        };
        root.Add(metadata);

        var regions = new CompoundTag("Regions");
        
        foreach (var r in scene.Regions)
        {
            var palette = r.GetPalette();
            Console.WriteLine($"Region {r.Name} palette size: {palette.Count}");
            int bitsPerEntry = (int) Math.Max(2, Math.Ceiling(Math.Log2(palette.Count)));
            int blocksPerLong = 64 / bitsPerEntry;
            var arraySize = (int) Math.Ceiling((float) (scene.MaxX - scene.MinX) * (scene.MaxY - scene.MinY) * (scene.MaxZ - scene.MinZ) / blocksPerLong);
            long[] blockStates = new long[arraySize];
            int i = 0;

            
            for (int y = scene.MinY; y < scene.MaxY; y++)
                for (int z = scene.MinZ; z < scene.MaxZ; z++)
                    for (int x = scene.MinX; x < scene.MaxX; x++)
                    {
                        Block? block = r.GetBlock(x, y, z);
                        int longIndex = i / blocksPerLong;
                        int bitShift = i % blocksPerLong * bitsPerEntry;
                        int paletteIndex = palette.FindIndex(b => b.Name == block.Name);
                        if (paletteIndex == -1) paletteIndex = 0;
                        blockStates[longIndex] |= (long) paletteIndex << bitShift;
                        i++;
                    }
            var region = new CompoundTag(r.Name)
            {
                new CompoundTag("Position")
                {
                    new IntTag("x", r.X),
                    new IntTag("y", r.Y),
                    new IntTag("z", r.Z)
                },
                new CompoundTag("Size")
                {
                    new IntTag("x", scene.MaxX - scene.MinX),
                    new IntTag("y", scene.MaxY - scene.MinY),
                    new IntTag("z", scene.MaxZ - scene.MinZ)
                },
                CreatePaletteTag(palette),
                new LongArrayTag("BlockStates", blockStates),
                new ListTag("TileEntities", TagType.Compound),
                new ListTag("Entities", TagType.Compound),
                new ListTag("PendingFluidTicks", TagType.Compound),
                new ListTag("PendingBlockTicks", TagType.Compound)
            };
            regions.Add(region);
        };
        
        root.Add(regions);

        using var fileStream = File.Open(path, FileMode.Create);
        using var gzip = new GZipStream(fileStream, CompressionMode.Compress);
        using var writer = new TagWriter(gzip, FormatOptions.Java);
        writer.WriteTag(root);
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