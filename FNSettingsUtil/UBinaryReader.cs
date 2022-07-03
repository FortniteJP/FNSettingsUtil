using GenericReader;

namespace FNSettingsUtil
{
    public class UBinaryReader : GenericStreamReader
    {
        private static int n = 0;

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

                if (settingName == "None" || string.IsNullOrEmpty(settingName))
                {
                    return properties;
                }

                var type = ReadFString();
                Console.WriteLine($"({type}){settingName}");
                var uProperty = UTypes.GetPropertyByName(type);
                uProperty.Deserialize(this);
                properties.Add(settingName, uProperty);
            }
        }

        public Dictionary<string, UProperty> ReadPropertiesN()// => ReadProperties(); private object _()
        {
            Guid _ = Guid.NewGuid();
            n++;
            Console.WriteLine($"{new(' ', n)}{new(' ', n)}[{_}]Start");
            var properties = new Dictionary<string, UProperty>();
            bool LNone = false;
            int pos = (int)Position;
            while (true)
            {
                //Console.WriteLine($"[{_}]bpos: {Position}, {pos}");
                string settingName = ReadFString();
                //Console.WriteLine($"[{_}]apos: {Position}, {settingName}");
                UProperty uProperty;

                if (LNone)
                {
                    Seek(pos, SeekOrigin.Begin);
                    Console.WriteLine($"{new(' ', n)}{new(' ', n)}[{_}]Last1: {Position}");
                    n--;
                    return properties;
                }
                else if (settingName == "None")
                {
                    LNone = true;
                    uProperty = new FNoneProperty();
                    properties.Add(settingName + Position, uProperty);
                }
                else
                {
                    try
                    {
                        LNone = false;
                        var type = ReadFString();
                        uProperty = UTypes.GetPropertyByName(type);
                        uProperty.Deserialize(this);
                        properties.Add(settingName, uProperty);
                    }
                    catch (NotImplementedException ex)
                    {
                        Seek(pos, SeekOrigin.Begin);
                        Console.WriteLine($"{new(' ', n)}{new(' ', n)}[{_}]Last2: {Position}");
                        n--;
                        return properties;
                    }
                }
                pos = (int)Position;
            }
        }
    }
}
