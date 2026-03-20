namespace SpoofFileParser.FileMetadata;

public class VideoMetadata : IFileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public FileType FileType { get; init; } = FileType.Image;
    public long Size { get; set; }
    public long Width { get; set; }
    public long Height { get; set; }
}
