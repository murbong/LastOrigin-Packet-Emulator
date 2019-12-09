namespace Pcap.Object
{
    public class MonsterEnterInfo : EnterInfo, ITCPReadable
    {
        public string MobGroupString { get; set; }
        public byte WaveStep { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            InstanceID = br.ReadUInt64();
            Name = br.ReadString();
            Grade = br.ReadByte();
            Level = br.ReadByte();
            MaxHP = br.ReadInt32();
            MaxAP = br.ReadUInt16();
            HP = br.ReadInt32();
            AP = br.ReadUInt16();
            GridPosIndex = br.ReadUInt16();
            TeamId = br.ReadByte();
            MobGroupString = br.ReadString();
            WaveStep = br.ReadByte();
        }
    }
}
