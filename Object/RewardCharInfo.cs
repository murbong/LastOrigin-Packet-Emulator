namespace Pcap.Object
{
    public class RewardCharInfo : ITCPReadable
    {
        public string CharString { get; set; }
        public byte Grade { get; set; }
        public byte Level { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            CharString = br.ReadString();
            Grade = br.ReadByte();
            Level = br.ReadByte();
        }
    }
}