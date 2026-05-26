namespace BlockCast.Core.Models;

public record BlockPos(int X, int Y, int Z)
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;
    public int Z { get; set; } = Z;

    public static BlockPos operator +(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static BlockPos operator -(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}