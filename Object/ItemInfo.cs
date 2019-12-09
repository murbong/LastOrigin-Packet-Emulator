namespace Pcap.Object
{
    public class ItemInfo : ITCPReadable
    {
        public ulong ItemUID { get; set; }
        public ulong ItemSN { get; set; }
        public byte ItemType { get; set; }
        public string ItemString { get; set; }
        public int StackCount { get; set; }
        public byte InvenCategory { get; set; }
        public int InvenPos { get; set; }
        public byte EnchantLevel { get; set; }
        public byte IsLock { get; set; }
        public int EnchantPoint { get; set; }
        public ulong EquippedCharID { get; set; }
        public byte EquipSlot { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            ItemUID = br.ReadUInt64();
            ItemSN = br.ReadUInt64();
            ItemType = br.ReadByte();
            ItemString = br.ReadString();
            StackCount = br.ReadInt32();
            InvenCategory = br.ReadByte();
            InvenPos = br.ReadInt32();
            EnchantLevel = br.ReadByte();
            IsLock = br.ReadByte();
            EnchantPoint = br.ReadInt32();
            EquippedCharID = br.ReadUInt64();
            EquipSlot = br.ReadByte();
        }
    }
}