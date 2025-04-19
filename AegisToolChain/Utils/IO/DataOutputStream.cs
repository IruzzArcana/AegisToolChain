using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegisToolChain.Utils.IO
{
    internal class DataOutputStream : IDisposable
    {
        private readonly BinaryWriter _writer;
        private readonly MemoryStream _stream;

        public DataOutputStream()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        public void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _writer.Write(bytes);
        }

        public void WriteString(string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = encoding.GetBytes(value);
            _writer.Write(bytes);
        }
        public static Encoding ShiftJISEncoding => Encoding.GetEncoding("shift_jis");
        public void WriteByte(sbyte value)
        {
            _writer.Write(value);
        }

        public void WriteBoolean(bool value)
        {
            _writer.Write(value);
        }

        public long Position()
        {
            return _stream.Position;
        }

        public long Length()
        {
            return _stream.Length;
        }

        public byte[] Bytes()
        {
            _writer.Flush();
            return _stream.ToArray();
        }

        public bool IsEOF => _stream.Position >= _stream.Length;
        public void Close()
        {
            _writer.Close();
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _stream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
