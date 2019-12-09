namespace Pcap.Object
{
    public class CharMaxEnchant : ITCPReadable
    {
        public int MaxAtkValue { get; set; }
        public double MaxAtkExp { get; set; }
        public int MaxDefValue { get; set; }
        public double MaxDefExp { get; set; }
        public int MaxHPValue { get; set; }
        public double MaxHPExp { get; set; }
        public int MaxAccValue { get; set; }
        public double MaxAccExp { get; set; }
        public int MaxEvadeValue { get; set; }
        public double MaxEvadeExp { get; set; }
        public int MaxCriValue { get; set; }
        public double MaxCriExp { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null)
            {
                return;
            }

            MaxAtkValue = br.ReadInt32();
            MaxAtkExp = br.ReadDouble();
            MaxDefValue = br.ReadInt32();
            MaxDefExp = br.ReadDouble();
            MaxHPValue = br.ReadInt32();
            MaxHPExp = br.ReadDouble();
            MaxAccValue = br.ReadInt32();
            MaxAccExp = br.ReadDouble();
            MaxEvadeValue = br.ReadInt32();
            MaxEvadeExp = br.ReadDouble();
            MaxCriValue = br.ReadInt32();
            MaxCriExp = br.ReadDouble();
        }

    }
}