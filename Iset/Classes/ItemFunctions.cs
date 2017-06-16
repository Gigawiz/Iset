using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class ItemFunctions
    {
        static SqlConnection conn;
        static SQLiteConnection m_dbConnection;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static string clearQueue()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM QueuedItem";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "Queue has been cleared!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void logRestore(string staffname, string accountid, string accountname, string charactername, string itemcode)
        {
            try
            {
                using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                {
                    string sql = "INSERT INTO item_restores (discordStaff,accountID, accountName,characterName,itemCode) VALUES (@staffName, @accountID, @accountName, @characterName, @itemCode);";
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("@staffName", staffname);
                    command.Parameters.AddWithValue("@accountID", accountid);
                    command.Parameters.AddWithValue("@accountName", accountname);
                    command.Parameters.AddWithValue("@characterName", charactername);
                    command.Parameters.AddWithValue("@itemCode", itemcode);
                    m_dbConnection.Open();
                    command.ExecuteNonQuery();
                    m_dbConnection.Close();
                }
            }
            catch (SQLiteException ex)
            {
                Logging.LogItem(ex.Message);
            }
        }

        public static List<string> hasRestored(string accountid)
        {
            List<string> restoredItems = new List<string>();
            try
            {
                using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                {
                    string oString = "SELECT * FROM item_restores WHERE accountID = @fName";
                    SQLiteCommand oCmd = new SQLiteCommand(oString, m_dbConnection);
                    oCmd.Parameters.AddWithValue("@fName", accountid);
                    m_dbConnection.Open();
                    using (SQLiteDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["itemCode"].ToString()))
                            {
                                restoredItems.Add(oReader["itemCode"].ToString());
                            }
                        }
                        m_dbConnection.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Logging.LogItem(ex.Message);
            }
            return restoredItems;
        }

        public static string restoreItem(string character, string enhancement, string weapontype, string staff)
        //!restoreweapon <charactername> <enhancement level> <weapon type> [quality] [prefix] [suffix] [fusion]
        {
            string charid = UserFunctions.getCharacterIdFromName(character);
            string accountId = UserFunctions.getUserIdFromCharacterName(character);
            string accountName = UserFunctions.getAccountNameFromID(accountId);
            if (string.IsNullOrEmpty(charid))
            {
                return "Invalid character name!";
            }
            List<string> restoredItems = hasRestored(accountId);
            int maxRestoredWeapons = 1;
            int.TryParse(ini.IniReadValue("misc", "maxitemrestoresperaccount"), out maxRestoredWeapons);
            if (restoredItems.Count() >= maxRestoredWeapons)
            {
                string retstr = "An item has allready been restored for " + character + "! Previously Restored Item(s): " + Environment.NewLine;
                foreach (string restore in restoredItems)
                {
                    retstr = retstr + "```" + restore + "```" + Environment.NewLine;
                }
                return retstr;
            }
            string wepToRestore = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesLog; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT TOP 1 ItemClass FROM ItemLedger01 WHERE ItemClass LIKE @fWeaponType ESCAPE ' ' AND ItemClass LIKE @fEnhance AND CharacterID = @fName ORDER BY TIME ASC";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charid);
                    oCmd.Parameters.AddWithValue("@fEnhance", "%ENHANCE:" + enhancement + "%");
                    oCmd.Parameters.AddWithValue("@fWeaponType", "%" + weapontype + "%");
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["ItemClass"].ToString()))
                            {
                                wepToRestore = oReader["ItemClass"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
                if (string.IsNullOrEmpty(wepToRestore))
                {
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesLog; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                        string oString = "SELECT TOP 1 ItemClass FROM ItemLedger02 WHERE ItemClass LIKE @fWeaponType ESCAPE ' ' AND ItemClass LIKE @fEnhance AND CharacterID = @fName ORDER BY TIME ASC";
                        SqlCommand oCmd = new SqlCommand(oString, conn);
                        oCmd.Parameters.AddWithValue("@fName", charid);
                        oCmd.Parameters.AddWithValue("@fEnhance", "%ENHANCE:" + enhancement + "%");
                        oCmd.Parameters.AddWithValue("@fWeaponType", "%" + weapontype + "%");
                        conn.Open();
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            while (oReader.Read())
                            {
                                if (!string.IsNullOrEmpty(oReader["ItemClass"].ToString()))
                                {
                                    wepToRestore = oReader["ItemClass"].ToString();
                                }
                            }
                            conn.Close();
                        }
                    }
                    if (string.IsNullOrEmpty(wepToRestore))
                    {
                        return "Weapon not found!";
                    }
                }
                if (!string.IsNullOrEmpty(wepToRestore))
                {
                    SendMail(character, wepToRestore, 1, "Here is your weapon restore. Please be warned, the next destruction of this weapon will be permanant! You now have 0 weapon/item restores available.", "BloodRedDawn Item Restoration");
                    logRestore(staff, accountId, accountName, character, wepToRestore);
                }
            }
            catch (SqlException ex)
            {
                Logging.LogItem(ex.Message);
                return ex.Message;
            }
            Logging.LogItem(character + " has had an item restored by " + staff + ". Item code: " + wepToRestore);
            return "The weapon has been found and restored for " + character + "!";
        }

        public static string SendMailToAllOnline(string itemtospawn, int count, string mailsender)
        {
            List<string> mailRecepients = MiscFunctions.onlinePlayers();
            string mailTo = null;
            int i = 0;
            try
            {
                foreach (string user in mailRecepients)
                {
                    string mailReciever = UserFunctions.getCharacterIdFromName(user);
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                        string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 0, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                        SqlCommand oCmd = new SqlCommand(oString, conn);
                        oCmd.Parameters.AddWithValue("@CID", mailReciever);
                        oCmd.Parameters.AddWithValue("@ItemClassEx", itemtospawn);
                        oCmd.Parameters.AddWithValue("@Count", count);
                        oCmd.Parameters.AddWithValue("@MailContent", "Here is the item you requested from " + mailsender);
                        oCmd.Parameters.AddWithValue("@MailTitle", "From Iset and " + mailsender);
                        conn.Open();
                        oCmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    if (i == 0)
                    {
                        mailTo = user;
                    }
                    else
                    {
                        mailTo = mailTo + ", " + user;
                    }
                    i++;
                }

                return "The items have been sent to " + mailTo + "! They will need to relog or run a quest to see the mail!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string SendMail(string charactername, string itemtospawn, int count, string mailcontent, string mailsender)
        {
            string characterId = UserFunctions.getCharacterIdFromName(charactername);
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 0, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@CID", characterId);
                    oCmd.Parameters.AddWithValue("@ItemClassEx", itemtospawn);
                    oCmd.Parameters.AddWithValue("@Count", count);
                    oCmd.Parameters.AddWithValue("@MailContent", mailcontent);
                    oCmd.Parameters.AddWithValue("@MailTitle", mailsender);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "The item has been sent to " + charactername + "! They will need to relog or run a quest to see the mail!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
