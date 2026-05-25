namespace BlockCast.Core.Models;

public class BlockRegion(string name, int x, int y, int z)
{
    public string Name {get; set; } = name;
    public int MinX { get; set; } = x;
    public int MinY { get; set; } = y;
    public int MinZ { get; set; } = z;
    public int MaxX { get; private set; } = 0;
    public int MaxY { get; private set; } = 0;
    public int MaxZ { get; private set; } = 0;

    public Dictionary<(int, int, int), Block> Blocks { get; private set; } = [];

    public void SetBlocks(Dictionary<(int, int, int), Block> blocks)
    {
        Blocks = blocks;
        MaxX = Blocks.Max(b => b.Key.Item1) + 1;
        MaxY = Blocks.Max(b => b.Key.Item2) + 1;
        MaxZ = Blocks.Max(b => b.Key.Item3) + 1;
        MinX = Blocks.Min(b => b.Key.Item1);
        MinY = Blocks.Min(b => b.Key.Item2);
        MinZ = Blocks.Min(b => b.Key.Item3);
    }

    public void AddBlock(int x, int y, int z, Block block)
    {
        Blocks[(x, y, z)] = block;
        MaxX = Math.Max(MaxX, x + 1);
        MaxY = Math.Max(MaxY, y + 1);
        MaxZ = Math.Max(MaxZ, z + 1);
        MinX = Math.Min(MinX, x);
        MinY = Math.Min(MinY, y);
        MinZ = Math.Min(MinZ, z);
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