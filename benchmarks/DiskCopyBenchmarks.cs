using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DiskCopyReader;
using DiskCopyReader.Utilities;

namespace DiskCopyReader.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class DiskCopyBenchmarks
{
    private byte[] _fileBytes = null!;
    private byte[] _imageData = null!;
    private byte[] _headerBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Use an 800K GCR disk (819,200 bytes of image data) for benchmarking.
        _fileBytes = File.ReadAllBytes(Path.Combine("Samples", "DART 1.5.3.image"));

        // Pre-parse once to extract the image data for isolated checksum benchmarks.
        using var stream = new MemoryStream(_fileBytes);
        var image = new DiskCopy42Image(stream);
        _imageData = image.ImageData;

        // Extract the raw 84-byte header for isolated header parse benchmarks.
        _headerBytes = _fileBytes[..DC42Header.Size];
    }

    // ── Full image read ─────────────────────────────────────────────────

    [Benchmark(Description = "Read DiskCopy42Image")]
    public DiskCopy42Image ReadImage()
    {
        using var stream = new MemoryStream(_fileBytes);
        return new DiskCopy42Image(stream);
    }

    // ── Checksum ────────────────────────────────────────────────────────

    [Benchmark(Description = "Calculate checksum")]
    public uint Checksum() => DiskCopyChecksum.Calculate(_imageData);

    // ── Header parse: String63 (current) vs string (old) ────────────────

    [Benchmark(Baseline = true, Description = "Parse header (String63, zero-alloc)")]
    public DC42Header ParseHeader() => new DC42Header(_headerBytes);

    [Benchmark(Description = "Parse header (string, allocating)")]
    public OldDC42Header ParseHeaderOld() => new OldDC42Header(_headerBytes);
}

/// <summary>
/// Snapshot of the original DC42Header that allocated a string for ImageName.
/// Used only as a benchmark comparison baseline.
/// </summary>
public readonly struct OldDC42Header
{
    public const int Size = 84;

    public byte ImageNameLength { get; }
    public string ImageName { get; }
    public uint DataSize { get; }
    public uint TagSize { get; }
    public uint DataChecksum { get; }
    public uint TagChecksum { get; }
    public DiskEncoding Encoding { get; }
    public DiskFormat Format { get; }
    public ushort MagicNumber { get; }

    public OldDC42Header(ReadOnlySpan<byte> data)
    {
        int offset = 0;

        ImageNameLength = data[offset];
        offset += 1;

        ImageName = System.Text.Encoding.ASCII.GetString(data.Slice(offset, ImageNameLength));
        offset += 63;

        DataSize = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        TagSize = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        DataChecksum = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        TagChecksum = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        Encoding = (DiskEncoding)data[offset];
        offset += 1;

        Format = (DiskFormat)data[offset];
        offset += 1;

        MagicNumber = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
    }
}
