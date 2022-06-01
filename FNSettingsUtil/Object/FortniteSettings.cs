namespace FNSettingsUtil.Object
{
    public class FortniteSettings
    {
        public IReadOnlyCollection<FGuid> Guids { get; set; }
        public IReadOnlyDictionary<string, UProperty> Properties { get; set; }

        public FortniteSettingsHeader Header { get; set; }

        public FortniteSettings(UBinaryReader stream)
        {
            Header = new FortniteSettingsHeader(stream);
            Guids = ParseGuidData(stream);
            Properties = stream.ReadProperties();
        }

        private static IReadOnlyCollection<FGuid> ParseGuidData(UBinaryReader reader)
        {
            var length = reader.ReadInt32();
            var guids = new List<FGuid>(length);

            for (var i = 0; i < length; i++)
            {
                var guid = reader.Read<FGuid>();
                //reader.Seek(4, SeekOrigin.Current);

                guids.Add(guid);
            }

            return guids;
        }
    }

    public class FortniteSettingsGuid
    {
        public string Guid { get; set; }
        public int Value { get; set; }

        public FortniteSettingsGuid(UBinaryReader reader)
        {
            Guid = reader.ReadGuid();
            Value = reader.ReadInt32();
        }
    }

    public class FortniteSettingsHeader
    {
        public const int HEADER_BLOCK_SIZE = 22; // 18
        public byte[] Unknown1 { get; set; }
        public string Branch { get; set; }
        public int Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }

        public FortniteSettingsHeader(UBinaryReader reader)
        {
            Unknown1 = reader.ReadBytes(HEADER_BLOCK_SIZE); // 18 + 4 = 22; old is 18
            Branch = reader.ReadFString();
            Unknown2 = reader.ReadInt32();
            Unknown3 = reader.ReadByte();
            Unknown4 = reader.ReadByte();
        }
    }
}