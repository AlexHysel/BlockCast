using System.Diagnostics;
using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;

var stopwatch= Stopwatch.StartNew();
PrintHeader();

var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());

string obj = File.ReadAllText("untitled.obj");

var progress = new Progress<float>(p => Console.Write($"\rParsing OBJ...\t{p:F1}%"));
Mesh mesh = OBJParser.Parse(obj, progress);
Console.WriteLine($"\tTime: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Restart();

progress = new Progress<float>(p => Console.Write($"\rVoxelizing...\t{p:F1}%"));
BlockScene scene = voxelizer.Voxelize(mesh, progress);
Console.WriteLine($"\tTime: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Restart();

progress = new Progress<float>(p => Console.Write($"\rWriting .litematic...\t{p:F1}%"));
LitematicWriter.WriteToFile("t.litematic", scene, progress);
Console.WriteLine($"\tTime: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Stop();

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
