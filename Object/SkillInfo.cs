
namespace Pcap.Object
{
    public class SkillInfo : ITCPReadable
    {
        public string SkillString { get; set; }
        public byte SkillLevel { get; set; }
        public uint SkillExp { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;

            SkillString = br.ReadString();
            SkillLevel = br.ReadByte();
            SkillExp = br.ReadUInt32();
        }
    }
}