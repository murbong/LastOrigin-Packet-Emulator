

namespace Pcap.Object
{
    public class SkillLevelupInfo : ITCPReadable
    {

        public string SkillString { get; set; }
        public uint BeforeExp { get; set; }
        public uint AfterExp { get; set; }
        public uint OffsetExp { get; set; }
        public byte IsLevelUp { get; set; }
        public byte BeforeLevel { get; set; }
        public byte AfterLevel { get; set; }

        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;

            SkillString = br.ReadString();
            BeforeExp = br.ReadUInt32();
            AfterExp = br.ReadUInt32();
            OffsetExp = br.ReadUInt32();
            IsLevelUp = br.ReadByte();
            BeforeLevel = br.ReadByte();
            AfterLevel = br.ReadByte();
        }
    }
}