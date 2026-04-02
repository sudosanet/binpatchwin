using Xunit;

namespace binpatchwin.Tests;

public class PatchFileSerializationTests
{
    [Fact]
    public void SerializationRoundTrip_PreservesAllFields()
    {
        var rng = new Random(1);
        var hash = new byte[32];
        rng.NextBytes(hash);
        var data1 = new byte[16];
        rng.NextBytes(data1);

        var original = new PatchFile
        {
            OriginalHash = hash,
            OriginalSize = 1000,
            ModifiedSize = 1200,
            Records = [new DeltaRecord { Offset = 50, Data = data1 }]
        };

        using var ms = new MemoryStream();
        original.WriteTo(ms);
        ms.Position = 0;
        var loaded = PatchFile.ReadFrom(ms);

        Assert.Equal(original.OriginalHash, loaded.OriginalHash);
        Assert.Equal(original.OriginalSize, loaded.OriginalSize);
        Assert.Equal(original.ModifiedSize, loaded.ModifiedSize);
        Assert.Single(loaded.Records);
        Assert.Equal(50, loaded.Records[0].Offset);
        Assert.Equal(data1, loaded.Records[0].Data);
    }

    [Fact]
    public void SerializationRoundTrip_EmptyRecords()
    {
        var original = new PatchFile
        {
            OriginalHash = new byte[32],
            OriginalSize = 500,
            ModifiedSize = 500,
            Records = []
        };

        using var ms = new MemoryStream();
        original.WriteTo(ms);
        ms.Position = 0;
        var loaded = PatchFile.ReadFrom(ms);

        Assert.Empty(loaded.Records);
        Assert.Equal(500, loaded.OriginalSize);
        Assert.Equal(500, loaded.ModifiedSize);
    }

    [Fact]
    public void SerializationRoundTrip_MultipleRecords()
    {
        var rng = new Random(2);
        var hash = new byte[32];
        rng.NextBytes(hash);

        var records = new List<DeltaRecord>();
        for (int i = 0; i < 5; i++)
        {
            var data = new byte[32 + i * 10];
            rng.NextBytes(data);
            records.Add(new DeltaRecord { Offset = i * 100, Data = data });
        }

        var original = new PatchFile
        {
            OriginalHash = hash,
            OriginalSize = 2000,
            ModifiedSize = 2500,
            Records = records
        };

        using var ms = new MemoryStream();
        original.WriteTo(ms);
        ms.Position = 0;
        var loaded = PatchFile.ReadFrom(ms);

        Assert.Equal(5, loaded.Records.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(records[i].Offset, loaded.Records[i].Offset);
            Assert.Equal(records[i].Data, loaded.Records[i].Data);
        }
    }

    [Fact]
    public void ReadFrom_InvalidMagic_ThrowsException()
    {
        var data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        using var ms = new MemoryStream(data);
        Assert.Throws<InvalidDataException>(() => PatchFile.ReadFrom(ms));
    }

    [Fact]
    public void ReadFrom_UnsupportedVersion_ThrowsException()
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            writer.Write("BPAT"u8);
            writer.Write((ushort)999);
        }
        ms.Position = 0;
        Assert.Throws<InvalidDataException>(() => PatchFile.ReadFrom(ms));
    }
}
