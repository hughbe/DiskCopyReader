using System.Buffers.Binary;
using System.Diagnostics;
using HfsReader.Utilities;

namespace DiskCopyReader;

/// <summary>
/// Represents an MFS (Macintosh File System) tag associated with a file in a Disk Copy image.
/// </summary>
public readonly struct MfsTag
{
    /// <summary>
    /// Size of the MFS tag in bytes.
    /// </summary>
    public const int Size = 12;

    /// <summary>
    /// Gets the file number on disk within the MFS file system.
    /// </summary>
    public uint FileNumber { get; }

    /// <summary>
    /// Gets the flags associated with the file.
    /// </summary>
    public ushort Flags { get; }

    /// <summary>
    /// Gets the logical block number where the file's data starts.
    /// </summary>
    public ushort LogicalBlockNumber { get; }

    /// <summary>
    /// Gets the last modification date of the file.
    /// </summary>
    public DateTime LastModificationDate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MfsTag"/> struct.
    /// </summary>
    /// <param name="data">The span containing the MFS tag data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public MfsTag(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException("Invalid MFS tag data length.", nameof(data));
        }

        int offset = 0;

        FileNumber = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        Flags = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        LogicalBlockNumber = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        LastModificationDate = SpanUtilities.ReadMacOSTimestamp(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all bytes of MFS tag.");
    }
}
