using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class Logging
    {
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static void LogItem(string logStr)
        {
            string currentTime = DateTime.Now.ToString();
            string currentDate = DateTime.Now.ToString("dd.MM.yyy");
            bool logToFile = true;
            bool logtoConsole = true;
            bool logToDiscordChannel = true;
            ulong channelId = 0;
            ulong.TryParse(ini.IniReadValue("logs", "logchannelid"), out channelId);
            bool.TryParse(ini.IniReadValue("logs", "logtofile"), out logToFile);
            bool.TryParse(ini.IniReadValue("logs", "logtoconsole"), out logtoConsole);
            bool.TryParse(ini.IniReadValue("logs", "logToDiscordChannel"), out logtoConsole);
            string logEntry = currentTime + ": " + logStr;
            if (logToFile)
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\logs"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\logs");
                }
                File.AppendAllText(Directory.GetCurrentDirectory() + @"\logs\" + currentDate + ".txt", logEntry + Environment.NewLine);
            }
            if (logtoConsole)
            {
                Console.WriteLine(logEntry);
            }
            if (logToDiscordChannel && channelId > 0)
            {
                Channel logTo = Program._client.GetChannel(channelId);
                logTo.SendMessage(logEntry);
            }
        }
    }
}
