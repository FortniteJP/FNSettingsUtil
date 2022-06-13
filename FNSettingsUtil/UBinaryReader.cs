using GenericReader;

namespace FNSettingsUtil
{
    public class UBinaryReader : GenericStreamReader
    {
        internal Stream stream;
        public UBinaryReader(Stream stream) : base(stream)
        {
            this.stream = stream;
        }

        //public string ReadBytesToString(int count) => BitConverter.ToString(this.ReadBytes(count)).Replace("-", "");
        public Guid ReadGuid() => Read<Guid>();
        public float ReadSingle() => Read<float>();
        public short ReadInt16() => Read<short>();
        public int ReadInt32() => Read<int>();
        public long ReadInt64() => Read<long>();
        public uint ReadUInt32() => Read<uint>();

        public Dictionary<string, UProperty> ReadProperties()
        {
            var properties = new Dictionary<string, UProperty>();
            while (true)
            {
                string settingName = ReadFString();

                if (settingName == "None")
                {
                    return properties;
                }

                var type = ReadFString();
                //Console.WriteLine($"\n{settingName} :\n{type}\n");
                var uProperty = UTypes.GetPropertyByName(type);
                uProperty.Deserialize(this);
                properties.Add(settingName, uProperty);
            }
        }

        public Dictionary<string, UProperty> ReadProperties(int size)
        {
            var properties = new Dictionary<string, UProperty>();
            while (size < 0)
            {
                string settingName = ReadFString();
                UProperty uProperty;

                if (settingName == "None")
                {
                    uProperty = new FNoneProperty();
                }
                else
                {
                    var type = ReadFString();
                    uProperty = UTypes.GetPropertyByName(type);
                    uProperty.Deserialize(this);
                }

                properties.Add(settingName, uProperty);
            }
            return properties;
        }
    }
}
