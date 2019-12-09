namespace Pcap.Object
{
    public class MailItemInfo : ITCPReadable
    {
        public string ItemString { get; set; }
        public int ItemCount { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            ItemString = br.ReadString();
            ItemCount = br.ReadInt32();
        }
    }
}