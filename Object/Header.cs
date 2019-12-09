namespace Pcap.Object
{
    public class TcpHeader : ITCPReadable
    {
        public int Length { get; set; }
        public ushort PacketNumber { get; set; }
        public ushort Flags { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null)
            {
                return;
            }

            Length = br.ReadInt32();
            PacketNumber = br.ReadUInt16();
            Flags = br.ReadUInt16();
        }
    }
}
