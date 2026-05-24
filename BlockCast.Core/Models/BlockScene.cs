using System.Numerics;

namespace BlockCast.Core.Models;

public class BlockScene(string name, string author)
{
    public string Name { get; set; } = name;
    public string Author { get; set; } = author;
    public List<BlockRegion> Regions {get; set; } = [];
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public int MaxZ { get; set; }
    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MinZ { get; set; }

    public void AddRegion(BlockRegion region)
    {
        Regions.Add(region);
        MaxX = Math.Max(MaxX, region.X + region.Blocks.Max(b => b.Key.Item1) + 1);
        MaxY = Math.Max(MaxY, region.Y + region.Blocks.Max(b => b.Key.Item2) + 1);
        MaxZ = Math.Max(MaxZ, region.Z + region.Blocks.Max(b => b.Key.Item3) + 1);
        MinX = Math.Min(MinX, region.X + region.Blocks.Min(b => b.Key.Item1));
        MinY = Math.Min(MinY, region.Y + region.Blocks.Min(b => b.Key.Item2));
        MinZ = Math.Min(MinZ, region.Z + region.Blocks.Min(b => b.Key.Item3));
    }
}