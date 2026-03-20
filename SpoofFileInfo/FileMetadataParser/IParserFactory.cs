namespace SpoofFileParser.FileMetadataParser;

public interface IParserFactory
{
    public IFileMetadaParser? Get(FileType fileType);
}
