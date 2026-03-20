namespace SpoofFileParser.FileMetadata;

public class ImageMetadata : IFileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public FileType FileType { get; init; } = FileType.Image;
    public long Size { get; set; }
    public ulong Width { get; set; }
    public ulong Height { get; set; }
}
