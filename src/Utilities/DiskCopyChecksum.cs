using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiskCopyReader.Utilities;

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

        ref byte start = ref MemoryMarshal.GetReference(data);
        int length = data.Length;

        for (int i = 0; i + 1 < length; i += 2)
        {
            uint word = (uint)(Unsafe.Add(ref start, i) << 8 | Unsafe.Add(ref start, i + 1));
            checksum += word;
            checksum = BitOperations.RotateRight(checksum, 1);
        }

        return checksum;
    }
}
