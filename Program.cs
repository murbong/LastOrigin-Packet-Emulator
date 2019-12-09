using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Pcap
{
    /// <summary>
    /// Basic capture example
    /// </summary>
    /// 

    public static class Program
    {
        public static void Main(string[] args)
        {

            

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.Title = "라스트오리진 통발 블랙박스";
            Console.CursorSize = 11;
            Translate.SetDictionary(Environment.CurrentDirectory+@"\trans.txt");



            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process p = Process.Start(psi);

            StreamWriter sw = p.StandardInput;
            StreamReader sr = p.StandardOutput;

            sw.WriteLine("Hello world!");
            sr.Close();

            string str = @"{""ErrorCode"":0,""Result"":[{""GateServerVersion"":""1"",""ServerName"":""LIVE01"",""LoginServerIP"":""LiveLoLoginServerALB-1349436694.ap-northeast-2.elb.amazonaws.com"",""LoginServerPort"":""80"",""GateServerIP"":""LiveLoGateServerALB-668344614.ap-northeast-2.elb.amazonaws.com"",""GateServerPort"":""80"",""RankServerIP"":""LiveLoGateServerELB-771478060.ap-northeast-2.elb.amazonaws.com"",""RankServerPort"":""8000"",""ChatServerIP"":""Live-LoGateServerELB-366742435.ap-northeast-2.elb.amazonaws.com"",""ChatServerPort"":""9000"",""OrderedNumber"":""1"",""IsVisible"":""1""}],""WaitingIndex"":0,""WaitingPeopleCount"":0,""FrontAccessToken"":""LoGateFrontAccessToken_63df1dbd29470191ebef3eb9cd95194f_22269062019-10-2200:20:11.785276482+0000UTCm=+337218.828358409"",""Sequence"":3}";
            LOServerList.ParseServer(str);
            PacketSniffer sniffer = new PacketSniffer();

            for (int i = 0; i < 3; i++)
            {
                CLI.PrintGrid(0 + CLI.tableWidth * (i + 1) + CLI.alt + CLI.gap * i, 0);
                for (int j = 0; j < 9; j++)
                {
                    CLI.PrintCharacter(0 + CLI.tableWidth * (i + 1) + CLI.alt + CLI.gap * i, 0, j, "김수한무거북이" + j);
                }
                CLI.RemoveCharacter(0 + CLI.tableWidth * (i + 1) + CLI.alt + CLI.gap * i, 0, 4);
            }

            for (int i = 0; i < CLI.logLimit * 2; i++)
            {
                CLI.PrintQueueLog(Channel.Zero, $"안녕 {i} 번", ConsoleColor.Yellow);
            }

            for (int i = 0; i < CLI.logLimit * 2; i++)
            {
                CLI.PrintQueueLog(Channel.One, $"안녕 {i} 번", ConsoleColor.Yellow);
            }

            Console.WriteLine();
            sniffer.Select();
            Console.OutputEncoding = Encoding.Unicode;
            CLI.CleanQueue(Channel.Zero);
            CLI.CleanQueue(Channel.One);
            //Console.WriteLine("\n─────────────────────────────────────────────────────────────────────");
            sniffer.Run();


        }
    }
}

