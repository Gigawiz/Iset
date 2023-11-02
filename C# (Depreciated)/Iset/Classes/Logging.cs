using Discord;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class Logging
    {
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static void OldLogItem(string logStr)
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

        public static void LogItem(string logline, string staffname = null, string cmd = null, string vars = null)
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
            string logEntry = currentTime + ": " + logline;
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
            if (!String.IsNullOrEmpty(staffname) && !String.IsNullOrEmpty(cmd) && !String.IsNullOrEmpty(vars))
            {
                logToDB(staffname, cmd, vars);
            }
        }

        public static void logToDB(string staffname, string cmd, string vars)
        {
            try
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                {
                    string sql = "INSERT INTO command_logs (discordStaffName,command,variables) VALUES (@staffName,@cmd, @vars);";
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("@staffName", staffname);
                    command.Parameters.AddWithValue("@cmd", cmd);
                    command.Parameters.AddWithValue("@vars", vars);
                    m_dbConnection.Open();
                    command.ExecuteNonQuery();
                    m_dbConnection.Close();
                }
            }
            catch (SQLiteException ex)
            {
                Logging.OldLogItem(ex.Message);
            }
        }
    }
}
