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
                new IntTag("x", 5),
                new IntTag("y", 5),
                new IntTag("z", 5)
            }
        };
        root.Add(metadata);

        var regions = new CompoundTag("Regions");

        foreach (var r in scene.Regions)
        {
            var palette = r.GetPalette();
            int sX = r.Blocks.GetLength(0);
            int sY = r.Blocks.GetLength(1);
            int sZ = r.Blocks.GetLength(2);
            int size = sX * sY * sZ;
            int paletteSize = palette.Count;
            int bitsPerEntry = (int) Math.Max(2, Math.Ceiling(Math.Log2(paletteSize)));
            int blocksPerLong = 64 / bitsPerEntry;
            var arraySize = (int) Math.Ceiling((float) size / blocksPerLong);
            long[] blockStates = new long[arraySize];
            int i = 0;

            for (int x = 0; x < sX; x++)
                for (int y = 0; y < sY; y++)
                    for (int z = 0; z < sZ; z++)
                    {
                        Block block = r.Blocks[x, y, z];
                        int longIndex = i / blocksPerLong;
                        int bitShift = i % blocksPerLong * bitsPerEntry;
                        blockStates[longIndex] |= (long) palette.FindIndex(b => b == block) << bitShift;
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
                    new IntTag("x", sX),
                    new IntTag("y", sY),
                    new IntTag("z", sZ)
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