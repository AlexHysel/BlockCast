namespace BlockCast.Core.Models;

public class BlockRegion(string name, int x, int y, int z, Block[,,] blocks)
{
    public string Name {get; set; } = name;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Z { get; set; } = z;
    public Block[,,] Blocks { get; set; } = blocks;
    public List<Block> GetPalette()
    {
        var set = new HashSet<Block>();
        foreach (var block in Blocks)
            set.Add(block);

        var palette = new List<Block> { new("minecraft:air") };
        palette.AddRange(set.Where(b => b != new Block("minecraft:air")));
        return palette;
    }
}