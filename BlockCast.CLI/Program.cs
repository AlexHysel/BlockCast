using System.Diagnostics;
using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;
using Spectre.Console;

var stopwatch= Stopwatch.StartNew();
PrintHeader();

var voxelizer = new DominantAxisCpuVoxelizer(new VoxelizerOptions());
string obj = File.ReadAllText("untitled.obj");
AnsiConsole.Progress().Start(ctx =>
{
    var parseTask = ctx.AddTask("Parsing OBJ", maxValue: 100);
    var voxelTask = ctx.AddTask("Voxelizing", maxValue: 100);
    var writeTask = ctx.AddTask("Writing .litematic", maxValue: 100);

    var progress = new Progress<float>(p => parseTask.Value = p);
    MeshScene meshScene = OBJParser.Parse(obj, progress);

    progress = new Progress<float>(p => voxelTask.Value = p);
    BlockScene scene = voxelizer.Voxelize(meshScene, progress);

    progress = new Progress<float>(p => writeTask.Value = p);
    LitematicWriter.WriteToFile("t.litematic", scene, progress);
});

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
