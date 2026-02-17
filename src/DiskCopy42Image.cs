namespace DiskCopyReader;

/// <summary>
/// Represents a Disk Copy 4.2 image.
/// </summary>
public class DiskCopy42Image
{
    /// <summary>
    /// Gets the Disk Copy image header.
    /// </summary>
    public DC42Header Header { get; }

    /// <summary>
    /// Gets the Disk Copy image data.
    /// </summary>
    public byte[] ImageData { get; }

    /// <summary>
    /// Gets the Disk Copy image tag data.
    /// </summary>
    public byte[] TagData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiskCopy42Image"/> class.
    /// </summary>
    /// <param name="stream">The stream containing the Disk Copy image data.</param>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception> 
    /// <exception cref="ArgumentException">Thrown when the stream is not seekable or readable.</exception>
    public DiskCopy42Image(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek || !stream.CanRead)
        {
            throw new ArgumentException("Stream must be seekable and readable.", nameof(stream));
        }
        
        // Length of image name string ('Pascal name length')
        Span<byte> data = stackalloc byte[DC42Header.Size];
        stream.ReadExactly(data);

        Header = new DC42Header(data);

        var expectedDataSize = Header.Encoding switch
        {
            DiskEncoding.GCR400k => 409_600,
            DiskEncoding.GCR800k => 819_200,
            DiskEncoding.MFM720k => 737_280,
            DiskEncoding.MFM1440k => 1_474_560,
            _ => throw new NotSupportedException($"Disk encoding '{Header.Encoding}' is not supported.")
        };
        if (Header.DataSize % 2 != 0 || Header.DataSize != expectedDataSize)
        {
            throw new ArgumentException("Disk Copy image has an invalid data size.", nameof(stream));
        }

        ImageData = new byte[Header.DataSize];
        stream.ReadExactly(ImageData);

        TagData = new byte[Header.TagSize];
        stream.ReadExactly(TagData);
    }
}
