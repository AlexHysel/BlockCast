using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;

var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());

string obj = File.ReadAllText("untitled.obj");
Mesh mesh = OBJParser.Parse(obj);
BlockScene scene = voxelizer.Voxelize(mesh);
LitematicaWriter.WriteToFile("t.litematic", scene);