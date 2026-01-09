using System.Buffers.Binary;

namespace DiskCopyReader;

/// <summary>
/// Provides methods to calculate Disk Copy checksums.
/// </summary>
public static class DiskCopyChecksum
{
    /// <summary>
    /// Calculates the Disk Copy checksum for the given data.
    /// </summary>
    /// <param name="data">The data to calculate the checksum for.</param>
    /// <returns>The calculated checksum.</returns>
    public static uint Calculate(ReadOnlySpan<byte> data)
    {
        uint checksum = 0;

        for (int i = 0; i + 1 < data.Length; i += 2)
        {
            ushort word = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(i, 2));
            checksum += word;
            checksum = ((checksum >> 1) & ~(1 << 31)) | ((checksum & 1) << 31); // Rotate right by 1 bit
        }

        return checksum;
    }
}
