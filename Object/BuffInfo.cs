namespace Pcap.Object
{
    public class GiveBuffInfo : ITCPReadable
    {
        public ulong FromID { get; set; }
        public ulong ToID { get; set; }
        public ulong BuffID { get; set; }
        public string BuffString { get; set; }
        public int RemainRound { get; set; }
        public string FromString { get; set; }
        public ushort AttrType { get; set; }
        public byte AttrValueType { get; set; }
        public float AttrValue { get; set; }
        public byte BuffIndex { get; set; }
        public byte _ { get; set; }
        public CharInfo CharInfo { get; set; }
        public void Read(TCPBinaryReader br)
        {
            FromID = br.ReadUInt64();
            ToID = br.ReadUInt64();
            BuffID = br.ReadUInt64();
            BuffString = br.ReadString();
            RemainRound = br.ReadInt32();
            FromString = br.ReadString();
            AttrType = br.ReadUInt16();
            AttrValueType = br.ReadByte();
            AttrValue = br.ReadSingle();
            BuffIndex = br.ReadByte();
            _ = br.ReadByte();//나도몰라
            CharInfo = br.ReadObj<CharInfo>();
        }
    }
    public class RemoveBuffInfo : ITCPReadable
    {
        public ulong FromID { get; set; }
        public ulong ToID { get; set; }
        public ulong BuffID { get; set; }
        public byte IsTransform { get; set; }
        public CharInfo CharInfo { get; set; }

        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            FromID = br.ReadUInt64();
            ToID = br.ReadUInt64();
            BuffID = br.ReadUInt64();
            IsTransform = br.ReadByte();
            CharInfo = br.ReadObj<CharInfo>();
        }
    }
}

