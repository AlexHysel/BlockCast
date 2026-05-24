using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;

var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());

string obj = File.ReadAllText("untitled.obj");

Console.WriteLine("Parsing OBJ...");
Mesh mesh = OBJParser.Parse(obj);
Console.WriteLine($"Faces: {mesh.Faces.Count}");
Console.WriteLine($"Size: {mesh.Size}");

Console.WriteLine("Voxelizing...");
BlockScene scene = voxelizer.Voxelize(mesh);

Console.WriteLine("Writing Litematica file...");
LitematicaWriter.WriteToFile("t.litematic", scene);