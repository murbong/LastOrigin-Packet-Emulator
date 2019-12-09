namespace Pcap.Object
{
    public class SkinInfo : ITCPReadable
    {
        public string SkinString { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            SkinString = br.ReadString();
        }

    }
}