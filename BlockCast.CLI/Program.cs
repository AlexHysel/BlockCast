using BlockCast.Core.Formats.Litematica;
using BlockCast.Core.Models;
using BlockCast.Core.Parsers;
using BlockCast.Core.Voxelization;
using Spectre.Console;
using System.CommandLine;


var inputArg = new Argument<FileInfo>("input .obj");
var outputArg = new Argument<FileInfo>("output");

var rootCommand = new RootCommand
{
    inputArg,
    outputArg
};

rootCommand.SetAction(parseResult =>
{
    var input = parseResult.GetValue(inputArg);
    var output = parseResult.GetValue(outputArg);

    PrintHeader();
    var voxelizer = new RayTracingVoxelizer(new VoxelizerOptions());
    string obj = File.ReadAllText(input.FullName);
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
        LitematicWriter.WriteToFile(output.FullName, scene, progress);
    });

    return 0;
});

await rootCommand.Parse(args).InvokeAsync();

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
