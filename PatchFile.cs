using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace binpatchwin
{
    public sealed class PatchFile
    {
        private static readonly byte[] Magic = new byte[] { 0x42, 0x50, 0x41, 0x54 }; // "BPAT"
        private const ushort CurrentVersion = 1;

        public byte[] OriginalHash { get; set; } = new byte[32];
        public long OriginalSize { get; set; }
        public long ModifiedSize { get; set; }
        public List<DeltaRecord> Records { get; set; } = new List<DeltaRecord>();

        public void WriteTo(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
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
        }

        public static PatchFile ReadFrom(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var magic = reader.ReadBytes(4);
                if (!magic.SequenceEqual(Magic))
                    throw new InvalidDataException("無効なパッチファイルです。");

                var version = reader.ReadUInt16();
                if (version != CurrentVersion)
                    throw new InvalidDataException(string.Format("サポートされていないパッチバージョンです: {0}", version));

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
    }
}
