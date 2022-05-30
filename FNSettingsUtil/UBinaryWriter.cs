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
            Write(settings.Header.Branch.Length);
            WriteFString(settings.Header.Branch);
            Write(settings.Header.Unknown2);
            WriteByte(settings.Header.Unknown3);
            WriteByte(settings.Header.Unknown4);

            Write(settings.Guids.Count);
            foreach (var guid in settings.Guids)
            {
                Write(guid);
                Seek(4, SeekOrigin.Current);
            }

            foreach (var property in settings.Properties)
            {
                WriteFString(property.Key);
                property.Value.SerializeProperty(this);
                /*_stream.Write(BitConverter.GetBytes(property.Key.Length));
                _stream.Write(Encoding.UTF8.GetBytes(property.Key));

                switch (property.Value)
                {
                    case FArrayProperty fArrayProperty:
                        _stream.Write(BitConverter.GetBytes(fArrayProperty._innerType.Length));
                        _stream.Write(Encoding.UTF8.GetBytes(fArrayProperty._innerType));

                        _stream.Write(BitConverter.GetBytes(fArrayProperty.Value.Count));
                        if (fArrayProperty._innerType == "StructProperty")
                        {
                            _stream.Write(BitConverter.GetBytes(fArrayProperty._settingName.Length));
                            _stream.Write(Encoding.UTF8.GetBytes(fArrayProperty._settingName));
                            _stream.Write(BitConverter.GetBytes(fArrayProperty._typeName.Length));
                            _stream.Write(Encoding.UTF8.GetBytes(fArrayProperty._typeName));

                            _stream.Write(BitConverter.GetBytes(fArrayProperty._property.Size));
                            _stream.Write(BitConverter.GetBytes(fArrayProperty._property.ArrayIndex));
                            _stream.Write(BitConverter.GetBytes(property.Value.HasPropertyGuid));
                            if (fArrayProperty._property is FStructProperty)
                            {
                                fArrayProperty._property
                            }
                        }
                        break;
                    case UStruct uStruct:
                    //_stream.Write(BitConverter.uStruct.Value);
                    default:
                        throw new NotImplementedException("No property type specified.");
                        //_stream.Write(BitConverter.GetBytes(property.Value.Size));
                        //_stream.Write(BitConverter.GetBytes(property.Value.ArrayIndex));
                        //_stream.Write(BitConverter.GetBytes(property.Value.HasPropertyGuid));

                        //if (property.Value.HasPropertyGuid)
                        //{
                        //throw new NotImplementedException("HasPropertyGuid is not implemented.");
                        //var guid = property.Value.Guid.Split('-'); // is this correct?
                        //_stream.Write(Encoding.UTF8.GetBytes(guid[0]));
                        //}

                        //_stream.Write(property.Value.Value);

                        //break;
                };*/
            }
        }
    }
}
