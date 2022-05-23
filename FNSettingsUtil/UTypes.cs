namespace FNSettingsUtil
{
    public class UTypes
    {
        private static Dictionary<string, Func<UProperty>> _types = new(20)
        {
            { "FloatProperty", () => new FFloatProperty() },
            { "NameProperty", () => new FNameProperty() },
            { "StrProperty", () => new FStringProperty() },
            { "EnumProperty", () => new FEnumProperty() },
            { "TextProperty", () => new FTextProperty() },
            { "BoolProperty", () => new FBoolProperty() },
            { "StructProperty", () => new FStructProperty() },
            { "MapProperty", () => new FMapProperty() },
            { "ArrayProperty", () => new FArrayProperty() },
            { "SetProperty", () => new FSetProperty() },
            { "IntProperty", () => new FIntProperty() },
            { "Int16Property", () => new FInt16Property() },
            { "UInt32Property", () => new FUInt32Property() },
            { "ObjectProperty", () => new FObjectProperty() },
            { "ByteProperty", () => new FByteProperty() },
            { "Vector2D", () => new FVector2D() },
            { "DateTime", () => new FDateTime() },
            { "Guid", () => new FGuid() },
            { "FortActorRecord", () => new FFortActorRecord() }
        };

        public static bool HasPropertyName(string name) => _types.ContainsKey(name);

        public static UProperty GetPropertyByName(string name)
        {
            if (!_types.TryGetValue(name, out Func<UProperty> propertyFunc))
            {
                //Console.WriteLine($"? {name}[{System.Text.Encoding.UTF8.GetBytes(name)}]");
                throw new NotImplementedException($"Type {name} currently not implemented");
            }

            var uProperty = propertyFunc();
            uProperty.TypeName = name;
            return uProperty;
        }
    }
}
