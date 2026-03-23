namespace SpoofFileParser;

public readonly record struct ExtensionRoadMap(
        short Id,
        byte[] Magic,
        UInt128 MagicMask,
        string Name,
        int Offset,
        FileType Type,
        byte[] SubMarkerOffset,
        int SubOffset,
        ExtensionRoadMap[] SubMarkers
    )
{
    private static readonly ExtensionRoadMap[] EmptyMarkers = [];
    private static readonly byte[] EmptySubMarkerOffset = [];
    public ExtensionRoadMap(
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
    public ExtensionRoadMap(
        short Id,
        byte[] Magic,
        UInt128 MagicMask,
        string Name,
        int Offset,
        FileType Type,
        int SubOffset,
        ExtensionRoadMap[] SubMarkers)
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
