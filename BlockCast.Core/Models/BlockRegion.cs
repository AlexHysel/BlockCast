namespace BlockCast.Core.Models;

public class BlockRegion(string name, int x, int y, int z)
{
    public string Name {get; set; } = name;
    public int PosX { get; set; } = x;
    public int PosY { get; set; } = y;
    public int PosZ { get; set; } = z;
    public int SizeX { get; private set; } = 0;
    public int SizeY { get; private set; } = 0;
    public int SizeZ { get; private set; } = 0;

    public Dictionary<(int, int, int), Block> Blocks { get; private set; } = [];

    public void SetBlocks(Dictionary<(int, int, int), Block> blocks)
    {
        Blocks = blocks;
        SizeX = Blocks.Max(b => b.Key.Item1) + 1;
        SizeY = Blocks.Max(b => b.Key.Item2) + 1;
        SizeZ = Blocks.Max(b => b.Key.Item3) + 1;
    }

    public void AddBlock(int x, int y, int z, Block block)
    {
        Blocks[(x, y, z)] = block;
        SizeX = Math.Max(SizeX, x + 1);
        SizeY = Math.Max(SizeY, y + 1);
        SizeZ = Math.Max(SizeZ, z + 1);
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