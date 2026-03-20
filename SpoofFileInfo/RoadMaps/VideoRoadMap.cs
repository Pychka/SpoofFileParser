namespace SpoofFileParser.RoadMaps;

public readonly record struct VideoRoadMap(
        int WitdhtOffset,
        int WitdhtSize,
        int HeightOffset,
        int HeightSize,
        bool IsBigEndian,
        int[][] SizeMaskOffset,
        int SizeOffset
    )
{
    private static readonly int[][] EmptySizeMask = [];

    public VideoRoadMap(
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