using GenericReader;

namespace FNSettingsUtil
{
    public class UBinaryReader : GenericStreamReader
    {
        public UBinaryReader(Stream stream) : base(stream) { }

        public string ReadBytesToString(int count) => BitConverter.ToString(this.ReadBytes(count)).Replace("-", "");
        public string ReadGuid() => Read<Guid>().ToString();
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
    }
}
