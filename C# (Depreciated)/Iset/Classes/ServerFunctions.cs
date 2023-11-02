using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class ServerFunctions
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static void doTimedCommands()
        {
            Logging.LogItem(returnExpiredMarketItems(), "console", "!fix", "market");
            runMarketAvgPrices();
        }

        public static string runMarketAvgPrices()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesMarketPlace; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    using (var command = new SqlCommand("JobTradeItemAvgPrice", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                    };
                }
                return "Average Prices on market have been updated!";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        public static string returnExpiredMarketItems()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesMarketPlace; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    using (var command = new SqlCommand("JobExpireTradeItem", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                    };
                }
                return "Expired items on the market have been returned to sellers!";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        public static string sendAnnounceToServer(string message)
        {


            return null;
        }

        public static string parseServerAction(string action, string server)
        {
            switch (action)
            {
                case "start":
                    stopServer(server);
                    startServer(server);
                    return "Server Started";
                case "stop":
                    stopServer(server);
                    return "Server Stopped";
                case "restart":
                    stopServer(server);
                    startServer(server);
                    return "Server Restarted";
                case "updateHeroesContents":

                    break;
                default:

                    break;
            }
            return "Not Implemented Yet.";
        }

        internal static void startServer(string servername)
        {
            switch (servername)
            {
                case "all":
                    Process p = new Process();
                    break;
                case "LocationService":

                    break;
                case "AdminService":

                    break;
                case "FrontendService":

                    break;
                case "CashShopService":

                    break;
                case "RankService":

                    break;
                case "GuildService":

                    break;
                case "PvpService":

                    break;
                case "LoginService":

                    break;
                case "MIcroPlayService":

                    break;
                case "MMOChannelService":

                    break;
                case "PlayerService":

                    break;
                case "DSService":

                    break;
                case "PingService":

                    break;
                case "UserDSHostService":

                    break;
            }
        }

        internal static void stopServer(string servername)
        {

            //taskkill / f / im Executer.exe
            switch (servername)
            {
                case "all":

                    break;
                case "LocationService":

                    break;
                case "AdminService":

                    break;
                case "FrontendService":

                    break;
                case "CashShopService":

                    break;
                case "RankService":

                    break;
                case "GuildService":

                    break;
                case "PvpService":

                    break;
                case "LoginService":

                    break;
                case "MIcroPlayService":

                    break;
                case "MMOChannelService":

                    break;
                case "PlayerService":

                    break;
                case "DSService":

                    break;
                case "PingService":

                    break;
                case "UserDSHostService":

                    break;
            }
        }
    }
}
