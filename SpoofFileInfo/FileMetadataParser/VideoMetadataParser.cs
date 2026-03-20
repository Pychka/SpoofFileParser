using Microsoft.Win32.SafeHandles;
using SpoofFileParser.FileMetadata;
using SpoofFileParser.RoadMaps;
using System.Buffers.Binary;
using System.Collections.Concurrent;

namespace SpoofFileParser.FileMetadataParser;

public class VideoMetadataParser(ConcurrentDictionary<string, ImageRoadMap> roadMaps) : IFileMetadaParser
{
    private readonly ConcurrentDictionary<string, ImageRoadMap> _roadMaps = roadMaps;

    public bool CanParse(FileType type) =>
        type == FileType.Image;

    public IFileMetadata? Parse(SafeFileHandle handle, FileExtension2 extension2)
    {
        if (_roadMaps.TryGetValue(extension2.Name, out ImageRoadMap roadMap))
        {
            Span<byte> width = stackalloc byte[roadMap.WitdhtSize];
            Span<byte> height = stackalloc byte[roadMap.HeightSize];
            RandomAccess.Read(handle, width, roadMap.WitdhtOffset);
            RandomAccess.Read(handle, height, roadMap.HeightOffset);
            ImageMetadata metadata = new()
            {
                Extension = extension2.Name,
                FileType = extension2.Type,
                Height = GetULong(height, roadMap.IsBigEndian),
                Width = GetULong(width, roadMap.IsBigEndian),
            };
            return metadata;
        }
        return default;
    }

    private static ulong GetULong(Span<byte> span, bool isBigEndian)
    {
        return span.Length switch
        {
            8 => isBigEndian ? BinaryPrimitives.ReadUInt64BigEndian(span) : BinaryPrimitives.ReadUInt64LittleEndian(span),
            4 => isBigEndian ? BinaryPrimitives.ReadUInt32BigEndian(span) : BinaryPrimitives.ReadUInt32LittleEndian(span),
            2 => isBigEndian ? BinaryPrimitives.ReadUInt16BigEndian(span) : BinaryPrimitives.ReadUInt16LittleEndian(span),
            _ => 0
        };
    }
}
