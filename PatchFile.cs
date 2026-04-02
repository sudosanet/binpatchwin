namespace binpatchwin;

public sealed class PatchFile
{
    private static readonly byte[] Magic = "BPAT"u8.ToArray();
    private const ushort CurrentVersion = 1;

    public byte[] OriginalHash { get; init; } = new byte[32];
    public long OriginalSize { get; init; }
    public long ModifiedSize { get; init; }
    public List<DeltaRecord> Records { get; init; } = [];

    public void WriteTo(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(Magic);
        writer.Write(CurrentVersion);
        writer.Write(OriginalHash);
        writer.Write(OriginalSize);
        writer.Write(ModifiedSize);
        writer.Write((uint)Records.Count);

        foreach (var record in Records)
        {
            writer.Write(record.Offset);
            writer.Write((uint)record.Data.Length);
            writer.Write(record.Data);
        }
    }

    public static PatchFile ReadFrom(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        var magic = reader.ReadBytes(4);
        if (!magic.AsSpan().SequenceEqual(Magic))
            throw new InvalidDataException("無効なパッチファイルです。");

        var version = reader.ReadUInt16();
        if (version != CurrentVersion)
            throw new InvalidDataException($"サポートされていないパッチバージョンです: {version}");

        var hash = reader.ReadBytes(32);
        var originalSize = reader.ReadInt64();
        var modifiedSize = reader.ReadInt64();
        var recordCount = reader.ReadUInt32();

        var records = new List<DeltaRecord>((int)recordCount);
        for (uint i = 0; i < recordCount; i++)
        {
            var offset = reader.ReadInt64();
            var length = reader.ReadUInt32();
            var data = reader.ReadBytes((int)length);
            records.Add(new DeltaRecord { Offset = offset, Data = data });
        }

        return new PatchFile
        {
            OriginalHash = hash,
            OriginalSize = originalSize,
            ModifiedSize = modifiedSize,
            Records = records
        };
    }
}
