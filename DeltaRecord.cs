namespace binpatchwin;

public sealed class DeltaRecord
{
    public long Offset { get; init; }
    public byte[] Data { get; init; } = [];
}
