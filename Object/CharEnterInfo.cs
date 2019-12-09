namespace Pcap.Object
{
    public class CharEnterInfo : EnterInfo, ITCPReadable
    {
        public ulong CharacterID { get; set; }
        public byte IsFriend { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            InstanceID = br.ReadUInt64();
            CharacterID = br.ReadUInt64();
            Name = br.ReadString();
            Grade = br.ReadByte();
            Level = br.ReadByte();
            MaxHP = br.ReadInt32();
            MaxAP = br.ReadUInt16();
            HP = br.ReadInt32();
            GridPosIndex = br.ReadUInt16();
            TeamId = br.ReadByte();
            IsFriend = br.ReadByte();
        }
    }
}