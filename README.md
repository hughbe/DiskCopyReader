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
var image = new DiskCopyImage(stream);

// Get image header information
Console.WriteLine($"Image Name: {image.Header.ImageName}");
Console.WriteLine($"Disk Encoding: {image.Header.DiskEncoding}");
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

### DiskCopyImage

The main class for reading Disk Copy 4.2 images.

- `DiskCopyImage(Stream stream)` - Opens a Disk Copy image from a stream
- `Header` - Gets the DC42 header containing image metadata
- `ImageData` - Gets the raw disk data as a byte array
- `TagData` - Gets the tag data as a byte array

### DC42Header

Contains the Disk Copy 4.2 image header metadata:

- `ImageName` - Name of the disk image (up to 63 characters)
- `ImageNameLength` - Length of the image name
- `DataSize` - Size of the disk data in bytes
- `TagSize` - Size of the tag data in bytes
- `DataChecksum` - Checksum of the disk data
- `TagChecksum` - Checksum of the tag data
- `DiskEncoding` - Encoding type of the disk (GCR400k, GCR800k, MFM720k, MFM1440k)
- `Format` - Format byte
- `MagicNumber` - Magic number for format verification (0x0100)

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

## Requirements

- .NET 9.0 or later

## License

MIT License. See [LICENSE](LICENSE) for details.

Copyright (c) 2025 Hugh Bellamy

## About Disk Copy 4.2

Disk Copy was a disk image utility created by Apple Computer for the classic Macintosh operating system. The DC42 format (Disk Copy 4.2) was widely used for distributing software and system disks. Key characteristics:

- 84-byte header containing metadata
- Support for both GCR (Group Coded Recording) and MFM (Modified Frequency Modulation) encodings
- Optional tag data for copy protection and verification
- Checksums for data integrity verification
- Used extensively for Macintosh floppy disk distribution

## Related Projects

- [MfsReader](https://github.com/hughbe/MfsReader) - Reader for MFS (Macintosh File System) volumes
- [HfsReader](https://github.com/hughbe/HfsReader) - Reader for HFS (Hierarchical File System) volumes
