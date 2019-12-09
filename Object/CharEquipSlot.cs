namespace Pcap.Object
{
    public class CharEquipSlot : ITCPReadable
    {
        public byte SlotNumber { get; set; }
        public ItemInfo Item { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            SlotNumber = br.ReadByte();
            Item.Read(br);
        }
    }
}