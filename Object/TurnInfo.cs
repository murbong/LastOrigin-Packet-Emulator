namespace Pcap.Object
{
    public class TurnInfo : ITCPReadable
    {
        public ulong Aid { get; set; }
        public byte TeamId { get; set; }
        public double PreTurnPos { get; set; }
        public double TurnPos { get; set; }
        public int Index { get; set; }
        public int ReserveRound { get; set; }
        public byte IsReserveSkill { get; set; }
        public string ReserveSkillString { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            Aid = br.ReadUInt64();
            TeamId = br.ReadByte();
            PreTurnPos = br.ReadDouble();
            TurnPos = br.ReadDouble();
            Index = br.ReadInt32();
            ReserveRound = br.ReadInt32();
            IsReserveSkill = br.ReadByte();
            ReserveSkillString = br.ReadString();
        }
    }
}
