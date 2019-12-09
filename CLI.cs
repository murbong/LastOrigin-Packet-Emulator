using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Pcap
{
    public enum Channel
    {
        Zero,
        One,
        Two
    }
    public static class CLI
    {
        public static void ShowToast(string appId, string title, string message, string image)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(
                string.IsNullOrEmpty(image) ? ToastTemplateType.ToastText02 :
                ToastTemplateType.ToastImageAndText02);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            if (string.IsNullOrEmpty(image) == false)
            {
                string imagePath = "file:///" + image;
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
            }

            ToastNotification toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }

        public const int gap = 4;
        public const int alt = 10;
        private const int combo = 3;
        public const int tableWidth = combo * 16;
        public const int logLimit = 30;
        public const int channelGap = 100;
        private static readonly Dictionary<int, Queue<(string, ConsoleColor, ConsoleColor)>> ConsoleQueue = new Dictionary<int, Queue<(string, ConsoleColor, ConsoleColor)>>();


        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }
        public static void CleanQueue(Channel ch)
        {
            int channel = (int)ch;
            if (ConsoleQueue.ContainsKey(channel) == true)
            {
                for (int i = 0; i < ConsoleQueue[channel].Count; i++)
                {
                    Console.SetCursorPosition(channel * channelGap, i + tableWidth / 2);
                    Console.Write(new string(' ', channelGap));// 줄 지우기
                }
            }
            if (ConsoleQueue.ContainsKey(channel) == true)
            {
                ConsoleQueue[channel].Clear();
            }
        }
        public static void PrintQueueLog(Channel ch,string str)
        {
            PrintQueueLog(ch, str, ConsoleColor.White);
        }
        public static void PrintQueueLog(Channel ch,string str, ConsoleColor clr,ConsoleColor back = 0,bool date = true)
        {
            int channel = (int)ch;
            if (ConsoleQueue.ContainsKey(channel) == false)
            {
                ConsoleQueue[channel] = new Queue<(string, ConsoleColor, ConsoleColor)>();
            }
            str = (date? GetCurrentDate() + " : " : "") + str;
            ConsoleQueue[channel].Enqueue((str, clr, back));

            if (ConsoleQueue[channel].Count > logLimit)
            {
                ConsoleQueue[channel].Dequeue();
            }
            for (int i = 0; i < ConsoleQueue[channel].Count; i++)
            {
                Console.SetCursorPosition(channel * channelGap , i + tableWidth / 2);
                Console.Write(new string(' ', channelGap));// 줄 지우기

                Console.SetCursorPosition(channel * channelGap, i + tableWidth / 2);

                var prevC = Console.ForegroundColor;
                var prevB = Console.BackgroundColor;
                Console.ForegroundColor = ConsoleQueue[channel].ElementAt(i).Item2;
                Console.BackgroundColor = ConsoleQueue[channel].ElementAt(i).Item3;
                Console.Write(ConsoleQueue[channel].ElementAt(i).Item1);
                Console.ForegroundColor = prevC;
                Console.BackgroundColor = prevB;

            }
        }
        public static void RemoveRange(int x, int y, int range)
        {
            for (int i = 0; i < range; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(new string(' ', Console.BufferWidth));
            }
        }
        public static void PrintGrid(int x, int y)
        {
            int width = tableWidth;
            int height = tableWidth / 2;

            for (int c = 1; c <= combo - 1; c++)
            {
                Console.SetCursorPosition(x, y + height * c / combo);
                for (int i = 0; i < width; i++)
                {
                    Console.Write("─");
                }
            }
            for (int c = 1; c <= combo - 1; c++)
            {
                for (int i = 0; i < height; i++)
                {
                    Console.SetCursorPosition(x + width * c / combo, y + i);
                    Console.Write("│");
                }
            }
            Console.SetCursorPosition(0, y + height);
        }
        public static int AlignCenter(string str)
        {
            int strWidth = -1;
            foreach (char ch in str)
            {
                if (Encoding.UTF8.GetByteCount(ch.ToString()) > 1)
                {
                    strWidth += 2;
                }
                else
                {
                    strWidth++;
                }
            }
            return strWidth;
        }
        public static void RemoveCharacter(int x, int y, int grid, int offsetX = 0, int offsetY = 0)
        {
            int width = tableWidth;
            int height = tableWidth / 2;
            int strX = grid % combo;
            int strY = grid / combo;

            int _x = x + width * (strX * 2) / (combo * 2) + offsetX + 1;
            int _y = y + height * (1 + strY * 2) / (combo * 2) + offsetY;
            Console.SetCursorPosition(_x, _y);
            Console.Write(new string(' ', tableWidth / combo - 1));
            Console.SetCursorPosition(0, y + height);
        }
        public static void PrintCharacter(int x, int y, int grid, string str, int offsetX = 0, int offsetY = 0)
        {
            int width = tableWidth;
            int height = tableWidth / 2;
            int strX = grid % combo;
            int strY = grid / combo;
            int _x = x + width * (1 + strX * 2) / (combo * 2);
            int _y = y + height * (1 + strY * 2) / (combo * 2);
            int Align = AlignCenter(str);
            Console.SetCursorPosition(_x - Align / 2 + offsetX, _y + offsetY);
            Console.Write(str);
            Console.SetCursorPosition(0, y + height);
        }
    }
}
