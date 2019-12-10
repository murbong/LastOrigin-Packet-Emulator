using System.Collections.Generic;


namespace Pcap.Object
{
    public class AttackTargetInfo : ITCPReadable
    {
        public ulong AttackID { get; set; }
        public List<ulong> TargetAidList { get; set; }
        public List<int> TargetGridList { get; set; }
        public string AttackerSkillString { get; set; }
        public byte IsCri { get; set; }
        public List<SpecialSkillInfo> SpecialSkillList { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            AttackID = br.ReadUInt64();
            TargetAidList = br.ReadUlongList();
            TargetGridList = br.ReadIntList();
            AttackerSkillString = br.ReadString();
            IsCri = br.ReadByte();
            SpecialSkillList = br.ReadList<SpecialSkillInfo>(); 
        }
    }


}