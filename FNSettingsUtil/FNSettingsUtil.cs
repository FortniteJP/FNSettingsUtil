using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using FNSettingsUtil.Object;

namespace FNSettingsUtil
{
    public class FNSettingsUtil
    {
        private static UBinaryReader _binaryReader;

        private const uint ClientSettingsMagic = 0x44464345;

        public static async Task<FortniteSettings> GetClientSettingsAsync(Stream stream)
        {
            _binaryReader = new UBinaryReader(stream);
            await Decompress();
            return new FortniteSettings(_binaryReader);
        }

        public static Task<FortniteSettings> GetClientSettingsAsync(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open);
            return GetClientSettingsAsync(stream);
        }

        private static async Task Decompress()
        {
            var magic = _binaryReader.Read<uint>();

            if (!magic.Equals(ClientSettingsMagic)) throw new NotImplementedException("Invalid settings file.");
            _binaryReader.Seek(4, SeekOrigin.Current); //63001 unknown?

            var isCompressed = _binaryReader.ReadBoolean();
            if (!isCompressed) throw new NotImplementedException("File is not compressed");

            var size = _binaryReader.Read<int>();
            await DecompressData(_binaryReader.ReadBytes(_binaryReader.Size - (int)_binaryReader.Position), size);
        }

        private static async Task DecompressData(byte[] data, int length)
        {
            var decompressedStream = new MemoryStream(length);

            await using var compressedStream = new MemoryStream(data);
            await using var inflaterStream = new InflaterInputStream(compressedStream);

            await inflaterStream.CopyToAsync(decompressedStream);
            decompressedStream.Seek(0, SeekOrigin.Begin);
            await decompressedStream.CopyToAsync(File.OpenWrite("Data/RawStream"));
            decompressedStream.Seek(0, SeekOrigin.Begin);

            _binaryReader = new UBinaryReader(decompressedStream);
        }

    }
}