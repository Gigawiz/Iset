using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class GuildFunctions
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");

        public static string findGuildSN (string guildname)
        {
            string ret = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM GuildInfo WHERE GuildName = @fGuildName OR GuildID = @fGuildName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildName", guildname);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["GuildSN"].ToString()))
                            {
                                return oReader["GuildSN"].ToString();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        public static string addGuildMaxMembers(string guildname, int newMaxMembers = 100)
        {
            string retmsg = null;
            string guildSN = findGuildSN(guildname);
            if (String.IsNullOrEmpty(guildSN))
            {
                return "Invalid Guild Name";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE Guild SET MaxMemberLimit=MaxMemberLimit + @fAddtlMax WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    oCmd.Parameters.AddWithValue("@fAddtlMax", newMaxMembers);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Added " + newMaxMembers + " member slots to guild '" + guildname + "'!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string seGuildLevel(string guildname, int newlevel = 20)
        {
            string retmsg = null;
            string guildSN = findGuildSN(guildname);
            if (String.IsNullOrEmpty(guildSN))
            {
                return "Invalid Guild Name";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE Guild SET Level=@fNewLevel WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    oCmd.Parameters.AddWithValue("@fNewLevel", newlevel);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "level for guild '" + guildname + "' has been set to " + newlevel + "!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string reopenGuild(string guildname)
        {
            string retmsg = null;
            string guildSN = findGuildSN(guildname);
            if (String.IsNullOrEmpty(guildSN))
            {
                return "Invalid Guild Name";
            }
            try
            {
                //first conn to add deletion time
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE GuildInfo SET DateClosed=@fDateClosed WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    oCmd.Parameters.AddWithValue("@fDateClosed", DBNull.Value);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //second conn to get guild owner character id
                string owner = null;
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM GuildInfo WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["OwnerID"].ToString()))
                            {
                                owner = oReader["OwnerID"].ToString();
                            }
                        }
                    }
                    conn.Close();
                }
                //last run to insert owner back into guild roster
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO GuildUser ([CID], [GuildSN], [dateCreate], [dateLastModified], [codeGroupUserType]) VALUES (@fOwnerCID, @fGuildSN, GETDATE(), GETDATE(), @fCodeGroupUserType)";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fOwnerCID", owner);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    oCmd.Parameters.AddWithValue("@fCodeGroupUserType", 5);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Guild '" + guildname + "' has been restored!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }


        public static string closeGuild(string guildname)
        {
            string retmsg = null;
            string guildSN = findGuildSN(guildname);
            if (String.IsNullOrEmpty(guildSN))
            {
                    return "Invalid Guild Name";
            }
            try
            {
                //first conn to add deletion time
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE GuildInfo SET DateClosed=GETDATE() WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //second conn to kick all characters from a guild
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM GuildUser WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Guild '" + guildname + "' has been marked as deleted!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string changeGuildName(string currentname, string newname, string oldid = null, string newid = null)
        {
            string retmsg = null;
            string guildSN = findGuildSN(currentname);
            if (String.IsNullOrEmpty(guildSN))
            {
                if (!string.IsNullOrEmpty(oldid))
                {
                    guildSN = findGuildSN(oldid);
                    if (String.IsNullOrEmpty(guildSN))
                    {
                        return "Invalid Guild Name or ID";
                    }
                }
                else
                {
                    return "Invalid Guild Name";
                }
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE GuildInfo SET GuildName=@fNewName";
                    if (!string.IsNullOrEmpty(oldid) && !string.IsNullOrEmpty(newid))
                    {
                        oString = oString + ",GuildID=@fNewID";
                    }
                    oString = oString + " WHERE GuildSN=@fGuildSN";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fGuildSN", guildSN);
                    oCmd.Parameters.AddWithValue("@fNewName", newname);
                    if (!string.IsNullOrEmpty(oldid) && !string.IsNullOrEmpty(newid))
                    {
                        oCmd.Parameters.AddWithValue("@fNewID", newid);
                    }
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Guild Name changed from '" + currentname + "' to '" + newname + "'.";
                if (!string.IsNullOrEmpty(oldid) && !string.IsNullOrEmpty(newid))
                {
                    retmsg = retmsg + " Guild ID changed from '" + oldid + "' to '" + newid + "'";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }
    }
}
