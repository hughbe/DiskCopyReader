namespace DiskCopyReader;

/// <summary>
/// Specifies the disk format.
/// </summary>
public enum DiskFormat : byte
{
    /// <summary>
    /// Mac OS X disk format.
    /// </summary>
    MacOSX = 0x00,

    /// <summary>
    /// Twiggy disk format.
    /// </summary>
    Twiggy = 0x01,

    /// <summary>
    /// 400k Macintosh disk format.
    /// </summary>
    Mac400k = 0x02,

    /// <summary>
    /// 800k Macintosh disk format.
    /// </summary>
    Lisa400k = 0x12,

    /// <summary>
    /// 800k Macintosh disk format.
    /// </summary>
    Mac800k = 0x22,

    /// <summary>
    /// 800k Pro Dos disk format.
    /// </summary>
    ProDos800k = 0x24,

    /// <summary>
    /// Non-standard disk format.
    /// </summary>
    NonStandard = 0x93,

    /// <summary>
    /// Invalid or unknown disk format.
    /// </summary>
    Invalid = 0x96
}

