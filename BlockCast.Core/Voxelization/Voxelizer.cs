using BlockCast.Core.Models;

namespace BlockCast.Core.Voxelization;

public abstract class Voxelizer(VoxelizerOptions options)
{
    protected VoxelizerOptions Options { get; set; } = options;

    public abstract BlockScene Voxelize(Mesh mesh);
}