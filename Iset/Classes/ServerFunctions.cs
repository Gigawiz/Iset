using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            returnExpiredMarketItems();
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

        public static string parseServerAction(string action, string args = null)
        {
            switch (action)
            {
                case "start":
                    stopServer();
                    startServer();
                    return "Server Started";
                case "stop":
                    stopServer();
                    return "Server Stopped";
                case "restart":
                    stopServer();
                    startServer();
                    return "Server Restarted";
                case "updateHeroesContents":

                    break;
                default:

                    break;
            }
            return "Not Implemented Yet.";
        }

        internal static void startServer()
        {
            /*cd bin
start Executer.exe UnifiedNetwork.dll UnifiedNetwork.LocationService.LocationService StartService LocationService 42
start Executer.exe AdminClientServiceCore.dll AdminClientServiceCore.AdminClientService StartService AdminService 127.0.0.1 42
start Executer.exe FrontendServiceCore.dll FrontendServiceCore.FrontendService StartService FrontendService 127.0.0.1 42
start Executer.exe CashShopService.dll CashShopService.CashShopService StartService CashShopService 127.0.0.1 42
start Executer.exe RankService.dll RankService.RankService StartService RankService 127.0.0.1 42
start Executer.exe GuildService.dll GuildService.GuildService StartService GuildService 127.0.0.1 42
start Executer.exe PvpService.dll PvpService.PvpService StartService PvpService 127.0.0.1 42
start Executer.exe LoginServiceCore.dll LoginServiceCore.LoginService StartService LoginService 127.0.0.1 42
start Executer.exe MicroPlayServiceCore.dll MicroPlayServiceCore.MicroPlayService StartService MIcroPlayService 127.0.0.1 42
start Executer.exe MMOChannelService.dll MMOChannelService.MMOChannelService StartService MMOChannelService 127.0.0.1 42
start Executer.exe PlayerService.dll PlayerService.PlayerService StartService PlayerService 127.0.0.1 42
start Executer.exe DSService.dll DSService.DSService StartService DSService 127.0.0.1 42
start Executer.exe PingService.dll PingServiceCore.PingService StartService PingService 127.0.0.1 42
start Executer.exe UserDSHostService.dll UserDSHostService.UserDSHostService StartService UserDSHostService 127.0.0.1 42*/
        }

        internal static void stopServer()
        {
            //taskkill / f / im Executer.exe
        }
    }
}
