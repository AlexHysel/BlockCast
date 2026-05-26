using System.IO.Compression;
using BlockCast.Core.Models;
using SharpNBT;

namespace BlockCast.Tests;

class LitematicReader
{
    public static BlockScene GetSceneFromLitematic(string filePath)
    {
        using Stream stream = File.OpenRead(filePath);
        using GZipStream gzip = new(stream, CompressionMode.Decompress);
        using TagReader reader = new(gzip, FormatOptions.Java);

        CompoundTag root = reader.ReadTag<CompoundTag>() ?? throw new Exception("Invalid Litematic file");

        CompoundTag metadata = root.Get<CompoundTag>("Metadata") ?? throw new Exception("Missing Metadata tag");
        string name = metadata.Get<StringTag>("Name")?.Value ?? "Unnamed";
        string author = metadata.Get<StringTag>("Author")?.Value ?? "Unknown";

        CompoundTag regions = root.Get<CompoundTag>("Regions") ?? throw new Exception("Missing Metadata tag");

        BlockScene scene = new(name, author);

        foreach (CompoundTag region in regions)
        {
            BlockRegion blockRegion = new(region.Name);
            ListTag blockPalette = region.Get<ListTag>("BlockStatePalette");
            LongArrayTag blocks = region.Get<LongArrayTag>("BlockStates");
            CompoundTag posTag = region.Get<CompoundTag>("Position");
            BlockPos min = new(posTag.Get<IntTag>("x").Value, posTag.Get<IntTag>("y").Value, posTag.Get<IntTag>("z").Value);
            CompoundTag sizeTag = region.Get<CompoundTag>("Size");
            BlockPos max = new BlockPos(sizeTag.Get<IntTag>("x"), sizeTag.Get<IntTag>("y"), sizeTag.Get<IntTag>("z")) + min - new BlockPos(1, 1, 1);

            int bitsPerBlock = (int) Math.Max(2, Math.Ceiling(Math.Log2(blockPalette.Count)));
            int blocksPerLong = 64 / bitsPerBlock;
            int longIndex = 0;
            int blockIndex = 0;

            for (int y = min.Y; y <= max.Y; y++)
            for (int z = min.Z; z <= max.Z; z++)
            for (int x = min.X; x <= max.X; x++)
            {
                if (blockIndex >= blocksPerLong)
                {
                    blockIndex = 0;
                    longIndex++;
                }
                if (longIndex >= blocks.Count)
                    break;
                int mask = (1 << bitsPerBlock) - 1;
                int blockInPalette = (int) ((blocks[longIndex] >> bitsPerBlock * blockIndex) & mask);
                Block block = new(((CompoundTag)blockPalette[blockInPalette]).Get<StringTag>("Name").Value);
                if (block != new Block("minecraft:air"))
                    blockRegion.AddBlock(x, y, z, block);
            }
            scene.AddRegion(blockRegion);
        }
        return scene;
    }
}