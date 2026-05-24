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
        MaxX = Math.Max(MaxX, region.PosX + region.SizeX);
        MaxY = Math.Max(MaxY, region.PosY + region.SizeY);
        MaxZ = Math.Max(MaxZ, region.PosZ + region.SizeZ);
        MinX = Math.Min(MinX, region.PosX);
        MinY = Math.Min(MinY, region.PosY);
        MinZ = Math.Min(MinZ, region.PosZ);
    }
}