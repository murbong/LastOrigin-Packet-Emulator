using System.Collections.Generic;

namespace Pcap.Object
{

    public enum Grade
    {
        B = 2,
        A,
        S,
        SS
    }
    public class CharInfo : ITCPReadable
    {
        public ulong CharID { get; set; }
        public string Name { get; set; }
        public byte Grade { get; set; }
        public byte Level { get; set; }
        public uint CurEXP { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public ushort Atk { get; set; }
        public ushort Def { get; set; }
        public ushort Dodge { get; set; }
        public ushort TurnSpeed { get; set; }
        public ushort ResistFire { get; set; }
        public ushort ResistIce { get; set; }
        public ushort ResistElect { get; set; }
        public ulong CreateTime { get; set; }
        public List<SkillInfo> SkillList { get; set; }
        public List<CharEquipSlot> CharSlotList { get; set; }
        public ushort MaxEnchantCount { get; set; }
        public List<CharEnchantInfo> CharEnchantInfoList { get; set; }
        public CharMaxEnchant CharMaxEnchantInfo { get; set; }
        public byte IsFirstChar { get; set; }
        public byte IsUseCoreslotRef { get; set; }
        public byte IsLock { get; set; }
        public uint FavorPoint { get; set; }
        public ulong LastGiveFavorPointTime { get; set; }
        public string SkinName { get; set; }
        public List<SkinInfo> HaveSkinList { get; set; }
        public byte IsDestroyed { get; set; }
        public string LinkBonus { get; set; }
        public void Read(TCPBinaryReader br)
        {

            if (br == null)
            {
                return;
            }
            CharID = br.ReadUInt64();
            Name = br.ReadString();
            Grade = br.ReadByte();
            Level = br.ReadByte();
            CurEXP = br.ReadUInt32();
            HP = br.ReadInt32();
            MaxHP = br.ReadInt32();
            Atk = br.ReadUInt16();
            Def = br.ReadUInt16();
            Dodge = br.ReadUInt16();
            TurnSpeed = br.ReadUInt16();
            ResistFire = br.ReadUInt16();
            ResistIce = br.ReadUInt16();
            ResistElect = br.ReadUInt16();
            CreateTime = br.ReadUInt64();
            SkillList = br.ReadList<SkillInfo>();
            CharSlotList = br.ReadList<CharEquipSlot>();
            MaxEnchantCount = br.ReadUInt16();
            CharEnchantInfoList = br.ReadList<CharEnchantInfo>();
            CharMaxEnchantInfo = br.ReadObj<CharMaxEnchant>();
            IsFirstChar = br.ReadByte();
            IsUseCoreslotRef = br.ReadByte();
            IsLock = br.ReadByte();
            FavorPoint = br.ReadUInt32();
            LastGiveFavorPointTime = br.ReadUInt64();
            SkinName = br.ReadString();
            HaveSkinList = br.ReadList<SkinInfo>();
            IsDestroyed = br.ReadByte();
            LinkBonus = br.ReadString();
        }
    }
}
