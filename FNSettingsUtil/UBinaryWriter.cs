using FNSettingsUtil.Object;
using System.Runtime.CompilerServices;
using System.Text;

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
            WriteByte(0x00); // why?
            Write(settings.Header.Unknown2);
            WriteByte(settings.Header.Unknown3);
            WriteByte(settings.Header.Unknown4);

            Write(settings.Guids.Count);
            foreach (var guid in settings.Guids)
            {
                Write(guid);
                Seek(4, SeekOrigin.Current);
            }

            /*foreach (var property in settings.Properties)
            {
                WriteFString(property.Key);
                property.Value.Serialize(this);
            }*/
        }
    }
}
