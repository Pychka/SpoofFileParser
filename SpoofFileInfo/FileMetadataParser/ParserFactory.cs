namespace SpoofFileParser.FileMetadataParser;

public class ParserFactory(IEnumerable<IFileMetadaParser> parsers) : IParserFactory
{
    private readonly IEnumerable<IFileMetadaParser> _parsers = parsers;

    public IFileMetadaParser? Get(FileType fileType)
    {
        return _parsers.FirstOrDefault(x => x.CanParse(fileType));
    }
}
