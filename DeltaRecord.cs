namespace binpatchwin
{
    public sealed class DeltaRecord
    {
        public long Offset { get; set; }
        public byte[] Data { get; set; } = new byte[0];
    }
}
