namespace Pcap.Object
{
    public class CharEXPLevel : ITCPReadable
    {
        public ulong CharID { get; set; }
        public int BeforeExp { get; set; }
        public int AfterExp { get; set; }
        public int OffsetExp { get; set; }
        public byte IsLevelUp { get; set; }
        public byte BeforeLevel { get; set; }
        public byte AfterLevel { get; set; }
        public ushort BeforeHP { get; set; }
        public ushort AfterHP { get; set; }

        // Token: 0x06003D98 RID: 15768 RVA: 0x0012659C File Offset: 0x0012499C
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            CharID = br.ReadUInt64();
            BeforeExp = br.ReadInt32();
            AfterExp = br.ReadInt32();
            OffsetExp = br.ReadInt32();
            IsLevelUp = br.ReadByte();
            BeforeLevel = br.ReadByte();
            AfterLevel = br.ReadByte();
            BeforeHP = br.ReadUInt16();
            AfterHP = br.ReadUInt16();
        }
    }
}