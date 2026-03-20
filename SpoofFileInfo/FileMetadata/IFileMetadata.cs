namespace SpoofFileParser.FileMetadata;

public interface IFileMetadata
{
    public string FileName { get; set; }
    public string Extension { get; set; }
    public FileType FileType { get; init; }
    public long Size { get; set; }
}
