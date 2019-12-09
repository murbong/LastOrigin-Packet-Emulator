using Pcap.Object;
using System.Collections.Generic;

namespace Pcap
{
    public class RewardInfo : ITCPReadable
    {
        public RewardInfo()
        {
            Exp = 0u;
            AccountExp = 0u;
            Coin = 0u;
            Cash = 0u;
            Metal = 0u;
            Nutrient = 0u;
            Power = 0u;
            RewardCharList = new List<RewardCharInfo>();
            RewardItemList = new List<ItemInfo>();
        }
        public uint Exp { get; set; }
        public uint AccountExp { get; set; }
        public uint Coin { get; set; }
        public uint Cash { get; set; }
        public uint Metal { get; set; }
        public uint Nutrient { get; set; }
        public uint Power { get; set; }
        public List<RewardCharInfo> RewardCharList { get; set; }
        public List<ItemInfo> RewardItemList { get; set; }
        public void Read(TCPBinaryReader br)
        {
            if (br == null) return;
            Exp = br.ReadUInt32();
            AccountExp = br.ReadUInt32();
            Coin = br.ReadUInt32();
            Cash = br.ReadUInt32();
            Metal = br.ReadUInt32();
            Nutrient = br.ReadUInt32();
            Power = br.ReadUInt32();
            RewardCharList = br.ReadList<RewardCharInfo>();
            RewardItemList = br.ReadList<ItemInfo>();
        }
    }
}