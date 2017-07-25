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

        public static string findItemID (string characterID, string itemName)
        {
            string ret = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT TOP 1 * FROM Item WHERE OwnerID = @fName AND ItemClass LIKE @fCode";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterID);
                    oCmd.Parameters.AddWithValue("@fCode", "%" + itemName + "%");
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["ID"].ToString()))
                            {
                                 ret = oReader["ID"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Logging.OldLogItem(ex.Message);
            }
            return ret;
        }

        public static string dyeItem(string charactername, string itemtoDye, string slot1, string slot2, string slot3)
        {
            string characterid = UserFunctions.getCharacterIdFromName(charactername);
            if (string.IsNullOrEmpty(characterid))
            {
                return "Invalid Character Name!";
            }
            string itemID = findItemID(characterid, itemtoDye);
            if (string.IsNullOrEmpty(itemID))
            {
                return "Invalid item name!";
            }
            int slt1 = 0;
            int slt2 = 0;
            int slt3 = 0;
            if (!int.TryParse(slot1, out slt1))
            {
                return "Invalid Color given for slot 1!";
            }
            if (!int.TryParse(slot2, out slt2))
            {
                return "Invalid Color given for slot 2!";
            }
            if (!int.TryParse(slot3, out slt3))
            {
                return "Invalid Color given for slot 3!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE Equippable SET";
                    if (slt1 >= 0)
                    {
                        oString = oString + " Color1 = @fColor1";
                    }
                    if (slt2 >= 0)
                    {
                        if (slt1 >= 0)
                            oString = oString + ",";

                        oString = oString + " Color2 = @fColor2";
                    }
                    if (slt3 >= 0)
                    {
                        if (slt2 >= 0 || slt1 >= 0)
                            oString = oString + ",";

                        oString = oString + " Color3 = @fColor3";
                    }
                    oString = oString + " WHERE ID = @fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", itemID);
                    if (slt1 >= 0)
                    {
                        oCmd.Parameters.AddWithValue("@fColor1", slt1);
                    }
                    if (slt2 >= 0)
                    {
                        oCmd.Parameters.AddWithValue("@fColor2", slt2);
                    }
                    if (slt3 >= 0)
                    {
                        oCmd.Parameters.AddWithValue("@fColor3", slt3);
                    }
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            return "The item colors have been changed! " + charactername + " will need to re-equip the item to see the changes.";
        }

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

        public static void logSpawn(string staffname, string characterid, string charactername, string itemcode, string quantity)
        {
            try
            {
                using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                {
                    string sql = "INSERT INTO item_spawns (discordStaffName,characterName,itemCode,qty) VALUES (@staffName,@characterName, @itemCode, @qty);";
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.Parameters.AddWithValue("@staffName", staffname);
                    command.Parameters.AddWithValue("@characterName", charactername);
                    command.Parameters.AddWithValue("@itemCode", itemcode);
                    command.Parameters.AddWithValue("@qty", quantity);
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
                Logging.OldLogItem(ex.Message);
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
                Logging.OldLogItem(ex.Message);
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
                string retstr = "An item has allready been restored for " + character + "!";
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
                Logging.OldLogItem(ex.Message);
                return ex.Message;
            }
            Logging.LogItem(character + " has had an item restored by " + staff + ". Item code: " + wepToRestore, staff, "restoreweapon - " +character, wepToRestore);
            return "The weapon has been found and restored for " + character + "!";
        }

        public static string forceRestoreItem(string character, string wepcode, string staff)
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
                string retstr = "An item has allready been restored for " + character + "!";
                return retstr;
            }
            string wepToRestore = null;
            try
            {
                if (string.IsNullOrEmpty(wepcode))
                {
                    return "Invalid or missing weapon code!";
                }
                wepToRestore = wepcode;
                if (!string.IsNullOrEmpty(wepToRestore))
                {
                    SendMail(character, wepToRestore, 1, "Here is your weapon restore. Please be warned, the next destruction of this weapon will be permanant! You now have 0 weapon/item restores available.", "BloodRedDawn Item Restoration");
                    logRestore(staff, accountId, accountName, character, wepToRestore);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            Logging.LogItem(character + " has had an item restored by " + staff + ". Item code: " + wepToRestore, staff, "forcerestoreweapon - " +character, wepToRestore);
            return "The weapon has been found and restored for " + character + "!";
        }

        public static Constructors.CharacterItem getItemDetails(string charactername, string itemcode, string enhancement)
        {
            Constructors.CharacterItem item = new Constructors.CharacterItem();
            string characterid = UserFunctions.getCharacterIdFromName(charactername);
            if (String.IsNullOrEmpty(characterid))
            {
                return null;
            }
            int enhancelevel = 0;
            if (!int.TryParse(enhancement, out enhancelevel))
            {
                return null;
            }
            if (enhancelevel < 0 || enhancelevel > 15)
            {
                return null;
            }
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                string oString = "SELECT TOP 1 * FROM Item WHERE OwnerID = @CID AND ItemClass LIKE @ItemClass";
                SqlCommand oCmd = new SqlCommand(oString, conn);
                oCmd.Parameters.AddWithValue("@CID", characterid);
                oCmd.Parameters.AddWithValue("@ItemClass", "'%" + itemcode + "%'");
                conn.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        if (!string.IsNullOrEmpty(oReader["ItemClass"].ToString()))
                        {
                            item.ItemClass = oReader["ItemClass"].ToString();
                        }
                        if (!string.IsNullOrEmpty(oReader["ID"].ToString()))
                        {
                            item.ItemID = oReader["ID"].ToString();
                        }
                    }
                    conn.Close();
                }
            }
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                string oString = "SELECT * FROM ItemAttribute WHERE ItemID = @ItemID";
                SqlCommand oCmd = new SqlCommand(oString, conn);
                oCmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                conn.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        switch(oReader["Attribute"].ToString())
                        {
                            case "ANTIBIND":
                                item.BindCount = oReader["Value"].ToString();
                                break;
                            case "COMBINATION":
                                item.Combination = oReader["Value"].ToString();
                                break;
                            case "ENHANCE":
                                item.Enhancement = oReader["Value"].ToString();
                                break;
                            case "LOOK":
                                item.Look = oReader["Value"].ToString();
                                break;
                            case "PREFIX":
                                item.Prefix = oReader["Value"].ToString();
                                break;
                            case "PS_0":
                                item.PS0 = new Tuple<string, string, string>(oReader["Value"].ToString(), oReader["Arg"].ToString(), oReader["Arg2"].ToString());
                                break;
                            case "PS_1":
                                item.PS1.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;
                            case "PS_2":
                                item.PS2.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;
                            case "PS_3":
                                item.PS3.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;
                            case "PS_4":
                                item.PS4.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;
                            case "SUFFIX":
                                item.Suffix = oReader["Value"].ToString();
                                break;
                            case "QUALITY":
                                item.Quality.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;
                            case "RESTORED":
                                if (oReader["Value"].ToString() == "1")
                                {
                                    item.Restored = true;
                                }
                                else
                                {
                                    item.Restored = false;
                                }
                                break;
                            case "SPIRIT_INJECTION":
                                item.Infusion.Add(oReader["Value"].ToString(), oReader["Arg"].ToString());
                                break;

                        }
                    }
                    conn.Close();
                }
            }
            return item;
        }

        public static string SendMail(string charactername, string itemtospawn, int count, string mailcontent, string mailsender, string staffname = "Iset")
        {
            List<string> mailToList = null;
            string retmsg = "The item has been sent to ";
            if (charactername == "all")
            {
                mailToList = UserFunctions.playerList("id", "all");
                if (mailToList.Count() <=0 )
                {
                    return "error getting players!";
                }
                retmsg = retmsg + "all players (" + mailToList.Count().ToString() + ")";
            }
            else if (charactername == "allonline")
            {
                mailToList = UserFunctions.playerList("id", "online");
                if (mailToList.Count() <=0)
                {
                    return "There are no players online!";
                }
                retmsg = retmsg + "all online players (" + mailToList.Count().ToString() + ")";
            }
            else
            {
                mailToList = new List<string>();
                mailToList.Add(UserFunctions.getCharacterIdFromName(charactername));
                retmsg = retmsg + charactername;
            }
            string itemCode = itemtospawn;
            if (itemCode != "gold")
            {
                if (itemCode.Contains("COMBINATION:"))
                {
                    if (!itemCode.Contains("ANTIBIND"))
                    {
                        itemCode = itemCode.Replace("]",",") + "ANTIBIND:3]";
                    }
                    if (!itemCode.Contains("RESTORED"))
                    {
                        itemCode = itemCode.Replace("]", ",") + "RESTORED:1]";
                    }
                }
                else
                {
                    itemCode = itemCode + "[ANTIBIND:3][RESTORED:1]";
                }
                return itemCode;
            }
            try
            {
                foreach (string recipient in mailToList)
                {
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                        string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 1, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                        SqlCommand oCmd = new SqlCommand(oString, conn);
                        oCmd.Parameters.AddWithValue("@CID", recipient);
                        oCmd.Parameters.AddWithValue("@ItemClassEx", itemCode);
                        oCmd.Parameters.AddWithValue("@Count", count);
                        oCmd.Parameters.AddWithValue("@MailContent", mailcontent);
                        oCmd.Parameters.AddWithValue("@MailTitle", mailsender);
                        conn.Open();
                        oCmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    logSpawn(staffname.Replace("()", ""), recipient, charactername, itemCode, count.ToString());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            retmsg = retmsg + "! They will need to relog or run a quest to see the mail!";
            return retmsg;
        }
    }
}
