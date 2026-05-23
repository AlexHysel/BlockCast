namespace BlockCast.Core.Models;

public class BlockScene(string name, string author)
{
    public string Name { get; set; } = name;
    public string Author { get; set; } = author;
    public List<BlockRegion> Regions {get; set; } = [];

    public void AddRegion(BlockRegion region)
    {
        Regions.Add(region);
    }
}