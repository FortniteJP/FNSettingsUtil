﻿using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using FNSettingsUtil.Object;
using System.IO.Compression;

namespace FNSettingsUtil
{
    public class FNSettingsUtil
    {
        private static UBinaryReader _binaryReader;
        private static UBinaryWriter _binaryWriter;

        private const uint ClientSettingsMagic = 0x44464345;

        public static async Task<FortniteSettings> DeserializeClientSettingsAsync(Stream stream)
        {
            _binaryReader = new UBinaryReader(stream);
            await Decompress();
            return new FortniteSettings(_binaryReader);
        }

        public static Task<FortniteSettings> DeserializeClientSettingsAsync(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open);
            return DeserializeClientSettingsAsync(stream);
        }

        public static async Task<MemoryStream> SerializeClientSettingsAsync(FortniteSettings settings)
        {
            _binaryWriter = new UBinaryWriter(new());
            await Compress(settings);
            return _binaryWriter._stream;
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
            //Console.WriteLine(BitConverter.ToString(data[0..2]));
            var decompressedStream = new MemoryStream(length);

            await using var compressedStream = new MemoryStream(data);
            await using var inflaterStream = new InflaterInputStream(compressedStream);

            await inflaterStream.CopyToAsync(decompressedStream);
            decompressedStream.Seek(0, SeekOrigin.Begin);
            //await decompressedStream.CopyToAsync(File.OpenWrite("Data/RawStream"));
            //decompressedStream.Seek(0, SeekOrigin.Begin);

            _binaryReader = new UBinaryReader(decompressedStream);

            /*var compressedStream = new MemoryStream(data);
            var ds = new DeflateStream(compressedStream, CompressionMode.Decompress);

            var decompressedStream = new MemoryStream(length);

            await ds.CopyToAsync(decompressedStream);
            compressedStream.Close();
            ds.Close();
            _binaryReader = new UBinaryReader(decompressedStream);*/
        }

        private static async Task Compress(FortniteSettings settings)
        {
            _binaryWriter.WriteSettings(settings);

            await CompressData();

            _binaryWriter.Seek(0, SeekOrigin.Begin);
            _binaryWriter.Write(ClientSettingsMagic);
            _binaryWriter.Seek(4, SeekOrigin.Current);

            _binaryWriter.Write(true);

            _binaryWriter.Write(_binaryReader.Size);
        }

        private static async Task CompressData()
        {
            var compressedStream = new MemoryStream();
            var deflaterStream = new DeflaterOutputStream(compressedStream, new());

            int mSize;
            byte[] mWriteData = new byte[4096];
            while ((mSize = _binaryWriter._stream.Read(mWriteData, 0, mWriteData.Length)) > 0)
            {
                deflaterStream.Write(mWriteData, 0, mSize);
            }
            deflaterStream.Finish();
            compressedStream.Seek(0, SeekOrigin.Begin);

            _binaryWriter = new UBinaryWriter(compressedStream);
        }
    }
}