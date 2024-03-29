﻿using FNSettingsUtil.Object;

namespace FNSettingsUtil
{
    public class UBinaryWriter : GenericWriter.GenericStreamWriter
    {
        internal MemoryStream _stream = new();

        public UBinaryWriter(MemoryStream stream) : base(stream)
        {
            this._stream = stream;
        }

        public void WriteGuid(Guid value) => Write(value);
        public void WriteSingle(float value) => Write(value);
        public void WriteInt16(short value) => Write(value);
        public void WriteInt32(int value) => Write(value);
        public void WriteInt64(long value) => Write(value);
        public void WriteUInt32(uint value) => Write(value);

        public void WriteSettings(FortniteSettings settings)
        {
            WriteBytes(settings.Header.Unknown1);
            WriteFString(settings.Header.Branch);
            Write(settings.Header.Unknown2);
            WriteByte(settings.Header.Unknown3);
            WriteByte(settings.Header.Unknown4);

            Write(settings.Guids.Count);
            foreach (var guid in settings.Guids)
            {
                Write(guid);
            }

            foreach (var property in settings.Properties)
            {
                Console.WriteLine($"1({property.Value.TypeName}){property.Key}");
                if (property.Key.StartsWith("None"))
                {
                    WriteFString((string)property.Value.Value);
                }
                else
                {
                    WriteFString(property.Key);
                    WriteFString(property.Value.TypeName);
                    property.Value.Serialize(this);
                }
            }

            WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x4e, 0x6f, 0x6e, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        internal void WriteProperties(Dictionary<string, UProperty> properties)
        {
            foreach (var property in properties)
            {
                Console.WriteLine($"2({property.Value.TypeName}){property.Key}");
                if (property.Key.StartsWith("None"))
                {
                    WriteFString((string)property.Value.Value);
                }
                else
                {
                    WriteFString(property.Key);
                    WriteFString(property.Value.TypeName);
                    property.Value.Serialize(this);
                }
            }
        }
    }
}
