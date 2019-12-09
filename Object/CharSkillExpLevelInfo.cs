using System.Collections.Generic;

namespace Pcap.Object
{
    public class CharSkillExpLevelInfo : ITCPReadable
    {
        public ulong CharID { get; set; }
        public List<SkillLevelupInfo> SkillInfo { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            CharID = br.ReadUInt64();
            SkillInfo = br.ReadList<SkillLevelupInfo>();
        }
    }
}