namespace SpoofFileParser;

public readonly record struct FileExtension(
        short Id,
        string Name,
        long Size,
        FileType Type
    );
