using Microsoft.Win32.SafeHandles;
using SpoofFileParser.FileMetadata;
using SpoofFileParser.FileMetadataParser;
using System.Runtime.InteropServices;

namespace SpoofFileParser;

public class FileClassifier : IFileClassifier
{
    private readonly IParserFactory _parserFactory;

    public ExtensionRoadMap[] FileExtensions;
    public FileClassifier(IParserFactory parserFactory, ExtensionRoadMap[] fileExtension2s)
    {
        FileExtensions = fileExtension2s;
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
        FileExtension extension2 = GetExtension(handle);
        return extension2 == default
            ? null
            : extension2.Name;
    }

    public short GetExtensionId(SafeFileHandle handle)
    {
        FileExtension extension2 = GetExtension(handle);
        return extension2 == default
            ? (short)-1
            : extension2.Id;
    }

    public FileExtension GetExtension(string filepath)
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
        FileExtension extension2 = GetExtension(handle);
        if (extension2 == default)
            return default;

        IFileMetadaParser? fileMetadaParser = _parserFactory.Get(extension2.Type);
        if (fileMetadaParser is null)
            return default;

        return fileMetadaParser.Parse(handle, extension2);
    }

    public FileExtension GetExtension(SafeFileHandle handle)
    {
        Span<byte> buffer = stackalloc byte[1024];
        buffer.Clear();
        int bytesRead = RandomAccess.Read(handle, buffer, 0);
        int offset = -1;
        UInt128 mask = 0, subMask = 0;
        ExtensionRoadMap extension;
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
                            foreach (ExtensionRoadMap subExtension2 in extension.SubMarkers)
                                if ((subMask & subExtension2.MagicMask) == subExtension2.MagicMask)
                                    return new(
                                        subExtension2.Id, 
                                        subExtension2.Name, 
                                        RandomAccess.GetLength(handle), 
                                        subExtension2.Type);
                        }
                        else
                        {
                            subMask = MemoryMarshal.Read<UInt128>(
                                fileSpan.Slice(
                                    extension.SubOffset,
                                    16));

                            foreach (ExtensionRoadMap subExtension2 in extension.SubMarkers)
                                if ((subMask & subExtension2.MagicMask) == subExtension2.MagicMask)
                                    return new(
                                        subExtension2.Id,
                                        subExtension2.Name,
                                        RandomAccess.GetLength(handle),
                                        subExtension2.Type);
                        }
                    }
                    else
                        return new(
                                extension.Id,
                                extension.Name,
                                RandomAccess.GetLength(handle),
                                extension.Type);
                }
            }
        }
        return default;
    }
}
