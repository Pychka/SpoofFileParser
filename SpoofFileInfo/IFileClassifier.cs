using Microsoft.Win32.SafeHandles;

namespace SpoofFileParser;

public interface IFileClassifier
{
    public string? GetExtensionName(string filepath);
    public string? GetExtensionName(SafeFileHandle handle);
    public short GetExtensionId(string filepath);
    public short GetExtensionId(SafeFileHandle handle);
    public FileExtension GetExtension(string filepath);
}
