namespace BlockCast.Core.Models;

public class BlockRegion(string name)
{
    public string Name {get; set; } = name;
    public BlockPos Min { get; private set; } = new(int.MaxValue, int.MaxValue, int.MaxValue);
    public BlockPos Max { get; private set; } = new(int.MinValue, int.MinValue, int.MinValue);
    public BlockPos Size => Max - Min;

    public Dictionary<(int, int, int), Block> Blocks { get; private set; } = [];

    public void SetBlocks(Dictionary<(int, int, int), Block> blocks)
    {
        Blocks = blocks;
        Max = new BlockPos(Blocks.Max(b => b.Key.Item1), Blocks.Max(b => b.Key.Item2), Blocks.Max(b => b.Key.Item3));
        Min = new BlockPos(Blocks.Min(b => b.Key.Item1), Blocks.Min(b => b.Key.Item2), Blocks.Min(b => b.Key.Item3));
    }

    public void AddBlock(int x, int y, int z, Block block)
    {
        Blocks[(x, y, z)] = block;
        Max = new BlockPos(Math.Max(Max.X, x), Math.Max(Max.Y, y), Math.Max(Max.Z, z));
        Min = new BlockPos(Math.Min(Min.X, x), Math.Min(Min.Y, y), Math.Min(Min.Z, z));
    }

    public List<Block> GetPalette()
    {
        HashSet<Block> palette = [new Block("minecraft:air"), .. Blocks.Values];
        return [.. palette];
    }
    
    public Block GetBlock(int x, int y, int z)
    {
        return Blocks.TryGetValue((x, y, z), out var block) ? block : new Block("minecraft:air");
    }
}