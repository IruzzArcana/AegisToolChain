using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegisToolChain.Utils.IO
{
    using System;
    using System.IO;
    using System.Text;

    class DataInputStream : IDisposable
    {
        private readonly BinaryReader _reader;
        private readonly MemoryStream _stream;

        public DataInputStream(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _stream = new MemoryStream(data);
            _reader = new BinaryReader(_stream);
        }

        public int ReadInt()
        {
            var bytes = _reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        public short ReadShort()
        {
            var bytes = _reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        public long ReadLong()
        {
            var bytes = _reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt64(bytes, 0);
        }

        public float ReadFloat()
        {
            var bytes = _reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToSingle(bytes, 0);
        }

        public double ReadDouble()
        {
            var bytes = _reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToDouble(bytes, 0);
        }

        public string ReadString(int length, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = _reader.ReadBytes(length);
            return encoding.GetString(bytes);
        }
        public static Encoding ShiftJISEncoding => Encoding.GetEncoding("shift_jis");
        public sbyte ReadByte()
        {
            return (sbyte)_reader.ReadByte();
        }

        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }

        public long Position()
        {
            return _stream.Position;
        }

        public long Length()
        {
            return _stream.Length;
        }

        public bool IsEOF => _stream.Position >= _stream.Length;
        public void Close()
        {
            _reader.Close();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _stream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
