using Microsoft.Win32.SafeHandles;
using SpoofFileParser.FileMetadata;
using SpoofFileParser.FileMetadataParser;
using System.Runtime.InteropServices;

namespace SpoofFileParser;

public class FileClassifier : IFileClassifier
{
    private readonly IParserFactory _parserFactory;

    public FileExtension2[] FileExtensions = [
        new(
            1,
            [0x66, 0x74, 0x79, 0x70 ],
            1887007846,
            "mp4",
            4,
            FileType.Video),

        new(
            9,
            [0xFF, 0xD8, 0xFF, 0xDB ],
            3690977535,
            "jpg",
            0,
            FileType.Image),
        new(
            9,
            [0xFF, 0xD8, 0xFF, 0xE0 ],
            3774863615,
            "jpg",
            0,
            FileType.Image),
        new(
            9,
            [0xFF, 0xD8, 0xFF, 0xE1],
            3791640831,
            "jpg",
            0,
            FileType.Image),
        new(
            10,
            [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
            727905341920923785,
            "png",
            0,
            FileType.Image),
        new(
            3,
            [0x57, 0x41, 0x56, 0x45 ],
            1163280727,
            "wav",
            0,
            FileType.Video),
        new(
            8,
            [0x41, 0x56, 0x49, 0x20 ],
            541677121,
            "avi",
            0,
            FileType.Video),
        new(
            11,
            [ 0xFF, 0xFB ],
            64511,
            "mp3",
            0,
            FileType.Audio),
        new(
            0,
            [0x47, 0x49, 0x46, 0x38, 0x37, 0x61 ],
            106889795225927,
            "gif",
            0,
            FileType.Image),
        new(
            0,
            [ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 ],
            106898385160519,
            "gif",
            0,
            FileType.Image),
        new(
            12,
            [ 0x4F, 0x67, 0x67, 0x53 ],
            1399285583,
            "oog",
            0,
            FileType.Audio,
            28,
            [
                new(
                    12,
                    [79, 112, 117, 115, 72, 101, 97, 100],
                    7233173838382854223,
                    "opus",
                    0,
                    FileType.Audio)
            ]),
        new(
            11,
            [ 0xFF, 0xF3 ],
            62463,
            "mp3",
            0,
            FileType.Audio),
        new(
            11,
            [ 0xFF, 0xF2 ],
            62207,
            "mp3",
            0,
            FileType.Audio),
        new(
            11,
            [ 0x49, 0x44, 0x33 ],
            3359817,
            "mp3",
            0,
            FileType.Audio),
        new(
            2,
            [ 0x1A, 0x45, 0xDF, 0xA3 ],
            2749318426,
            "mkv",
            0,
            FileType.Video,
            [0x42, 0x82],
            2,
            [
                new(
                    2,
                    [0x88, 0x6D, 0x61, 0x74, 0x72, 0x6F, 0x73, 0x6B, 0x61],
                    7742654721749511560,
                    "mkv",
                    -1,
                    FileType.Video),
                new(
                    4,
                        [0x83, 0x6D, 0x6B, 0x61],
                        1634430339,
                    "mka",
                    -1,
                    FileType.Video),
                new(
                    6,
                        [0x83, 0x6D, 0x6B, 0x73],
                        1936420227,
                    "mks",
                    -1,
                    FileType.Video),
                new(
                    5,
                        [0x84, 0x6D, 0x6B, 0x33, 0x64],
                        430359408004,
                    "mk3d",
                    -1,
                    FileType.Video),
                new(
                    7,
                        [0x84, 0x77, 0x65, 0x62, 0x6D],
                        469802252164,
                    "webm",
                    -1,
                    FileType.Video),
            ]),
        ];
    public FileClassifier(IParserFactory parserFactory)
    {
        _parserFactory = parserFactory;
        FileExtensions = [.. FileExtensions
            .OrderBy(x => x.Offset)
            .ThenBy(x => x.SubMarkers.Length)
            .ThenByDescending(x => x.SubMarkers.Max(sm => (long?)sm.Offset) ?? 0)];

    }
    public string? GetExtensionName(string filepath)
    {
        using SafeFileHandle handle = File.OpenHandle(
            filepath, FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        return GetExtensionName(handle);
    }

    public short GetExtensionId(string filepath)
    {
        using SafeFileHandle handle = File.OpenHandle(
            filepath, FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        return GetExtensionId(handle);
    }

    public string? GetExtensionName(SafeFileHandle handle)
    {
        FileExtension2 extension2 = GetExtension(handle);
        return extension2 == default
            ? null
            : extension2.Name;
    }

    public short GetExtensionId(SafeFileHandle handle)
    {
        FileExtension2 extension2 = GetExtension(handle);
        return extension2 == default
            ? (short)-1
            : extension2.Id;
    }

    public FileExtension2 GetExtension(string filepath)
    {
        using SafeFileHandle handle = File.OpenHandle(
            filepath, FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        return GetExtension(handle);
    }

    public IFileMetadata? GetFileMetadata(string filePath)
    {
        using SafeFileHandle handle = File.OpenHandle(
            filePath, FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        FileExtension2 extension2 = GetExtension(handle);
        if (extension2 == default)
            return default;

        IFileMetadaParser? fileMetadaParser = _parserFactory.Get(extension2.Type);
        if (fileMetadaParser is null)
            return default;

        return fileMetadaParser.Parse(handle, extension2);
    }

    public FileExtension2 GetExtension(SafeFileHandle handle)
    {
        Span<byte> buffer = stackalloc byte[1024];
        buffer.Clear();
        int bytesRead = RandomAccess.Read(handle, buffer, 0);
        int offset = -1;
        UInt128 mask = 0, subMask = 0;
        FileExtension2 extension;
        ReadOnlySpan<byte> fileSpan = buffer[..bytesRead];

        for (int i = 0; i < FileExtensions.Length; i++)
        {
            extension = FileExtensions[i];
            if (offset != extension.Offset)
            {
                offset = extension.Offset;
                mask = MemoryMarshal.Read<UInt128>(fileSpan.Slice(offset, 16));
            }
            if (fileSpan.Length >= extension.Offset + extension.Magic.Length)
            {
                if ((mask & extension.MagicMask) == extension.MagicMask)
                {
                    if (extension.SubMarkers.Length > 0)
                    {
                        if (extension.SubMarkerOffset.Length > 0)
                        {
                            subMask = MemoryMarshal.Read<UInt128>(
                            fileSpan.Slice(
                                fileSpan.IndexOf(extension.SubMarkerOffset) + extension.SubOffset,
                                16)
                            );
                            foreach (FileExtension2 subExtension2 in extension.SubMarkers)
                                if ((subMask & subExtension2.MagicMask) == subExtension2.MagicMask)
                                    return subExtension2 with { Size = RandomAccess.GetLength(handle) };
                        }
                        else
                        {
                            subMask = MemoryMarshal.Read<UInt128>(
                                fileSpan.Slice(
                                    extension.SubOffset,
                                    16));

                            foreach (FileExtension2 subExtension2 in extension.SubMarkers)
                                if ((subMask & subExtension2.MagicMask) == subExtension2.MagicMask)
                                    return subExtension2 with
                                    {
                                        Size = RandomAccess.GetLength(handle)
                                    };
                        }
                    }
                    else
                        return extension with { Size = RandomAccess.GetLength(handle) };
                }
            }
        }
        return default;
    }
}
