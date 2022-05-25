using FNSettingsUtil.Object;
using System.Runtime.CompilerServices;
using System.Text;

namespace FNSettingsUtil
{
    public unsafe class UBinaryWriter
    {
        //private byte[] Bytes = new byte[] { };
        private MemoryStream _stream = new();
        public readonly FortniteSettings settings;

        public UBinaryWriter(FortniteSettings settings)
        {
            this.settings = settings;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value) where T : unmanaged
        {
            var buffer = Bytes.Last();
            Unsafe.WriteUnaligned<T>(ref buffer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value, int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged
        {
            var buffer = Bytes[offset];
            Unsafe.WriteUnaligned<T>(ref buffer, value);
        }

        public void WriteGuid(Guid value) => Write(value);
        public void WriteSingle(float value) => Write(value);
        public void WriteInt16(short value) => Write(value);
        public void WriteInt32(int value) => Write(value);
        public void WriteInt64(long value) => Write(value);
        public void WriteUInt32(uint value) => Write(value);*/

        public void WriteSettings()
        {
            _stream.Write(settings.Header.Unknown1);
            _stream.Write(BitConverter.GetBytes(settings.Header.Branch.Length));
            _stream.Write(Encoding.UTF8.GetBytes(settings.Header.Branch));
            _stream.Write(BitConverter.GetBytes(settings.Header.Unknown2));
            _stream.WriteByte(settings.Header.Unknown3);
            _stream.WriteByte(settings.Header.Unknown4);

            _stream.Write(BitConverter.GetBytes(settings.Guids.Count));
            foreach (var guid in settings.Guids)
            {
                _stream.Write(BitConverter.GetBytes(guid.A));
                _stream.Write(BitConverter.GetBytes(guid.B));
                _stream.Write(BitConverter.GetBytes(guid.C));
                _stream.Write(BitConverter.GetBytes(guid.D));
                _stream.Seek(4, SeekOrigin.Current);
            }

            foreach (var property in settings.Properties)
            {
                _stream.Write(BitConverter.GetBytes(property.Key.Length));
                _stream.Write(Encoding.UTF8.GetBytes(property.Key));

                switch (property.Value)
                {
                    case UStruct uStruct:

                    default:
                        throw new NotImplementedException("No property type specified.");
                        _stream.Write(BitConverter.GetBytes(property.Value.Size));
                        _stream.Write(BitConverter.GetBytes(property.Value.ArrayIndex));
                        _stream.Write(BitConverter.GetBytes(property.Value.HasPropertyGuid));

                        if (property.Value.HasPropertyGuid)
                        {
                            throw new NotImplementedException("HasPropertyGuid is not implemented.");
                            //var guid = property.Value.Guid.Split('-'); // is this correct?
                            //_stream.Write(Encoding.UTF8.GetBytes(guid[0]));
                        }

                        //_stream.Write(property.Value.Value);

                        break;
                };
            }
        }
    }
}
