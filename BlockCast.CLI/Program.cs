using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;

PrintHeader();
var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());

string obj = File.ReadAllText("untitled.obj");

Console.Write("Parsing OBJ... ");
Mesh mesh = OBJParser.Parse(obj);
PrintDone();

Console.Write("Voxelizing... ");
BlockScene scene = voxelizer.Voxelize(mesh);
PrintDone();

Console.Write("Writing Litematica file... ");
LitematicWriter.WriteToFile("t.litematic", scene);
PrintDone();

static void PrintDone()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Done.");
    Console.ForegroundColor = ConsoleColor.White;
}

static void PrintHeader()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("""

    /$$$$$$$  /$$                     /$$        /$$$$$$                        /$$    
    | $$__  $$| $$                    | $$       /$$__  $$                      | $$    
    | $$  \ $$| $$  /$$$$$$   /$$$$$$$| $$   /$$| $$  \__/  /$$$$$$   /$$$$$$$ /$$$$$$  
    | $$$$$$$ | $$ /$$__  $$ /$$_____/| $$  /$$/| $$       |____  $$ /$$_____/|_  $$_/  
    | $$__  $$| $$| $$  \ $$| $$      | $$$$$$/ | $$        /$$$$$$$|  $$$$$$   | $$    
    | $$  \ $$| $$| $$  | $$| $$      | $$_  $$ | $$    $$ /$$__  $$ \____  $$  | $$ /$$
    | $$$$$$$/| $$|  $$$$$$/|  $$$$$$$| $$ \  $$|  $$$$$$/|  $$$$$$$ /$$$$$$$/  |  $$$$/
    |_______/ |__/ \______/  \_______/|__/  \__/ \______/  \_______/|_______/    \___/  

                                                                        by AlexHysel

    """);
    Console.ForegroundColor = ConsoleColor.White;
}
