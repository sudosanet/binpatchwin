using System;
using System.IO;
using System.Linq;
using Xunit;

namespace binpatchwin.Tests
{
    public class BinaryPatcherTests : IDisposable
    {
        private readonly string _tempDir;

        public BinaryPatcherTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), string.Format("binpatch-test-{0:N}", Guid.NewGuid()));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private void AssertRoundTrip(byte[] original, byte[] modified)
        {
            var originalPath = Path.Combine(_tempDir, "original.bin");
            var modifiedPath = Path.Combine(_tempDir, "modified.bin");
            var patchPath = Path.Combine(_tempDir, "patch.bpat");
            var outputPath = Path.Combine(_tempDir, "output.bin");

            File.WriteAllBytes(originalPath, original);
            File.WriteAllBytes(modifiedPath, modified);

            var patch = BinaryPatcher.CreatePatch(originalPath, modifiedPath);

            using (var fs = File.Create(patchPath))
                patch.WriteTo(fs);

            PatchFile loadedPatch;
            using (var fs = File.OpenRead(patchPath))
                loadedPatch = PatchFile.ReadFrom(fs);

            BinaryPatcher.ApplyPatch(originalPath, loadedPatch, outputPath);

            var result = File.ReadAllBytes(outputPath);
            Assert.Equal(modified, result);
        }

        [Fact]
        public void RoundTrip_SameSizeWithDifferences()
        {
            var rng = new Random(42);
            var original = new byte[1024];
            rng.NextBytes(original);

            var modified = (byte[])original.Clone();
            modified[0] = (byte)(modified[0] ^ 0xFF);
            modified[100] = (byte)(modified[100] ^ 0xAB);
            modified[500] = (byte)(modified[500] ^ 0x01);
            modified[1023] = (byte)(modified[1023] ^ 0x77);

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void RoundTrip_ModifiedLonger()
        {
            var rng = new Random(100);
            var original = new byte[512];
            rng.NextBytes(original);

            var modified = new byte[1024];
            rng.NextBytes(modified);
            Array.Copy(original, modified, 256);

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void RoundTrip_ModifiedShorter()
        {
            var rng = new Random(200);
            var original = new byte[1024];
            rng.NextBytes(original);

            var modified = new byte[512];
            Array.Copy(original, modified, 512);
            modified[0] ^= 0xFF;
            modified[511] ^= 0xFF;

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void RoundTrip_IdenticalFiles()
        {
            var rng = new Random(300);
            var data = new byte[1024];
            rng.NextBytes(data);

            var original = (byte[])data.Clone();
            var modified = (byte[])data.Clone();

            var originalPath = Path.Combine(_tempDir, "original.bin");
            var modifiedPath = Path.Combine(_tempDir, "modified.bin");
            File.WriteAllBytes(originalPath, original);
            File.WriteAllBytes(modifiedPath, modified);

            var patch = BinaryPatcher.CreatePatch(originalPath, modifiedPath);
            Assert.Empty(patch.Records);

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void RoundTrip_CompletelyDifferent()
        {
            var rng1 = new Random(400);
            var rng2 = new Random(500);
            var original = new byte[1024];
            var modified = new byte[1024];
            rng1.NextBytes(original);
            rng2.NextBytes(modified);

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void RoundTrip_LargeFile()
        {
            var rng = new Random(600);
            var original = new byte[1024 * 1024];
            rng.NextBytes(original);

            var modified = (byte[])original.Clone();
            for (int i = 0; i < modified.Length; i += 4096)
                modified[i] ^= 0xFF;

            AssertRoundTrip(original, modified);
        }

        [Fact]
        public void ApplyPatch_WrongOriginal_ThrowsException()
        {
            var rng = new Random(700);
            var original = new byte[512];
            var modified = new byte[512];
            var wrongFile = new byte[512];
            rng.NextBytes(original);
            rng.NextBytes(modified);
            rng.NextBytes(wrongFile);

            var originalPath = Path.Combine(_tempDir, "original.bin");
            var modifiedPath = Path.Combine(_tempDir, "modified.bin");
            var wrongPath = Path.Combine(_tempDir, "wrong.bin");
            var outputPath = Path.Combine(_tempDir, "output.bin");

            File.WriteAllBytes(originalPath, original);
            File.WriteAllBytes(modifiedPath, modified);
            File.WriteAllBytes(wrongPath, wrongFile);

            var patch = BinaryPatcher.CreatePatch(originalPath, modifiedPath);

            Assert.Throws<InvalidOperationException>(() =>
                BinaryPatcher.ApplyPatch(wrongPath, patch, outputPath));
        }
    }
}
