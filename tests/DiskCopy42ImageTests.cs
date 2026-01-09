using System.Diagnostics;

namespace DiskCopyReader.Tests;

public class DiskCopy42ImageTests
{
    [Theory]
    [InlineData("MacWrite and Paint 1.0.dsk")]
    [InlineData("MacWrite 4.5.image")]
    [InlineData("LisaOS2/Lisa Office System 2.0 1.image")]
    [InlineData("LisaOS2/Lisa Office System 2.0 2.image")]
    [InlineData("LisaOS2/Lisa Office System 2.0 3.image")]
    [InlineData("LisaOS2/Lisa Office System 2.0 4.image")]
    [InlineData("LisaOS2/Lisa Office System 2.0 5.image")]
    [InlineData("BLU090_400k.dc42")]
    public void Ctor_Stream(string diskName)
    {
        var filePath = Path.Combine("Samples", diskName);
        using var stream = File.OpenRead(filePath);
        var image = new DiskCopy42Image(stream);
        DumpImage(image);
        ExportImage(image, Path.Combine("Output", Path.GetFileNameWithoutExtension(diskName)));
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("stream", () => new DiskCopy42Image(null!));
    }

    private static void DumpImage(DiskCopy42Image image)
    {
        Debug.WriteLine($"Image Name: {image.Header.ImageName}");
        Debug.WriteLine($"Disk Encoding: {image.Header.Encoding}");
        Debug.WriteLine($"Disk Format: {image.Header.Format}");
        Debug.WriteLine($"Data Size: {image.Header.DataSize} bytes");
        Debug.WriteLine($"Tag Size: {image.Header.TagSize} bytes");
        Debug.WriteLine("");
    }

    private static void ExportImage(DiskCopy42Image image, string path)
    {
        // Ensure the output directory exists
        Directory.CreateDirectory(path);

        // Sanitize file names for filesystem compatibility
        var safeName = SanitizeName(image.Header.ImageName) + ".dsk";
        var filePath = Path.Combine(path, safeName);
        File.WriteAllBytes(filePath, image.ImageData);
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
