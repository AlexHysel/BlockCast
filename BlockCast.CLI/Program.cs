using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;

Block stone = new("minecraft:stone");
Block dirt = new("minecraft:dirt");
Block acaciaFence = new("minecraft:acacia_fence");

Block[,,] blocks = new Block[5,5,5];
for (int x = 0; x < 5; x++)
    for (int y = 0; y < 5; y++)
        for (int z = 0; z < 5; z++)
        {
            if ((x + y + z) % 3 == 2)
                blocks[x,y,z] = stone;
            else if ((x + y + z) % 3 == 1)
                blocks[x,y,z] = acaciaFence;
            else
                blocks[x,y,z] = dirt;
        }
            

BlockRegion region = new("Main", 0, 0, 0, blocks);
BlockScene scene = new("Scene", "Somebody");
scene.AddRegion(region);

LitematicaWriter.WriteToFile("t.litematic", scene);

string obj = File.ReadAllText("untitled.obj");
Mesh mesh = OBJParser.Parse(obj);

Console.WriteLine("Verices:");
foreach(var v in mesh.Vertices)
    Console.WriteLine($"{v.X}, {v.Y}, {v.Z}");

Console.WriteLine("Normals:");
foreach(var n in mesh.Normals)
    Console.WriteLine($"{n.A}, {n.B}, {n.C}");