using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class Bans
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static List<string> bannedPlayers()
        {
            List<string> bannedPlayers = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM UserBan";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            bannedPlayers.Add(oReader["ID"].ToString());
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bannedPlayers.Add("Error in query!");
                bannedPlayers.Add(ex.Message);
            }
            return bannedPlayers;
        }

        public static string isBanned(string playername)
        {
            string bannedStatus = "is _not_ banned.";
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
                    string oString = "Select * from UserBan where ID=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["ID"].ToString()))
                            {
                                bannedStatus = "is banned." + Environment.NewLine + "Ban Expires on: " + oReader["ExpireTime"].ToString() + Environment.NewLine + "Ban Reason: " + oReader["Reason"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return bannedStatus;
        }

        public static string BanUser(string userName, string banMessage = "You have been permanently banned. Contact staff for additional info.", int duration = 0)
        {
            string banResult = null;
            string characterId = UserFunctions.getUserIdFromCharacterName(userName);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterId))
            {
                accountName = UserFunctions.getAccountNameFromID(characterId);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = userName;
                }
            }
            else
            {
                accountName = userName;
            }
            string preBanCheck = isBanned(accountName);
            if (!string.IsNullOrEmpty(preBanCheck) && preBanCheck.Contains("is banned."))
            {
                return "The user " + userName + " is all ready banned.";
            }
            //INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (N'Gigawiz', '4', '2099-03-25 17:00:00.000', N'Testing');
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (@fName, '4', '2099-03-25 17:00:00.000', @fReason);";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    oCmd.Parameters.AddWithValue("@fReason", banMessage);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //"The user " + userName + " was successfully banned.";
                string banStatus = isBanned(accountName);
                if (!string.IsNullOrEmpty(banStatus) && banStatus.Contains("is banned."))
                {
                    banResult = "The user " + userName + " was successfully banned.";
                }
                else
                {
                    banResult = "The user " + userName + " was _NOT_ banned.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return banResult;
        }

        public static string unbanUser(string userName)
        {
            string banResult = null;
            string characterId = UserFunctions.getUserIdFromCharacterName(userName);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterId))
            {
                accountName = UserFunctions.getAccountNameFromID(characterId);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = userName;
                }
            }
            else
            {
                accountName = userName;
            }
            string preBanCheck = isBanned(accountName);
            if (string.IsNullOrEmpty(preBanCheck) || preBanCheck.Contains("is _not_ banned."))
            {
                return "The user " + userName + " is not banned.";
            }
            //INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (N'Gigawiz', '4', '2099-03-25 17:00:00.000', N'Testing');
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM UserBan WHERE ID = @fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //"The user " + userName + " was successfully banned.";
                string banStatus = isBanned(accountName);
                if (string.IsNullOrEmpty(banStatus) || banStatus.Contains("is _not_ banned."))
                {
                    banResult = "The user " + userName + " was successfully unbanned.";
                }
                else
                {
                    banResult = "The user " + userName + " was _NOT_ unbanned.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return banResult;
        }
    }
}
