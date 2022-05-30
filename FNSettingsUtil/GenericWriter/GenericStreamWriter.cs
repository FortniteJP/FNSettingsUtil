using System.Runtime.CompilerServices;
using System.Text;

namespace GenericWriter
{
    public unsafe class GenericStreamWriter : IGenericWriter
    {
        private readonly MemoryStream _stream;

        public GenericStreamWriter(MemoryStream stream)
        {
            _stream = stream;
            Size = (int)_stream.Length;
        }

        public int Size { get; }

        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Seek(int offset, SeekOrigin origin)
        {
            return (int)_stream.Seek(offset, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value) where T : unmanaged
        {
            var size = sizeof(T);
            var buffer = new byte[size];
            Unsafe.WriteUnaligned<T>(ref buffer[0], value);
            _stream.Write(buffer, 0, size);
            Console.WriteLine("a:"+_stream.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value, int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged
        {
            var size = sizeof(T);
            var buffer = new byte[size];
            Unsafe.WriteUnaligned<T>(ref buffer[0], value);
            _stream.Write(buffer, 0, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value)
        {
            Write<bool>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Write<bool>(value, offset, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            WriteBytes(new byte[1] { value });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            WriteBytes(new byte[1] { value }, offset, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte[] value, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Seek(offset, origin);
            WriteBytes(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Write<int>(value.Length, offset, origin);
            WriteString(value, enc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, Encoding enc)
        {
            Write<int>(value.Length);
            WriteBytes(enc.GetBytes(value));
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            WriteBytes(enc.GetBytes(value), offset, origin);
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFString(string value)
        {
            // > 0 for ANSICHAR, < 0 for UCS2CHAR serialization
            Write<int>(value.Length);

            if (value.Length == 0)
            {
                return;
            }
            _stream.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
            Console.WriteLine("b:"+_stream.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFString(string value, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Seek(offset, origin);
            WriteFString(value);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFStringArray(string[] value)
        {
            Write<int>(value.Length);
            WriteFStringArray(value);
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFStringArray(string[] value, int offset, SeekOrigin origin)
        {
            Write<int>(value.Length, offset, origin);
            WriteFStringArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFStringArray(string[] value)
        {
            foreach (var item in value)
            {
                WriteFString(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFStringArray(string[] value, int length, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Seek(offset, origin);
            WriteFStringArray(value);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(T[] value) where T : unmanaged
        {
            Write<int>(value.Length);
            return WriteArray<T>(value);
        }*/

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(T[] value, int offset, SeekOrigin origin) where T : unmanaged
        {
            Write<int>(value.Length, offset, origin);
            WriteArray<T>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(T[] value) where T : unmanaged
        {
            if (value.Length == 0)
            {
                return;
            }

            var size = value.Length * sizeof(T);
            var buffer = _stream.GetBuffer();
            var result = new T[value.Length];
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, byte>(ref result[0]), ref buffer[0], (uint)size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(T[] value, int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged
        {
            Seek(offset, origin);
            return WriteArray<T>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(Func<T> getter)
        {
            var length = Write<int>();
            return WriteArray(length, getter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            var length = Write<int>(offset, origin);
            return WriteArray(length, getter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(Func<IGenericWriter, T> getter)
        {
            var length = Write<int>();
            return WriteArray(length, getter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(Func<IGenericWriter, T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            var length = Write<int>(offset, origin);
            return WriteArray(length, getter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(int length, Func<T> getter)
        {
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = getter();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(int length, Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Seek(offset, origin);
            return WriteArray(length, getter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(int length, Func<IGenericWriter, T> getter)
        {
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = getter(this);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] WriteArray<T>(int length, Func<IGenericWriter, T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            Seek(offset, origin);
            return WriteArray(length, getter);
        }*/

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
