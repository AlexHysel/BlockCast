namespace BlockCast.Core.Models;

public class BlockScene(string name, string author)
{
    public string Name { get; set; } = name;
    public string Author { get; set; } = author;
    public List<BlockRegion> Regions {get; set; } = [];
    public BlockPos Min {get; private set;} = new(int.MaxValue, int.MaxValue, int.MaxValue);
    public BlockPos Max {get; private set;} = new(int.MinValue, int.MinValue, int.MinValue);
    public BlockPos Size => Max - Min;

    public void AddRegion(BlockRegion region)
    {
        Regions.Add(region);
        Max = new BlockPos(Math.Max(Max.X, region.Max.X), Math.Max(Max.Y, region.Max.Y), Math.Max(Max.Z, region.Max.Z));
        Min = new BlockPos(Math.Min(Min.X, region.Min.X), Math.Min(Min.Y, region.Min.Y), Math.Min(Min.Z, region.Min.Z));
    }
}