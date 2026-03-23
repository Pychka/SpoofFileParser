namespace SpoofFileParser.RoadMaps;

public readonly record struct ImageRoadMap(
        int WitdhtOffset,
        int WitdhtSize,
        int HeightOffset,
        int HeightSize,
        bool IsBigEndian,
        byte[][] SizeMaskOffset,
        int SizeOffset
    )
{
    private static readonly byte[][] EmptySizeMask = [];

    public ImageRoadMap(
        int WitdhtOffset,
        int WitdhtSize,
        int HeightOffset,
        int HeightSize,
        bool IsBigEndian)
    : this(
        WitdhtOffset,
        WitdhtSize,
        HeightOffset,
        HeightSize,
        IsBigEndian,
        EmptySizeMask,
        -1)
    { }
}