namespace DiskCopyReader.Tests;

public class DiskCopyImageTests
{
    [Theory]
    [InlineData("MacWrite and Paint 1.0.dsk")]
    [InlineData("MacWrite 4.5.image")]
    public void Ctor_Stream(string diskName)
    {
        var filePath = Path.Combine("Samples", diskName);
        using var stream = File.OpenRead(filePath);
        var image = new DiskCopyImage(stream);
        
        ExportImage(image, Path.Combine("Output", Path.GetFileNameWithoutExtension(diskName)));
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("stream", () => new DiskCopyImage(null!));
    }

    private static void ExportImage(DiskCopyImage image, string path)
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
