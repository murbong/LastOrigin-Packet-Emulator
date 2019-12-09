using Pcap.Object;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace Pcap
{
    public class PacketSniffer
    {
        private readonly IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
        private LivePacketDevice selectedDevice;
        private string LastPath = string.Empty;

        private readonly Dictionary<string, string> Headers = new Dictionary<string, string>();
        private readonly Dictionary<ulong, EnterInfo> LoadedMap = new Dictionary<ulong, EnterInfo>();
        private readonly Dictionary<ulong, EnterInfo> PIDMap = new Dictionary<ulong, EnterInfo>();
        private readonly List<GiveBuffInfo> BuffList = new List<GiveBuffInfo>();

        private byte CurrentWave = 0;

        private readonly Dictionary<string, int> RewardChar = new Dictionary<string, int>();
        private readonly StringBuilder chunkedBody = new StringBuilder();
        private class StreamBuffer
        {
            public const int maxSize = 16384;
            public byte[] stream = new byte[maxSize];
            public int size;
        }
        private readonly StreamBuffer streamBuffer = new StreamBuffer();

        #region Class Init
        public void Run()
        {
            if (selectedDevice != null)
            {
                using (PacketCommunicator communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    // Check the link layer. We support only Ethernet for simplicity.
                    if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                    {
                        CLI.PrintQueueLog(Channel.Zero, "이더넷 프로토콜에서만 작동합니다..", ConsoleColor.White);
                        return;
                    }
                    // Compile the filter
                    using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and tcp and (port 80 or portrange 8000-9000)"))
                    {
                        // Set the filter
                        communicator.SetFilter(filter);
                    }

                    CLI.PrintQueueLog(Channel.Zero, "패킷 읽기 시작!", ConsoleColor.Yellow);
                    CLI.ShowToast("PCAP", "통발 블랙박스", "패킷 파싱을 시작합니다.", "");

                    // start the capture
                    communicator.ReceivePackets(0, PacketHandler);

                }
            }
            else
            {
                CLI.PrintQueueLog(Channel.Zero, "장치가 선택되지 않았습니다.", ConsoleColor.White);
            }
        }
        public void Select()
        {
            if (allDevices.Count == 0)
            {
                CLI.PrintQueueLog(Channel.Zero, "장치를 찾을 수 없습니다.", ConsoleColor.White);
                return;
            }
            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                CLI.PrintQueueLog(Channel.Zero, (i + 1) + " (" + device.Description + ")", ConsoleColor.White);
            }

            int deviceIndex = 0;
            do
            {
                CLI.PrintQueueLog(Channel.Zero, "인터페이스 넘버 (1-" + allDevices.Count + "):", ConsoleColor.White);
                string deviceIndexString = Console.ReadLine();
                if (!int.TryParse(deviceIndexString, out deviceIndex) ||
                    deviceIndex < 1 || deviceIndex > allDevices.Count)
                {
                    deviceIndex = 0;
                }
            } while (deviceIndex == 0);

            // Take the selected adapter
            selectedDevice = allDevices[deviceIndex - 1];

        }
        #endregion

        private void ParseBuffList()
        {
            CLI.CleanQueue(Channel.Two);
            foreach (var i in LoadedMap)
            {
                var id = i.Value.InstanceID;
                var buff = BuffList.Where(x => x.ToID == id && (BuffType)x.AttrType != BuffType.MAX)
                    .OrderBy(x => x.AttrType).GroupBy(x => (BuffType)x.AttrType)
                    .Select(kv => new KeyValuePair<BuffType, float>(kv.Key, kv.Select(x => x.AttrValue).Sum())).ToList();
                var Builder = new StringBuilder();
                Builder.Append(i.Value.Name + "\t");
                buff.ForEach(buffPair =>
                {
                    Builder.Append(buffPair.Key + ":" + buffPair.Value * 100 + "%  ");
                });
                if (i.Value.TeamId == 0)
                {
                    CLI.PrintQueueLog(Channel.Two, Builder.ToString(), ConsoleColor.White, ConsoleColor.Black, false);
                }
                else
                {
                    var mob = i.Value as MonsterEnterInfo;
                    if (mob.WaveStep == CurrentWave)
                    {
                        CLI.PrintQueueLog(Channel.Two, Builder.ToString(), ConsoleColor.White, ConsoleColor.Black, false);
                    }
                }
            }
        }

        private void ParseBody(string json)
        {
            if (string.IsNullOrEmpty(json)) { return; }

            if (LastPath == "/currencyinfo")
            {

            }
            else if (LastPath == "/serverlist")
            {
                LOServerList.ParseServer(json);
            }
            else if (LastPath == "/getbattleserver")
            {
                LOServerList.ParserBattleServer(json);
            }
        }
        private void ParsePayload(byte[] memory, int size)
        {
            using (MemoryStream ms = new MemoryStream(memory, 0, size))
            using (TCPBinaryReader br = new TCPBinaryReader(ms))
            {
                TcpHeader header = br.ReadObj<TcpHeader>();

                HParam type = (HParam)header.PacketNumber;


                if (type == HParam.CharacterCreate)//캐릭터 생성 구간.
                {
                    CharInfo info = br.ReadObj<CharInfo>();
                    string name = Translate.GetTranslate(info.Name.Split('_')[2]);
                    byte grade = info.Grade;
                    ConsoleColor color = ConsoleColor.White;
                    if (grade >= 4)
                    {
                        color = ConsoleColor.Yellow;
                        CLI.ShowToast("PCAP", "캐릭터 획득", $"{(Grade)grade} 등급 {name} 획득했다!", "");
                    }
                    CLI.PrintQueueLog(Channel.One, $"{(Grade)grade} 등급 {name} 획득했다!", color);

                    if (RewardChar.ContainsKey(name) == true) RewardChar[name]++; else RewardChar[name] = 1;
                }
                else if (type == HParam.CheckNetwork || type == HParam.Ping || type == HParam.SetAuto || type == HParam.Packet || type == HParam.BattleLoginSuccess || type == HParam.BattleLogout || type == HParam.QUESTUPDATENTF || type == HParam.EnterStageSuccess)
                {
                    //무시해도 되는 부분.
                }
                else if (type == HParam.ChangeWave)//웨이브 클리어 했을때.
                {
                    BuffList.Clear();
                    byte NextWaveStep = br.ReadByte();
                    CurrentWave = NextWaveStep;
                    RewardInfo ClearRewardInfo = br.ReadObj<RewardInfo>();
                    List<CharEXPLevel> pcExpAndLevelupList = br.ReadList<CharEXPLevel>();
                    List<CharSkillExpLevelInfo> SkillExpAndLevelupList = br.ReadList<CharSkillExpLevelInfo>();
                    byte IsApplyOpenUI = br.ReadByte();
                    ulong OpenUI_ActorID = br.ReadUInt64();
                    byte WaveClearRank = br.ReadByte();
                    CLI.PrintQueueLog(Channel.One, $"다음 웨이브 : {NextWaveStep + 1} ", ConsoleColor.White);
                    ClearRewardInfo.RewardCharList.ForEach(element => { CLI.PrintQueueLog(Channel.One, element.CharString, ConsoleColor.Cyan); });
                    ClearRewardInfo.RewardItemList.ForEach(element => { CLI.PrintQueueLog(Channel.One, element.ItemString + " : " + element.StackCount, ConsoleColor.Cyan); });
                    pcExpAndLevelupList.ForEach(element =>
                    {
                        CLI.PrintQueueLog(Channel.One, PIDMap[element.CharID] + " Level UP " + element.BeforeLevel + " =>" + element.AfterLevel, ConsoleColor.Cyan);
                    });
                    SkillExpAndLevelupList.ForEach(element =>
                    {
                        element.SkillInfo.ForEach(skill => { if (skill.IsLevelUp == 1) { CLI.PrintQueueLog(Channel.One, PIDMap[element.CharID] + " : " + skill.SkillString + " SkillLevel UP " + skill.BeforeLevel + " =>" + skill.AfterLevel, ConsoleColor.Cyan); } });

                    });
                    CLI.PrintQueueLog(Channel.One, new string('─', CLI.channelGap), ConsoleColor.White, ConsoleColor.Black, false);
                }
                //else if (type == HParam.CharInfo){}
                else if (type == HParam.Exp) 
                {
                    var PID = br.ReadUInt64();
                    var Exp = br.ReadUInt32();
                    var Offset = br.ReadUInt32();

                    CLI.PrintQueueLog(Channel.One, $"{PIDMap[PID].Name} {Exp}경험치 ({Offset}상승)");
                }
                else if (type == HParam.LevelUp)
                {
                    var PID = br.ReadUInt64();
                    var Level = br.ReadByte();
                    var NextExp = br.ReadInt32();
                    var MaxEnchant = br.ReadUInt16();

                    CLI.PrintQueueLog(Channel.One, $"{PIDMap[PID].Name} {Level}으로 레벨업! 다음 경험치 : {NextExp}");
                }
                else if (type == HParam.FavorPoint)
                {
                    var ActorID = br.ReadUInt64();
                    var CurrentFavorPoint = br.ReadUInt32();
                    var OffSet = br.ReadInt32();

                    CLI.PrintQueueLog(Channel.One, $"{LoadedMap[ActorID].Name} 호감도 {CurrentFavorPoint/100.0} {OffSet/100.0} 상승");
                }
                else if (type == HParam.StageClear)
                {
                    foreach (var kv in RewardChar)
                    {
                        CLI.PrintQueueLog(Channel.Zero, $"{kv.Key} {kv.Value} 개 얻음.");
                    }
                }
                else if (type == HParam.StageClearUpdate)
                {
                    var UID = br.ReadUInt64();
                    var StageKeyString = br.ReadString();
                    var ClearCount = br.ReadInt32();
                    var MissionInfoList = br.ReadList<MissionInfo>();
                    CLI.CleanQueue(Channel.Zero);
                    CLI.PrintQueueLog(Channel.Zero, $"{StageKeyString}클리어! ({ClearCount}회 클리어됨.)");
                }
                else if (type == HParam.GiveBuffList)
                {
                    List<GiveBuffInfo> infoList = br.ReadList<GiveBuffInfo>();

                    // GUI.PrintQueueLog(Channel.Zero, new string('─', GUI.channelGap), ConsoleColor.White, false);

                    BuffList.AddRange(infoList);

                    /*infoList.ForEach(element =>
                    {
                        if ((BuffType)element.AttrType != BuffType.MAX)
                        {
                            GUI.PrintQueueLog(Channel.Zero, LoadedMap[element.FromID].Name + " Buffed " + LoadedMap[element.ToID].Name + " " +
                                (BuffType)element.AttrType + " : " + element.AttrValue * 100 + $"% ({element.LeftRoundCount})Left ", ConsoleColor.Yellow);
                        }
                    });*/

                    ParseBuffList();
                }
                else if (type == HParam.GiveBuff)
                {
                    GiveBuffInfo info = br.ReadObj<GiveBuffInfo>();

                    BuffList.Add(info);

                    //GUI.PrintQueueLog(Channel.Zero, new string('─', GUI.channelGap), ConsoleColor.White, false);

                    /*if ((BuffType)info.AttrType != BuffType.MAX)
                    {
                        GUI.PrintQueueLog(Channel.Zero, LoadedMap[info.FromID].Name + " Buffed " + LoadedMap[info.ToID].Name + " " +
                            (BuffType)info.AttrType + " : " + info.AttrValue * 100 + $"% ({info.LeftRoundCount})Left ", ConsoleColor.Yellow);
                    }*/
                    ParseBuffList();
                }
                else if (type == HParam.RemoveBuffList)
                {
                    List<RemoveBuffInfo> infoList = br.ReadList<RemoveBuffInfo>();

                    //GUI.PrintQueueLog(Channel.Zero, new string('─', GUI.channelGap), ConsoleColor.White, false);

                    infoList.ForEach(element =>
                    {
                        BuffList.Remove(BuffList.Find(buff => buff.BuffID == element.BuffID));
                        //GUI.PrintQueueLog(Channel.Zero, LoadedMap[element.FromID].Name + " Buff Removed " + LoadedMap[element.ToID].Name, ConsoleColor.Red);
                    });
                    ParseBuffList();
                }
                else if (type == HParam.RemoveBuff)
                {
                    RemoveBuffInfo info = br.ReadObj<RemoveBuffInfo>();
                    BuffList.Remove(BuffList.Find(buff => buff.BuffID == info.BuffID));
                }
                else if (type == HParam.Hp)
                {
                    ulong Trid = br.ReadUInt64();
                    ulong Aid = br.ReadUInt64();
                    int HP = br.ReadInt32();
                    int MaxHP = br.ReadInt32();
                    int Damage = br.ReadInt32();
                    ulong Aid_From = br.ReadUInt64();
                    byte DamageType = br.ReadByte();
                    byte BuffEffectType = br.ReadByte();
                    byte IsCri = br.ReadByte();

                    CLI.PrintQueueLog(Channel.One, $"{LoadedMap[Aid_From].Name} 피해 입힘 => {LoadedMap[Aid].Name} Dmg : {Damage}{(IsCri == 1 ? "치명타!" : "")}", IsCri == 1 ? ConsoleColor.Red : ConsoleColor.White);
                    if (HP <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (((double)HP / MaxHP) <= 0.25 && LoadedMap[Aid].TeamId == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (LoadedMap[Aid].TeamId == 1)//적군
                    {
                        MonsterEnterInfo mob = LoadedMap[Aid] as MonsterEnterInfo;
                        byte wave = mob.WaveStep;
                        var xPos = CLI.tableWidth * (wave + 1) + CLI.alt + CLI.gap * wave;
                        CLI.RemoveCharacter(xPos, 0, mob.GridPosIndex, 0, 1);
                        CLI.PrintCharacter(xPos, 0, mob.GridPosIndex, HP + " / " + MaxHP, 0, 1);
                    }
                    else
                    {
                        CharEnterInfo pc = LoadedMap[Aid] as CharEnterInfo;
                        CLI.RemoveCharacter(0, 0, pc.GridPosIndex, 0, 1);
                        CLI.PrintCharacter(0, 0, pc.GridPosIndex, HP + " / " + MaxHP, 0, 1);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (type == HParam.AttackTarget)
                {
                    List<AttackTargetInfo> AttackTargetList = br.ReadList<AttackTargetInfo>();
                    byte IsNotRemove = br.ReadByte();
                    byte TargetGridPos = br.ReadByte();
                }
                else if (type == HParam.StartTurn)
                {
                    byte CurrentRound = br.ReadByte();
                    int CurrentGlobalTurn = br.ReadInt32();
                    List<TurnInfo> TurnList = br.ReadList<TurnInfo>();
                    List<TurnInfo> DisplayTurnList = br.ReadList<TurnInfo>();//실제로 보이는 행동력 리스트
                    byte IsSkillSelectTurn = br.ReadByte();
                    byte isReconnect = br.ReadByte();

                    DisplayTurnList.ForEach(element =>
                    {
                        EnterInfo info = LoadedMap[element.Aid];

                        double prev = element.PreTurnPos;
                        double turn = element.TurnPos;

                        int x = 0;
                        int grid = 0;

                        //GUI.PrintQueueLog($"{prev} {turn}", ConsoleColor.Blue);
                        Console.ForegroundColor = ConsoleColor.White;
                        if (turn < 10)
                            Console.ForegroundColor = ConsoleColor.Red;
                        else if (turn >= 20)
                            Console.ForegroundColor = ConsoleColor.Blue;

                        if (info.TeamId == 1)//적군
                        {
                            MonsterEnterInfo mob = info as MonsterEnterInfo;
                            byte wave = mob.WaveStep;
                            x = CLI.tableWidth * (wave + 1) + CLI.alt + CLI.gap * wave;
                            grid = mob.GridPosIndex;

                        }
                        else if (info.TeamId == 0)
                        {
                            grid = info.GridPosIndex;
                        }
                        CLI.RemoveCharacter(x, 0, grid, 0, -1);
                        CLI.PrintCharacter(x, 0, grid, turn.ToString("0.0AP"), 0, -1);
                        Console.ForegroundColor = ConsoleColor.White;
                    });
                }
                else if (type == HParam.ItemCreate)
                {
                    byte UpdateType = br.ReadByte();
                    ItemInfo Info = br.ReadObj<ItemInfo>();
                    CLI.PrintQueueLog(Channel.One, $"{Info.ItemString} 획득했다! 현재 {Info.StackCount}개 있음.", ConsoleColor.White);
                }
                else if (type == HParam.LoadResource)
                {
                    string StageKeyString = br.ReadString();
                    CurrentWave = 0;
                    List<CharEnterInfo> PCEnterInfoList = br.ReadList<CharEnterInfo>();
                    List<MonsterEnterInfo> MonsterEnterInfoList = br.ReadList<MonsterEnterInfo>();

                    LoadedMap.Clear();
                    PIDMap.Clear();

                    CLI.RemoveRange(0, 0, CLI.tableWidth / 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    CLI.PrintGrid(0, 0);

                    CLI.PrintQueueLog(Channel.One, $"스테이지 : {StageKeyString} 입장.", ConsoleColor.White);
                    CLI.PrintQueueLog(Channel.One, new string('─', CLI.channelGap), ConsoleColor.White, ConsoleColor.Black, false);
                    PCEnterInfoList.ForEach(element =>
                    {
                        string name = Translate.GetTranslate(element.Name.Split('_')[2]);

                        element.Name = name;
                        PIDMap[element.CharacterID] = element;
                        LoadedMap[element.InstanceID] = element;
                        //GUI.PrintQueueLog(Channel.Zero, $"Char ID : {element.InstanceID} Name: {name} Added { element.GridPosIndex} Grid", ConsoleColor.Cyan);

                        Console.ForegroundColor = ConsoleColor.White;

                        CLI.PrintCharacter(0, 0, element.GridPosIndex, name);
                        if (((double)element.HP / element.MaxHP) <= 0.25)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        CLI.PrintCharacter(0, 0, element.GridPosIndex, element.HP + " / " + element.MaxHP, 0, 1);
                        Console.ForegroundColor = ConsoleColor.White;

                        CLI.PrintCharacter(0, 0, element.GridPosIndex, element.MaxAP.ToString(), 0, -1);
                        CLI.PrintCharacter(0, 0, element.GridPosIndex, (Grade)element.Grade + " " + element.Level, 0, -2);

                    });
                    int lastWave = -1;

                    List<(string, int)> SameName = new List<(string, int)>();
                    Dictionary<(string, int), int> SameIndex = new Dictionary<(string, int), int>();

                    MonsterEnterInfoList.ForEach(element =>
                    {
                        SameName.Add((element.Name, element.WaveStep));
                        SameIndex[(element.Name, element.WaveStep)] = 0;
                    }
                    );
                    MonsterEnterInfoList.ForEach(element =>
                    {
                        string name = Translate.GetTranslate(element.Name.Split('_')[2]);
                        string Tier = element.Name.Split('_')[3];
                        if (SameName.FindAll(ind => (element.Name == ind.Item1 && element.WaveStep == ind.Item2)).Count > 1)
                        {
                            name += $"({(char)('A' + SameIndex[(element.Name, element.WaveStep)])})";
                            SameIndex[(element.Name, element.WaveStep)]++;
                        }

                        element.Name = name;
                        LoadedMap[element.InstanceID] = element;
                        //GUI.PrintQueueLog(Channel.Zero, $"Mob ID : {element.InstanceID} Name: {name} Added {element.GridPosIndex} Grid {element.WaveStep} Wave", ConsoleColor.Magenta);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        byte wave = element.WaveStep;
                        var xPos = 0;
                        if (lastWave != wave)
                        {
                            xPos = CLI.tableWidth * (wave + 1) + CLI.alt + CLI.gap * wave;
                            CLI.PrintGrid(xPos, 0);
                            lastWave = wave;
                        }
                        xPos = CLI.tableWidth * (wave + 1) + CLI.alt + CLI.gap * wave;
                        CLI.PrintCharacter(xPos, 0, element.GridPosIndex, name);
                        CLI.PrintCharacter(xPos, 0, element.GridPosIndex, element.HP + " / " + element.MaxHP, 0, 1);
                        CLI.PrintCharacter(xPos, 0, element.GridPosIndex, element.AP + " => " + element.MaxAP, 0, -1);
                        CLI.PrintCharacter(xPos, 0, element.GridPosIndex, (Grade)element.Grade + " " + element.Level, 0, -2);
                        CLI.PrintCharacter(xPos, 0, element.GridPosIndex, Tier, 0, 2);
                    });
                }
                else if (type == HParam.CHANGEROUND)
                {

                }
                else
                {
                    CLI.PrintQueueLog(Channel.Zero, string.Format("LEN : {0}, NUM : {1}, FLAG : {2} Enum : {3}", header.Length, header.PacketNumber, header.Flags, ((HParam)header.PacketNumber).ToString()), ConsoleColor.Yellow, ConsoleColor.Red, false);
                }

            }

        }
        private bool ChunkBodyAppend(string body)
        {
            if (Headers.ContainsKey("Transfer-Encoding") && Headers["Transfer-Encoding"].Equals("chunked", StringComparison.Ordinal))
            {
                string[] split = body.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (string str in split)
                {
                    if (str.Equals("0", StringComparison.Ordinal))
                    {
                        ParseBody(chunkedBody.ToString());
                    }
                    else if (int.TryParse(str, NumberStyles.HexNumber, null, out int result))
                    {
                        //Console.WriteLine(result);
                    }
                    else
                    {
                        chunkedBody.Append(str);
                    }
                }
                return true;
            }
            return false;
        }

        private void OnRequest(HttpDatagram http)
        {
            HttpRequestDatagram req = http as HttpRequestDatagram;
            string body = req.Body.Decode(Encoding.UTF8);
            //    Console.WriteLine("{0} {1} {2}", req.Method.Method, req.Uri, req.Version);
            LastPath = req.Uri;
            using (IEnumerator<HttpField> k = req.Header.GetEnumerator())
            {
                Headers.Clear();
                while (k.MoveNext())
                {
                    Headers[k.Current.Name] = k.Current.ValueString;
                }
            }

            //Console.WriteLine(body);
        }
        private void OnResponse(HttpDatagram http)
        {
            HttpResponseDatagram res = http as HttpResponseDatagram;
            //Console.WriteLine(LastPath);
            string body = res.Body.Decode(Encoding.UTF8);

            using (IEnumerator<HttpField> k = res.Header.GetEnumerator())
            {
                Headers.Clear();
                while (k.MoveNext())
                {
                    Headers[k.Current.Name] = k.Current.ValueString;
                }
            }
            bool isChunk = ChunkBodyAppend(body);
            if (isChunk == false)
            {
                ParseBody(body);
            }
            //LastStatus = (uint)res.StatusCode;
        }
        private void TcpAssembly(byte[] stream, int size)
        {
            if (size > 0)
            {
                stream.BlockCopy(0, streamBuffer.stream, streamBuffer.size, size);
                streamBuffer.size += size;
                do
                {
                    var Check = ParseAssembly(streamBuffer.stream, streamBuffer.size);
                    if (Check == 0)//1460
                    {
                        return;
                    }
                    if (Check < 0 || Check > streamBuffer.size)//별난놈
                    {
                        streamBuffer.size = 0;
                        return;
                    }
                    streamBuffer.stream.BlockCopy(Check, streamBuffer.stream, 0, streamBuffer.size - Check);
                    streamBuffer.size -= Check;
                }
                while (streamBuffer.size != 0);// 패킷헤더가 여러개가 한꺼번에 들어오는걸 처리함.
            }
        }
        private int ParseAssembly(byte[] stream, int size)
        {
            if (size < 2)//걸러
            {
                return 0;
            }
            using (MemoryStream input = new MemoryStream(stream))
            using (BinaryReader bin = new BinaryReader(input))
            {
                int headLen = bin.ReadInt32();
                if (headLen < sizeof(long) || headLen > sizeof(long) * 1024)//걸르자.
                {
                    return -1;

                }
                if (size < headLen)//분할되서 날라올때.
                {
                    return 0;
                }
                ParsePayload(stream, headLen);
                return headLen;
            }
        }
        private void PacketHandler(Packet packet)
        {
            IpV4Datagram ip = packet.Ethernet.IpV4;
            TcpDatagram tcp = ip.Tcp;
            HttpDatagram http = tcp.Http;
            ServerType serverType = LOServerList.GetServerType(ip.Source.ToString(), ip.Destination.ToString());
            if (serverType == ServerType.Error || tcp.Payload.Length <= 0)
            {
                return;
            }
            if (serverType == ServerType.Battle)
            {
                if (LOServerList.GetServerType(ip.Source.ToString()) == ServerType.Battle) // only response
                {
                    TcpAssembly(tcp.Payload.ToMemoryStream().ToArray(), tcp.PayloadLength);
                }
            }
            else if (serverType != ServerType.Error && !string.IsNullOrEmpty(http.Decode(Encoding.UTF8)))
            {
                if (http.Header != null)
                { // 헤더가 있을 때
                    chunkedBody.Clear();
                    if (http.IsRequest) // Request
                    {
                        //  Console.WriteLine("REQUEST");
                        OnRequest(http);
                    }
                    else if (http.IsResponse)
                    {
                        //  Console.WriteLine("RESPONSE");
                        OnResponse(http);
                    }
                }
                else//헤더가 없을 때 행동 Chunk, Continue 등등..
                {
                    bool isChunk = ChunkBodyAppend(http.Decode(Encoding.UTF8));

                    if (isChunk == false)
                    {
                        //  Console.WriteLine(http.Decode(Encoding.UTF8));
                    }

                }
                //Console.WriteLine(http.Decode(Encoding.UTF8));
            }
            //Console.WriteLine("{0} : {1} => {2} : {3}", ip.Source, tcp.SourcePort, ip.Destination, tcp.DestinationPort);
            // Console.WriteLine("------------------------------\n");

        }
    }
}
