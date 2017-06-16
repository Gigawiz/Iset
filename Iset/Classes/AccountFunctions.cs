using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class AccountFunctions
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static string resetSecondary(string playername)
        {
            string retmsg = null;
            string characterName = UserFunctions.getUserIdFromCharacterName(playername);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterName))
            {
                accountName = UserFunctions.getAccountNameFromID(characterName);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = playername;
                }
            }
            else
            {
                accountName = playername;
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE [User] SET SecondPassword=@fSecondary WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    oCmd.Parameters.AddWithValue("@fSecondary", DBNull.Value);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "The secondary password for " + playername + " has been reset.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }
    }
}
