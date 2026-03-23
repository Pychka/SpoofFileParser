using Microsoft.Win32.SafeHandles;
using SpoofFileParser.FileMetadata;
using SpoofFileParser.RoadMaps;
using System.Buffers.Binary;
using System.Collections.Concurrent;

namespace SpoofFileParser.FileMetadataParser;

public class ImageMetadataParser(ConcurrentDictionary<string, ImageRoadMap> roadMaps) : IFileMetadaParser
{
    private readonly ConcurrentDictionary<string, ImageRoadMap> _roadMaps = roadMaps;
    public bool CanParse(FileType type) =>
        type == FileType.Image;

    public IFileMetadata? Parse(SafeFileHandle handle, FileExtension extension2)
    {
        if (_roadMaps.TryGetValue(extension2.Name, out ImageRoadMap roadMap))
        {
            Span<byte> data = stackalloc byte[(int)Math.Min(1024 * 10, RandomAccess.GetLength(handle))];
            RandomAccess.Read(handle, data, 0);
            ImageMetadata metadata;
            if (roadMap.SizeMaskOffset.Length == 0)
            {
                metadata = new()
                {
                    Extension = extension2.Name,
                    FileType = extension2.Type,
                    Height = GetULong(data.Slice(roadMap.HeightOffset, roadMap.HeightSize), roadMap.IsBigEndian),
                    Width = GetULong(data.Slice(roadMap.WitdhtOffset, roadMap.WitdhtSize), roadMap.IsBigEndian),
                };
            }
            else
            {
                int index = -1;
                for (int i = 0; i < roadMap.SizeMaskOffset.Length; i++)
                {
                    index = data.IndexOf(roadMap.SizeMaskOffset[i]);
                    if (index != -1)
                        break;
                }
                if (index == -1)
                    return default;
                index += roadMap.SizeOffset;
                metadata = new()
                {
                    Extension = extension2.Name,
                    FileType = extension2.Type,
                    Height = GetULong(data.Slice(index + roadMap.HeightOffset, roadMap.HeightSize), roadMap.IsBigEndian),
                    Width = GetULong(data.Slice(index + roadMap.WitdhtOffset, roadMap.WitdhtSize), roadMap.IsBigEndian),
                };
            }
            return metadata;
        }
        return default;
    }

    private static ulong GetULong(Span<byte> span, bool isBigEndian)
    {
        return span.Length switch
        {
            8 => isBigEndian 
                ? BinaryPrimitives.ReadUInt64BigEndian(span) 
                : BinaryPrimitives.ReadUInt64LittleEndian(span),
            4 => isBigEndian 
                ? BinaryPrimitives.ReadUInt32BigEndian(span) 
                : BinaryPrimitives.ReadUInt32LittleEndian(span),
            2 => isBigEndian
                ? BinaryPrimitives.ReadUInt32BigEndian(span)
                : BinaryPrimitives.ReadUInt32LittleEndian(span),
            _ => 0
        };
    }
}