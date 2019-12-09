namespace Pcap.Object
{
    public class EnterInfo
    {
        public ulong InstanceID { get; set; }
        public string Name { get; set; }
        public byte Grade { get; set; }
        public byte Level { get; set; }
        public int MaxHP { get; set; }
        public ushort MaxAP { get; set; }
        public int HP { get; set; }
        public ushort AP { get; set; }
        public ushort GridPosIndex { get; set; }
        public byte TeamId { get; set; }
    }
}
