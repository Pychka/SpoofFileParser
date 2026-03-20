using Microsoft.Win32.SafeHandles;
using SpoofFileParser.FileMetadata;

namespace SpoofFileParser.FileMetadataParser;

public interface IFileMetadaParser
{
    public bool CanParse(FileType type);
    public IFileMetadata? Parse(SafeFileHandle handle, FileExtension2 extension2);
}
