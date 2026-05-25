namespace BlockCast.Core.Models;

public record BlockPos(int x, int y, int z)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Z { get; set; } = z;

    public static BlockPos operator +(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static BlockPos operator -(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}