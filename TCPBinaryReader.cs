using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Pcap.Object
{
    public struct Header
    {

    }
    public class TCPBinaryReader : BinaryReader
    {
        public TCPBinaryReader(Stream input) : base(input)
        {

        }
        public TCPBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {

        }
        public override string ReadString()
        {
            ushort cnt = ReadUInt16();
            byte[] buffer = ReadBytes(cnt);
            ushort _ = ReadUInt16();
            return Encoding.Unicode.GetString(buffer);
        }
        public T ReadObj<T>() where T : ITCPReadable, new()
        {
            T obj = new T();
            obj.Read(this);
            return obj;
        }
        public List<T> ReadList<T>() where T : ITCPReadable, new()
        {
            List<T> list = new List<T>();
            ushort cnt = ReadUInt16();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(ReadObj<T>());
            }
            return list;
        }
        public List<uint> ReadUIntList()
        {
            List<uint> list = new List<uint>();
            ushort cnt = ReadUInt16();
            for (int i = 0; i < cnt; i++)
            {
                uint item = ReadUInt32();
                list.Add(item);
            }
            return list;
        }
        public List<ulong> ReadUlongList()
        {
            List<ulong> list = new List<ulong>();
            ushort cnt = ReadUInt16();
            for (int i = 0; i < cnt; i++)
            {
                ulong item = ReadUInt64();
                list.Add(item);
            }
            return list;
        }
        public List<int> ReadIntList()
        {
            List<int> list = new List<int>();
            ushort cnt = ReadUInt16();
            for (int i = 0; i < cnt; i++)
            {
                int item = ReadInt32();
                list.Add(item);
            }
            return list;
        }

    }
}
