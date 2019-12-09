namespace Pcap.Object
{
    public class CharEnchantInfo : ITCPReadable

    {
        public uint EnchantCount { get; set; }
        public double EnchantExp { get; set; }
        public byte Type { get; set; }
        public double Value { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            EnchantCount = br.ReadUInt32();
            EnchantExp = br.ReadDouble();
            Type = br.ReadByte();
            Value = br.ReadDouble();
        }
    }

}