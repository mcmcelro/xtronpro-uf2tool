using System.Diagnostics;

using xtronpro_uf2tool.Utils;

// require a path to a source file as an argument
if (args.Length < 1)
{
    Console.Error.WriteLine("Please provide the path to a source file as an argument.");
    return;
}

if (Directory.Exists(args[0]))
{
    Console.WriteLine($"Running in batch mode for folder {args[0]}.");
    Directory.SetCurrentDirectory(args[0]);
    string[] files = Directory.GetFiles(".", "*.nes");
    Directory.CreateDirectory("./conv");
    foreach (string file in files)
    {
        Console.WriteLine($"Converting file {file}...");
        try
        {
            string targetFile = $"./conv/{file}";
            new XtronConverter().ConvertFile(file, targetFile);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while converting {file}: {ex.Message}");
        }
    }
}
else
{
    string sourceFile = args[0];
    string targetFile = $"{Path.GetFileNameWithoutExtension(sourceFile)}-conv.nes";
    new XtronConverter().ConvertFile(sourceFile, targetFile);
}


