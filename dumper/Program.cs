using Spectre.Console;
using Spectre.Console.Cli;
using DiskCopyReader;

public sealed class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp<ExtractCommand>();
        app.Configure(config =>
        {
            config.SetApplicationName("dc-dumper");
            config.ValidateExamples();
        });

        return app.Run(args);
    }
}

sealed class ExtractSettings : CommandSettings
{
    [CommandArgument(0, "<input>")]
    public required string Input { get; init; }

    [CommandOption("-o|--output")]
    public string? Output { get; init; }

    [CommandOption("--include-tags")]
    public bool IncludeTags { get; init; }

    [CommandOption("--verify")]
    public bool Verify { get; init; }
}

sealed class ExtractCommand : AsyncCommand<ExtractSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ExtractSettings settings, CancellationToken cancellationToken)
    {
        var input = new FileInfo(settings.Input);
        if (!input.Exists)
        {
            AnsiConsole.MarkupLine($"[red]Input file not found[/]: {input.FullName}");
            return -1;
        }

        await using var stream = input.OpenRead();
        DiskCopyImage image;
        try
        {
            image = new DiskCopyImage(stream);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to read Disk Copy image[/]: {ex.Message}");
            return -1;
        }

        // Display header information
        AnsiConsole.MarkupLine($"[green]Image Name[/]: {image.Header.ImageName}");
        AnsiConsole.MarkupLine($"[green]Disk Encoding[/]: {image.Header.DiskEncoding}");
        AnsiConsole.MarkupLine($"[green]Data Size[/]: {image.Header.DataSize} bytes");
        AnsiConsole.MarkupLine($"[green]Tag Size[/]: {image.Header.TagSize} bytes");

        // Verify checksums if requested
        if (settings.Verify)
        {
            var dataChecksum = DiskCopyChecksum.Calculate(image.ImageData);
            var tagChecksum = DiskCopyChecksum.Calculate(image.TagData);

            if (dataChecksum == image.Header.DataChecksum)
            {
                AnsiConsole.MarkupLine($"[green]Data checksum valid[/]: 0x{dataChecksum:X8}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Data checksum mismatch[/]: expected 0x{image.Header.DataChecksum:X8}, got 0x{dataChecksum:X8}");
            }

            if (tagChecksum == image.Header.TagChecksum)
            {
                AnsiConsole.MarkupLine($"[green]Tag checksum valid[/]: 0x{tagChecksum:X8}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Tag checksum mismatch[/]: expected 0x{image.Header.TagChecksum:X8}, got 0x{tagChecksum:X8}");
            }
        }

        var outputPath = settings.Output ?? Path.GetFileNameWithoutExtension(input.Name);
        var outputDir = new DirectoryInfo(outputPath);
        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        var safeName = SanitizeName(image.Header.ImageName);

        // Write data
        var dataPath = Path.Combine(outputDir.FullName, safeName + ".dsk");
        await File.WriteAllBytesAsync(dataPath, image.ImageData, cancellationToken);
        AnsiConsole.MarkupLine($"Wrote data: {Path.GetFileName(dataPath)} ({image.ImageData.Length} bytes)");

        // Write tags if requested and present
        if (settings.IncludeTags && image.TagData.Length > 0)
        {
            var tagPath = Path.Combine(outputDir.FullName, safeName + ".tags");
            await File.WriteAllBytesAsync(tagPath, image.TagData, cancellationToken);
            AnsiConsole.MarkupLine($"Wrote tags: {Path.GetFileName(tagPath)} ({image.TagData.Length} bytes)");
        }

        AnsiConsole.MarkupLine($"[green]Extraction complete[/]: {outputDir.FullName}");
        return 0;
    }

    private static string SanitizeName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            name = name.Replace(invalidChar, '_');
        }

        return name;
    }
}
