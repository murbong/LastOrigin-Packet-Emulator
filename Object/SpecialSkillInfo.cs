namespace Pcap.Object
{
    public class SpecialSkillInfo : ITCPReadable
    {
        public string SkillString { get; set; }
        public byte Type { get; set; }
        public double Value { get; set; }
        public ulong TargetAid { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null)return;
            SkillString = br.ReadString();
            Type = br.ReadByte();
            Value = br.ReadDouble();
            TargetAid = br.ReadUInt64();
        }
    }
}