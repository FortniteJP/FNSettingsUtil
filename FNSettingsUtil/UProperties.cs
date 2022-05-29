using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace FNSettingsUtil
{
    public abstract class UProperty
    {
        public string TypeName { get; internal set; }
        public virtual object Value { get; protected set; }
        public int Size { get; internal set; }
        public int ArrayIndex { get; internal set; }
        public bool HasPropertyGuid { get; internal set; }
        public string Guid { get; internal set; }

        public virtual void Deserialize(UBinaryReader reader)
        {
            DeserializeTypeInfo(reader);
            DeserializeProperty(reader);
        }

        protected internal virtual void PreDeserializeProperty(UBinaryReader reader) { }

        protected internal virtual void DeserializeTypeInfo(UBinaryReader reader)
        {
            Size = reader.ReadInt32();
            ArrayIndex = reader.ReadInt32();

            PreDeserializeProperty(reader);

            HasPropertyGuid = reader.Read<bool>();

            if (HasPropertyGuid)
            {
                Guid = reader.ReadGuid();
            }
        }

        protected internal virtual void DeserializeProperty(UBinaryReader reader)
        {
            Value = reader.ReadBytes(Size);
        }

        protected internal virtual void Serialize(UBinaryWriter writer)
        {
            SerializeTypeInfo(writer);
            SerializeProperty(writer);
        }

        protected internal virtual void SerializeTypeInfo(UBinaryWriter writer)
        {
            writer.WriteFString(TypeName);

            writer.Write(Size);
            writer.Write(ArrayIndex);
            writer.Write(HasPropertyGuid);

            if (HasPropertyGuid)
            {
                throw new NotImplementedException("HasPropertyGuid");
            }
        }

        protected internal virtual void SerializeProperty(UBinaryWriter writer)
        {
            //new BinaryFormatter().Serialize(stream, Value);
            var x = Value?.ToString();
            if (x is not null) writer.WriteFString(x);
        }
    }

    public class UStruct : UProperty
    {
        public override void Deserialize(UBinaryReader reader)
        {
            DeserializeProperty(reader);
        }

        protected internal override void PreDeserializeProperty(UBinaryReader reader) { }
    }

    public class FArrayProperty : UProperty
    {
        internal string _innerType;
        internal string innerTypeName;
        internal FStructProperty fStructProperty;
        internal string _settingName;
        internal string _typeName;
        internal UProperty _property;
        public new List<UProperty> Value { get; protected set; }

        protected internal override void PreDeserializeProperty(UBinaryReader reader) => _innerType = reader.ReadFString();

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            List<UProperty> items = new List<UProperty>();

            int count = reader.ReadInt32();

            innerTypeName = null;

            if (_innerType == "StructProperty")
            {
                _settingName = reader.ReadFString();
                _typeName = reader.ReadFString();

                _property = UTypes.GetPropertyByName(_innerType);
                _property.DeserializeTypeInfo(reader);

                innerTypeName = _property.TypeName;

                if (_property is FStructProperty structProperty)
                {
                    fStructProperty = structProperty;
                    if (UTypes.HasPropertyName(structProperty._structName))
                    {
                        innerTypeName = structProperty._structName;
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                UProperty arrayType = UTypes.GetPropertyByName(innerTypeName ?? _innerType);

                arrayType.DeserializeProperty(reader);
                arrayType.ArrayIndex = i;

                items.Add(arrayType);
            }

            Value = items;
        }

        protected internal override void SerializeProperty(UBinaryWriter writer)
        {
            writer.WriteFString(_innerType);

            writer.Write(Value.Count);
            if (_innerType == "StructProperty")
            {
                writer.WriteFString(_settingName);
                writer.WriteFString(_typeName);

                _property.Serialize(writer);
                /*stream.Write(BitConverter.GetBytes(_property.Size));
                stream.Write(BitConverter.GetBytes(_property.ArrayIndex));
                stream.Write(BitConverter.GetBytes(_property.Value.HasPropertyGuid));
                if (_property is FStructProperty _structProperty)
                {
                    _structProperty
                }*/

                foreach (var item in Value)
                {
                    item.Serialize(writer);
                }
            }
        }
    }

    public class FBoolProperty : UProperty
    {
        protected internal override void PreDeserializeProperty(UBinaryReader reader) => Value = reader.Read<bool>();

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            Value ??= reader.Read<bool>();
        }
    }

    public class FByteProperty : UProperty
    {
        public string Name { get; private set; }

        protected internal override void PreDeserializeProperty(UBinaryReader reader) => Name = reader.ReadFString();

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            if (Name == null || Name == "None")
            {
                Value = reader.ReadByte();
            }
            else
            {
                Value = reader.ReadFString();
            }
        }
    }

    public class FDateTime : UStruct
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = new DateTime(reader.ReadInt64());
    }

    public class FEnumProperty : UProperty
    {
        public string EnumName { get; private set; }

        protected internal override void PreDeserializeProperty(UBinaryReader reader) => EnumName = reader.ReadFString();

        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadFString();
    }

    public class FFloatProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadSingle();
    }

    public class FFortActorRecord : UStruct
    {
        public EFortBuildingPersistentState ActorState { get; private set; }
        public string ActorPath { get; private set; }
        public FQuat Rotation { get; private set; } = new FQuat();
        public FVector3D Location { get; private set; } = new FVector3D();
        public FVector3D Scale { get; private set; } = new FVector3D();
        public bool SpawnedActor { get; private set; }

        public int PropertyByteSize { get; private set; }
        public Dictionary<string, UProperty> ActorData { get; private set; }

        private byte[] UnknownExtraBytes;

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            Value = reader.ReadGuid();
            ActorState = (EFortBuildingPersistentState)reader.ReadByte();
            ActorPath = reader.ReadFString();

            Rotation.DeserializeProperty(reader);
            Location.DeserializeProperty(reader);
            Scale.DeserializeProperty(reader);

            SpawnedActor = reader.ReadInt32() == 1;

            PropertyByteSize = reader.ReadInt32();

            long currentPosition = reader.Position;

            if (PropertyByteSize > 0)
            {
                ActorData = reader.ReadProperties();
                reader.ReadInt32();
            }

            long remainingBytes = PropertyByteSize - (reader.Position - currentPosition);

            if (remainingBytes > 0)
            {
                UnknownExtraBytes = reader.ReadBytes((int)remainingBytes);
            }
        }
    }
    public enum EFortBuildingPersistentState
    {
        Default,
        New,
        Constructed,
        Destroyed,
        Searched,
        None,
        EFortBuildingPersistentState_MAX,
    }

    public class FGuid : UStruct
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadGuid();
    }

    public class FInt16Property : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadInt16();
    }

    public class FIntProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadInt32();
    }

    public class FMapProperty : UProperty
    {
        private string _innerType;
        private string _valueType;

        protected int NumKeysToRemove { get; set; }

        protected internal override void PreDeserializeProperty(UBinaryReader reader)
        {
            _innerType = reader.ReadFString();
            _valueType = reader.ReadFString();
        }

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            Dictionary<UProperty, UProperty> values = new Dictionary<UProperty, UProperty>();

            NumKeysToRemove = reader.ReadInt32();
            int numEntries = reader.ReadInt32();

            for (int i = 0; i < NumKeysToRemove; i++)
            {
                UProperty propertyKey = UTypes.GetPropertyByName(_innerType);
                propertyKey.DeserializeProperty(reader);
            }

            for (int i = 0; i < numEntries; i++)
            {
                UProperty propertyKey = UTypes.GetPropertyByName(_innerType);
                propertyKey.DeserializeProperty(reader);

                UProperty propertyValue = UTypes.GetPropertyByName(_valueType);

                propertyValue.DeserializeProperty(reader);

                values.Add(propertyKey, propertyValue);
            }

            Value = values;
        }
    }

    public class FNameProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadFString();
    }

    public class FObjectProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadFString();
    }

    public class FQuat : UStruct
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            W = reader.ReadSingle();

            Value = $"X: {X}, Y: {Y}, Z: {Z}, W: {W}";
        }
    }

    public class FStringProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadFString();
    }

    public class FStructProperty : UProperty
    {
        internal string _structName;
        private string _structGuid;

        protected internal override void PreDeserializeProperty(UBinaryReader reader)
        {
            _structName = reader.ReadFString();
            _structGuid = reader.ReadGuid();
        }

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            if (_structName == null || !UTypes.HasPropertyName(_structName))
            {
                Value = reader.ReadProperties();
            }
            else
            {
                UProperty property = UTypes.GetPropertyByName(_structName);
                property.Deserialize(reader);

                Value = property;
            }
        }
    }

    public class FTextProperty : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => base.DeserializeProperty(reader);
    }

    public class FUInt32Property : UProperty
    {
        protected internal override void DeserializeProperty(UBinaryReader reader) => Value = reader.ReadUInt32();
    }

    public class FVector2D : UStruct
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();

            Value = $"X: {X}, Y: {Y}";
        }
    }

    public class FVector3D : UStruct
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();

            Value = $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }

    public class FSetProperty : UProperty
    {
        private string _innerType;

        protected int NumKeysToRemove { get; set; }

        protected internal override void PreDeserializeProperty(UBinaryReader reader) => _innerType = reader.ReadFString();

        protected internal override void DeserializeProperty(UBinaryReader reader)
        {
            List<UProperty> items = new List<UProperty>();

            NumKeysToRemove = reader.ReadInt32();

            int count = reader.ReadInt32();

            for (int i = 0; i < NumKeysToRemove; i++)
            {
                UProperty property = UTypes.GetPropertyByName(_innerType);
                property.DeserializeProperty(reader);
            }

            for (int i = 0; i < count; i++)
            {
                UProperty property = UTypes.GetPropertyByName(_innerType);
                property.DeserializeProperty(reader);

                items.Add(property);
            }

            Value = items;
        }
    }
}
