namespace BlockCast.Core.Models;

public class BlockRegion(string name)
{
    public string Name {get; set; } = name;
    public BlockPos Min { get; private set; } = new(int.MaxValue, int.MaxValue, int.MaxValue);
    public BlockPos Max { get; private set; } = new(int.MinValue, int.MinValue, int.MinValue);
    public BlockPos Size => Max - Min + new BlockPos(1, 1, 1);

    public Dictionary<BlockPos, Block> Blocks { get; private set; } = [];

    public void SetBlocks(Dictionary<BlockPos, Block> blocks)
    {
        Blocks = blocks;
        Max = new BlockPos(Blocks.Max(b => b.Key.X), Blocks.Max(b => b.Key.Y), Blocks.Max(b => b.Key.Z));
        Min = new BlockPos(Blocks.Min(b => b.Key.X), Blocks.Min(b => b.Key.Y), Blocks.Min(b => b.Key.Z));
    }

    public void AddBlock(int x, int y, int z, Block block)
    {
        Blocks[new BlockPos(x, y, z)] = block;
        Max = new BlockPos(Math.Max(Max.X, x), Math.Max(Max.Y, y), Math.Max(Max.Z, z));
        Min = new BlockPos(Math.Min(Min.X, x), Math.Min(Min.Y, y), Math.Min(Min.Z, z));
    }

    public List<Block> GetPalette()
    {
        return [new Block("minecraft:air"), .. Blocks.Values];
    }
    
    public Block GetBlock(int x, int y, int z)
    {
        return Blocks.TryGetValue(new BlockPos(x, y, z), out var block) ? block : new Block("minecraft:air");
    }

    public Block GetBlock(BlockPos pos)
    {
        return Blocks.TryGetValue(pos, out var Block) ? Block : new Block("minecraft:air");
    }
}