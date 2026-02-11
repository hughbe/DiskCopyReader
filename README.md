# DiskCopyReader

A lightweight .NET library for reading Apple Disk Copy 4.2 (.dc42) disk image files. Disk Copy was a utility created by Apple for duplicating floppy disks on classic Macintosh computers.

## Features

- Read Disk Copy 4.2 image files
- Support for multiple disk encodings:
  - GCR 400KB (early Macintosh floppies)
  - GCR 800KB (double-sided Macintosh floppies)
  - MFM 720KB (PC-compatible floppies)
  - MFM 1.44MB (high-density floppies)
- Extract raw disk data and tag data
- Verify data and tag checksums
- Access image metadata (name, encoding, sizes)
- Support for .NET 9.0
- Zero external dependencies

## Installation

Add the project reference to your .NET application:

```sh
dotnet add reference path/to/DiskCopyReader.csproj
```

Or, if published on NuGet:

```sh
dotnet add package DiskCopyReader
```

## Usage

### Reading a Disk Copy Image

```csharp
using DiskCopyReader;

// Open a Disk Copy image file
using var stream = File.OpenRead("disk.image");

// Parse the Disk Copy image
var image = new DiskCopy42Image(stream);

// Get image header information
Console.WriteLine($"Image Name: {image.Header.ImageName}");
Console.WriteLine($"Disk Encoding: {image.Header.Encoding}");
Console.WriteLine($"Data Size: {image.Header.DataSize} bytes");
Console.WriteLine($"Tag Size: {image.Header.TagSize} bytes");
```

### Accessing Disk Data

```csharp
// Get the raw disk data
byte[] diskData = image.ImageData;

// Get the tag data (used for copy protection/verification)
byte[] tagData = image.TagData;

// Write the raw disk data to a file
File.WriteAllBytes("output.dsk", diskData);
```

### Verifying Checksums

```csharp
using DiskCopyReader.Utilities;

// Calculate and verify checksums
uint dataChecksum = DiskCopyChecksum.Calculate(image.ImageData);
uint tagChecksum = DiskCopyChecksum.Calculate(image.TagData);

bool dataValid = dataChecksum == image.Header.DataChecksum;
bool tagValid = tagChecksum == image.Header.TagChecksum;

Console.WriteLine($"Data checksum valid: {dataValid}");
Console.WriteLine($"Tag checksum valid: {tagValid}");
```

## API Overview

### DiskCopy42Image

The main class for reading Disk Copy 4.2 images.

- `DiskCopy42Image(Stream stream)` - Opens a Disk Copy image from a stream
- `Header` - Gets the DC42 header containing image metadata
- `ImageData` - Gets the raw disk data as a byte array
- `TagData` - Gets the tag data as a byte array

### DC42Header

Contains the Disk Copy 4.2 image header metadata:

- `ImageName` - Name of the disk image as a `String63` inline array (up to 63 characters, zero-alloc)
- `ImageNameLength` - Length of the image name
- `DataSize` - Size of the disk data in bytes
- `TagSize` - Size of the tag data in bytes
- `DataChecksum` - Checksum of the disk data
- `TagChecksum` - Checksum of the tag data
- `Encoding` - Encoding type of the disk (GCR400k, GCR800k, MFM720k, MFM1440k)
- `Format` - Format byte
- `MagicNumber` - Magic number for format verification (0x0100)

### String63

A fixed-size `[InlineArray(63)]` struct for storing Disk Copy image names without heap allocation. Implements `ISpanFormattable` and `IEquatable<String63>`, and has an implicit conversion to `string`.

### DiskCopyTimestamp

A lightweight struct representing a MacOS timestamp (seconds since January 1, 1904). Used by `MfsTag` to defer `DateTime` conversion until needed via `ToDateTime()`. Implements `IEquatable`, `IComparable`, and `IFormattable`, and has an implicit conversion to `DateTime`.

### DiskEncoding

Specifies the disk encoding used in the image:

- `GCR400k` - 400KB GCR encoding (early Macintosh)
- `GCR800k` - 800KB GCR encoding (double-sided Macintosh)
- `MFM720k` - 720KB MFM encoding (PC-compatible)
- `MFM1440k` - 1.44MB MFM encoding (high-density)

## Building

Build the project using the .NET SDK:

```sh
dotnet build
```

Run tests:

```sh
dotnet test
```

## DiskCopyDumper CLI

Extract a Disk Copy image to raw disk data using the dumper tool.

### Install/Build

```sh
dotnet build dumper/DiskCopyDumper.csproj -c Release
```

### Usage

```sh
dc-dumper <input> [-o|--output <path>] [--include-tags] [--verify]
```

- `<input>`: Path to the Disk Copy image file.
- `-o|--output`: Destination directory for extracted files (defaults to input filename).
- `--include-tags`: Also extract tag data to a separate file.
- `--verify`: Verify data and tag checksums.

Output files:
- `<ImageName>.dsk` - Raw disk data
- `<ImageName>.tags` - Tag data (if `--include-tags` is specified)

## Benchmarks

Run benchmarks using BenchmarkDotNet:

```sh
dotnet run --project benchmarks/DiskCopyReader.Benchmarks.csproj -c Release
```

Results on Apple M4, .NET 9.0:

| Method                                | Mean          | Allocated |
|-------------------------------------- |--------------:|----------:|
| Read DiskCopy42Image                  |  38,134.21 ns |  820081 B |
| Calculate checksum                    | 204,283.65 ns |       0 B |
| Parse header (String63, zero-alloc)   |      12.24 ns |       0 B |
| Parse header (string, allocating)     |      14.29 ns |      56 B |

## Requirements

- .NET 9.0 or later

## License

MIT License. See [LICENSE](LICENSE) for details.

Copyright (c) 2026 Hugh Bellamy

## About Disk Copy 4.2

Disk Copy was a disk image utility created by Apple Computer for the classic Macintosh operating system. The DC42 format (Disk Copy 4.2) was widely used for distributing software and system disks. Key characteristics:

- 84-byte header containing metadata
- Support for both GCR (Group Coded Recording) and MFM (Modified Frequency Modulation) encodings
- Optional tag data for copy protection and verification
- Checksums for data integrity verification
- Used extensively for Macintosh floppy disk distribution

## Related Projects

- [AppleDiskImageReader](https://github.com/hughbe/AppleDiskImageReader) - Reader for Apple II universal disk images (2IMG)
- [AppleIIDiskReader](https://github.com/hughbe/AppleIIDiskReader) - Reader for Apple II disks
- [ProDosVolumeReader](https://github.com/hughbe/ProDosVolumeReader) - Reader for ProDOS volumes
- [MfsReader](https://github.com/hughbe/MfsReader) - Reader for MFS (Macintosh File System) volumes
- [HfsReader](https://github.com/hughbe/HfsReader) - Reader for HFS (Hierarchical File System) volumes
