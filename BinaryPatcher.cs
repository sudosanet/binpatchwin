using System.Security.Cryptography;

namespace binpatchwin;

public static class BinaryPatcher
{
    private const int CoalesceGap = 8;

    public static PatchFile CreatePatch(string originalPath, string modifiedPath, IProgress<int>? progress = null)
    {
        var originalData = File.ReadAllBytes(originalPath);
        var modifiedData = File.ReadAllBytes(modifiedPath);
        var hash = ComputeHash(originalData);

        var overlapLen = Math.Min(originalData.Length, modifiedData.Length);
        var records = new List<DeltaRecord>();

        int diffStart = -1;
        for (int i = 0; i <= overlapLen; i++)
        {
            bool isDiff = i < overlapLen && originalData[i] != modifiedData[i];

            if (isDiff && diffStart < 0)
            {
                diffStart = i;
            }
            else if (!isDiff && diffStart >= 0)
            {
                // Check if the gap to the next diff is small enough to coalesce
                bool coalesce = false;
                if (i < overlapLen)
                {
                    for (int j = i; j < Math.Min(i + CoalesceGap, overlapLen); j++)
                    {
                        if (originalData[j] != modifiedData[j])
                        {
                            coalesce = true;
                            break;
                        }
                    }
                }

                if (!coalesce)
                {
                    var xorData = new byte[i - diffStart];
                    for (int j = 0; j < xorData.Length; j++)
                        xorData[j] = (byte)(originalData[diffStart + j] ^ modifiedData[diffStart + j]);
                    records.Add(new DeltaRecord { Offset = diffStart, Data = xorData });
                    diffStart = -1;
                }
            }

            if (i % 65536 == 0)
                progress?.Report((int)((long)i * 80 / Math.Max(1, overlapLen)));
        }

        // Handle extended portion (B is longer than A)
        if (modifiedData.Length > originalData.Length)
        {
            var mask = GenerateMask(hash, originalData.Length, modifiedData.Length - originalData.Length);
            var extData = new byte[modifiedData.Length - originalData.Length];
            for (int i = 0; i < extData.Length; i++)
                extData[i] = (byte)(modifiedData[originalData.Length + i] ^ mask[i]);
            records.Add(new DeltaRecord { Offset = originalData.Length, Data = extData });
        }

        progress?.Report(100);

        return new PatchFile
        {
            OriginalHash = hash,
            OriginalSize = originalData.Length,
            ModifiedSize = modifiedData.Length,
            Records = records
        };
    }

    public static void ApplyPatch(string originalPath, PatchFile patch, string outputPath, IProgress<int>? progress = null)
    {
        var originalData = File.ReadAllBytes(originalPath);
        var hash = ComputeHash(originalData);

        if (!hash.AsSpan().SequenceEqual(patch.OriginalHash))
            throw new InvalidOperationException("元ファイルのハッシュが一致しません。正しいファイルを選択してください。");

        progress?.Report(20);

        var output = new byte[patch.ModifiedSize];
        var copyLen = (int)Math.Min(originalData.Length, patch.ModifiedSize);
        Array.Copy(originalData, output, copyLen);

        // Fill extended portion with mask
        if (patch.ModifiedSize > originalData.Length)
        {
            var mask = GenerateMask(hash, originalData.Length, (int)(patch.ModifiedSize - originalData.Length));
            Array.Copy(mask, 0, output, originalData.Length, mask.Length);
        }

        progress?.Report(50);

        // Apply delta records
        for (int r = 0; r < patch.Records.Count; r++)
        {
            var record = patch.Records[r];
            for (int i = 0; i < record.Data.Length; i++)
                output[record.Offset + i] ^= record.Data[i];

            progress?.Report(50 + (int)((long)(r + 1) * 40 / patch.Records.Count));
        }

        File.WriteAllBytes(outputPath, output);
        progress?.Report(100);
    }

    public static byte[] ComputeHash(string filePath)
    {
        var data = File.ReadAllBytes(filePath);
        return ComputeHash(data);
    }

    private static byte[] ComputeHash(byte[] data)
    {
        return SHA256.HashData(data);
    }

    private static byte[] GenerateMask(byte[] hash, int startOffset, int length)
    {
        var mask = new byte[length];
        int pos = 0;
        int counter = 0;

        while (pos < length)
        {
            var input = new byte[hash.Length + 4 + 4];
            Array.Copy(hash, input, hash.Length);
            BitConverter.TryWriteBytes(input.AsSpan(hash.Length), startOffset);
            BitConverter.TryWriteBytes(input.AsSpan(hash.Length + 4), counter);
            var block = SHA256.HashData(input);

            var toCopy = Math.Min(block.Length, length - pos);
            Array.Copy(block, 0, mask, pos, toCopy);
            pos += toCopy;
            counter++;
        }

        return mask;
    }
}
