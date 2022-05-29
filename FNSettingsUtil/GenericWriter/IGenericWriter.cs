using System.Text;

namespace GenericWriter
{
    public interface IGenericWriter : IDisposable
    {
        public long Position { get; set; }
        public int Size { get; }
        public int Seek(int offset, SeekOrigin origin);
        public void Write<T>(T value) where T : unmanaged;
        public void Write<T>(T value, int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged;
        public void WriteBoolean(bool value);
        public void WriteBoolean(bool value, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteByte(byte value);
        public void WriteByte(byte value, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteBytes(byte[] value);
        public void WriteBytes(byte[] value, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteString(string value, Encoding enc);
        public void WriteString(string value, Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current);
        //public void WriteString(string value, int length, Encoding enc);
        //public void WriteString(string value, int length, Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteFString(string value);
        public void WriteFString(string value, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteFStringArray(string[] value);
        public void WriteFStringArray(string[] value, int offset, SeekOrigin origin);
        /*public void WriteFStringArray(string[] value, int length);
        public void WriteFStringArray(string[] value, int length, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteArray<T>() where T : unmanaged;
        public void WriteArray<T>(T[] value, int offset, SeekOrigin origin) where T : unmanaged;
        public void WriteArray<T>(T[] value, int length) where T : unmanaged;
        public void WriteArray<T>(T[] value, int length, int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged;
        public void WriteArray<T>(T[] value, Func<T> getter);
        public void WriteArray<T>(T[] value, Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteArray<T>(T[] value, Func<IGenericWriter, T> getter);
        public void WriteArray<T>(T[] value, Func<IGenericWriter, T> getter, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteArray<T>(T[] value, int length, Func<T> getter);
        public void WriteArray<T>(T[] value, int length, Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current);
        public void WriteArray<T>(T[] value, int length, Func<IGenericWriter, T> getter);
        public void WriteArray<T>(T[] value, int length, Func<IGenericWriter, T> getter, int offset, SeekOrigin origin = SeekOrigin.Current);*/
    }
}
