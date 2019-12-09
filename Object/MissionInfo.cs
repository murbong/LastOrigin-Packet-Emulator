using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcap.Object
{
    public class MissionInfo : ITCPReadable
    {
        public string MissionString { get; set; }
        public int MissionCount { get; set; }
        public byte SuccessYN { get; set; }
        public void Read(TCPBinaryReader br)
        {
            MissionString = br.ReadString();
            MissionCount = br.ReadInt32();
            SuccessYN = br.ReadByte();
        }
    }
}
