namespace SpoofFileParser;

public readonly record struct FileExtension2(
        short Id,
        byte[] Magic,
        UInt128 MagicMask,
        string Name,
        int Offset,
        FileType Type,
        byte[] SubMarkerOffset,
        int SubOffset,
        FileExtension2[] SubMarkers,
        long Size = -1
    )
{
    private static readonly FileExtension2[] EmptyMarkers = [];
    private static readonly byte[] EmptySubMarkerOffset = [];
    public FileExtension2(
        short Id,
        byte[] Magic,
        UInt128 MagicMask,
        string Name,
        int Offset,
        FileType Type)
        : this(
              Id, 
              Magic,
              MagicMask,
              Name,
              Offset, 
              Type, 
              EmptySubMarkerOffset,
              -1, 
              EmptyMarkers)
    { }
    public FileExtension2(
        short Id,
        byte[] Magic,
        UInt128 MagicMask,
        string Name,
        int Offset,
        FileType Type,
        int SubOffset,
        FileExtension2[] SubMarkers)
        : this(
              Id,
              Magic,
              MagicMask,
              Name,
              Offset,
              Type,
              EmptySubMarkerOffset,
              SubOffset,
              SubMarkers)
    { }
}
