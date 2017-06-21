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
            string market = returnExpiredMarketItems();
            string avgPrices = runMarketAvgPrices();
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

                    break;
                case "stop":

                    break;
                case "restart":

                    break;
                case "updateHeroesContents":

                    break;
                default:

                    break;
            }
            return "Not Implemented Yet.";
        }
    }
}
