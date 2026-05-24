namespace BlockCast.Core.Models;

public class BlockRegion(string name, int x, int y, int z)
{
    public string Name {get; set; } = name;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Z { get; set; } = z;
    public Dictionary<(int, int, int), Block> Blocks { get; set; } = [];
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