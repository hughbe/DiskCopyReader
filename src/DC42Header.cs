using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace DiskCopyReader;

/// <summary>
/// Represents the header of a Disk Copy 4.2 image.
/// </summary>
public readonly struct DC42Header
{
    /// <summary>
    /// Size of the DC42 header in bytes.
    /// </summary>
    public const int Size = 84;

    /// <summary>
    /// Initializes a new instance of the <see cref="DC42Header"/> struct.
    /// </summary>
    public byte ImageNameLength { get; }

    /// <summary>
    /// Gets the image name.
    /// </summary>
    public string ImageName { get; }

    /// <summary>
    /// Gets the data size in bytes.
    /// </summary>
    public uint DataSize { get; }

    /// <summary>
    /// Gets the tag size in bytes.
    /// </summary>
    public uint TagSize { get; }

    /// <summary>
    /// Gets the data checksum.
    /// </summary>
    public uint DataChecksum { get; }

    /// <summary>
    /// Gets the tag checksum.
    /// </summary>
    public uint TagChecksum { get; }

    /// <summary>
    /// Gets the disk encoding.
    /// </summary>
    public DiskEncoding Encoding { get; }

    /// <summary>
    /// Gets the disk format.
    /// </summary>
    public DiskFormat Format { get; }

    /// <summary>
    /// Gets the magic number.
    /// </summary>
    public ushort MagicNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DC42Header"/> struct from the given data.
    /// </summary>
    /// <param name="data">The header data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is incorrect or the magic number is invalid.</exception>
    public DC42Header(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes in length.", nameof(data));
        }

        int offset = 0;

        // Length of image name string ('Pascal name length')
        ImageNameLength = data[offset];
        offset += 1;

        if (ImageNameLength > 63)
        {
            throw new ArgumentException("Invalid DC42 image: image name length exceeds maximum.", nameof(data));
        }

        // Image name, in ascii, padded with NULs
        ImageName = System.Text.Encoding.ASCII.GetString(data.Slice(offset, ImageNameLength));
        offset += 63;

        // Data size in bytes (of block starting at 0x54)
        DataSize = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // Tag size in bytes (of block starting after end of Data block)
        TagSize = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // Data Checksum
        DataChecksum = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // Tag Checksum
        TagChecksum = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // Disk encoding
        Encoding = (DiskEncoding)data[offset];
        offset += 1;

        if (!Enum.IsDefined(Encoding))
        {
            throw new ArgumentException($"Invalid DC42 image: unknown disk encoding 0x{(byte)Encoding:X2}.", nameof(data));
        }

        // Format Byte
        Format = (DiskFormat)data[offset];
        offset += 1;

        if (!Enum.IsDefined(Format))
        {
            throw new ArgumentException($"Invalid DC42 image: unknown disk format 0x{(byte)Format:X2}.", nameof(data));
        }

        // '0x01 0x00' ('Private Word') AKA Magic Number
        MagicNumber = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        if (MagicNumber != 0x0100)
        {
            throw new ArgumentException("Invalid DC42 image: incorrect magic number.", nameof(data));
        }

        Debug.Assert(offset == data.Length, "Did not consume all header data.");
    }
}
