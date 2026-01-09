namespace DiskCopyReader;

/// <summary>
/// Specifies the disk encoding used in a Disk Copy image.
/// </summary>
public enum DiskEncoding : byte
{
    /// <summary>
    /// GCR 400KB encoding.
    /// </summary>
    GCR400k = 0x00,

    /// <summary>
    /// GCR 800KB encoding.
    /// </summary>
    GCR800k = 0x01,

    /// <summary>
    /// MFM 720KB encoding.
    /// </summary>
    MFM720k = 0x02,

    /// <summary>
    /// MFM 1.44MB encoding.
    /// </summary>
    MFM1440k = 0x03,
}
