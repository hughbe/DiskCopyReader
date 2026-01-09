using System.Buffers.Binary;

namespace HfsReader.Utilities;

/// <summary>
/// Provides utility methods for reading data from spans.
/// </summary>
internal static class SpanUtilities
{
    /// <summary>
    /// Reads a MacOS timestamp from the specified span and converts it to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="data">The span containing the data.</param>
    /// <returns>The corresponding <see cref="DateTime"/> value.</returns>
    public static DateTime ReadMacOSTimestamp(ReadOnlySpan<byte> data)
    {
        if (data.Length < 4)
        {
            throw new ArgumentException("Data span must contain at least 4 bytes for the timestamp.", nameof(data));
        }

        // 4 bytes MacOS timestamp
        var timestamp = BinaryPrimitives.ReadUInt32BigEndian(data);

        // MacOS timestamps are seconds since 00:00:00 on January 1, 1904
        var epoch = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(timestamp);
    }
}
