namespace BlockCast.Core.Models;

public readonly record struct BlockPos(int X, int Y, int Z)
{

    public static BlockPos operator +(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static BlockPos operator -(BlockPos a, BlockPos b)
    {
        return new BlockPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
}