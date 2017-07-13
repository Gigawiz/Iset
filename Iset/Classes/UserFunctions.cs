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
    class UserFunctions
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public static List<string> findPossibleAlts(string discordName, string discordNickname)
        {
            List<string> characterNames = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE UPPER(Name) LIKE @fName";
                    if (!string.IsNullOrEmpty(discordNickname))
                    {
                        oString = oString + " OR UPPER(Name) LIKE @fNick";
                    }
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", '%' + discordName.ToUpper() + '%');
                    if (!string.IsNullOrEmpty(discordNickname))
                    {
                        oCmd.Parameters.AddWithValue("@fNick", '%' + discordNickname.ToUpper() + '%');
                    }
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            characterNames.Add(oReader["Name"].ToString());
                        }
                        conn.Close();
                    }
                }
                List<string> accountIds = new List<string>();
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM [User] WHERE UPPER(Name) LIKE @fName";
                    if (!string.IsNullOrEmpty(discordNickname))
                    {
                        oString = oString + " OR UPPER(Name) LIKE @fNick";
                    }
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", "%" + discordName.ToUpper() + "%");
                    if (!string.IsNullOrEmpty(discordNickname))
                    {
                        oCmd.Parameters.AddWithValue("@fNick", '%' + discordNickname.ToUpper() + '%');
                    }
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            accountIds.Add(oReader["ID"].ToString());
                        }
                        conn.Close();
                    }
                }
                if (accountIds.Count() > 0)
                {
                    foreach (string accountId in accountIds)
                    {
                        using (conn = new SqlConnection())
                        {
                            conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                            string oString = "SELECT * FROM CharacterInfo WHERE UID=@fName";
                            SqlCommand oCmd = new SqlCommand(oString, conn);
                            oCmd.Parameters.AddWithValue("@fName", accountId);
                            conn.Open();
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                while (oReader.Read())
                                {
                                    if (!characterNames.Contains(oReader["Name"].ToString()))
                                    {
                                        characterNames.Add(oReader["Name"].ToString());
                                    }
                                }
                                conn.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.OldLogItem(ex.Message);
            }
            return characterNames;
        }

        public static string getCharacterNameFromID(string characterid)
        {
            string username = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from CharacterInfo where ID=@fId";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fId", characterid);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["Name"].ToString()))
                            {
                                username = oReader["Name"].ToString();
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
            return username;
        }

        public static List<string> playerList(string returnDataType, string searchType = "online", bool skipStorageChars = false)
        {
            List<string> characterNames = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE Status = 0";
                    if (searchType == "online")
                    {
                        oString = oString + " AND IsConnected=1";
                    }
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (returnDataType == "id")
                            {
                                characterNames.Add(oReader["ID"].ToString());
                            }
                            else if (returnDataType == "name")
                            {
                                characterNames.Add(oReader["Name"].ToString());
                            }
                            else
                            {
                                characterNames.Add(oReader["Name"].ToString());
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                characterNames.Add("Error in query!");
                characterNames.Add(ex.Message);
            }
            return characterNames;
        }

        public static List<string> findAlts(string charactername)
        {
            List<string> characterNames = new List<string>();
            string submittedCharacterAccID = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(submittedCharacterAccID))
            {
                return null;
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE UID=@fName ORDER BY STATUS ASC";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", submittedCharacterAccID);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            int statusCode = 0;
                            int.TryParse(oReader["Status"].ToString(), out statusCode);
                            if (statusCode > 0)
                            {
                                characterNames.Add("~~" + oReader["Name"].ToString() + "~~");
                            }
                            else
                            {
                                characterNames.Add(oReader["Name"].ToString());
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                characterNames.Add("Error in query!");
                characterNames.Add(ex.Message);
            }
            return characterNames;
        }

        public static string generateProfile(string charactername)
        {
            string charid = getCharacterIdFromName(charactername);
            if (string.IsNullOrEmpty(charid))
            {
                return "Invalid Character Name";
            }
            string str1 = charactername;
            string connectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
            string str2 = string.Empty + "SELECT [ID] ,[UID] ,[CharacterSN] ,[Name] ,[Class] ,[Level] ,[Exp],[STR],[DEX],[INT],[WILL],[LUCK],[HP],[STAMINA],[PlayTime],[SelectedTitle],[AP],[TotalUsedAP],[Status],[Quote],[IsConnected],[CreateTime],[FreeMatchWinCount],[FreeMatchLoseCount],[TodayPlayTime],[Channel] FROM[heroes].[dbo].[CharacterInfo] WHERE Name = '" + str1 + "' ";
            using (SqlConnection sqlConnection1 = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand1 = new SqlCommand())
                {
                    SqlCommand sqlCommand2 = sqlCommand1;
                    SqlConnection sqlConnection2 = sqlConnection1;
                    sqlCommand2.Connection = sqlConnection2;
                    int num = 1;
                    sqlCommand2.CommandType = (CommandType)num;
                    string str3 = str2;
                    sqlCommand2.CommandText = str3;
                    sqlConnection1.Open();
                    SqlDataReader sqlDataReader = sqlCommand2.ExecuteReader();
                    try
                    {
                        while (sqlDataReader.Read())
                        {
                            string str4 = sqlDataReader["ID"].ToString();
                            string str5 = sqlDataReader["UID"].ToString();
                            string str6 = sqlDataReader["CharacterSN"].ToString();
                            string str7 = sqlDataReader["Name"].ToString();
                            string str8 = sqlDataReader["Class"].ToString();
                            string str9 = sqlDataReader["Level"].ToString();
                            string str10 = sqlDataReader["Exp"].ToString();
                            string str11 = sqlDataReader["STR"].ToString();
                            string str12 = sqlDataReader["DEX"].ToString();
                            string str13 = sqlDataReader["INT"].ToString();
                            string str14 = sqlDataReader["WILL"].ToString();
                            string str15 = sqlDataReader["LUCK"].ToString();
                            string str16 = sqlDataReader["HP"].ToString();
                            string str17 = sqlDataReader["STAMINA"].ToString();
                            string str18 = sqlDataReader["PlayTime"].ToString();
                            string str19 = sqlDataReader["SelectedTitle"].ToString();
                            string str20 = sqlDataReader["AP"].ToString();
                            string str21 = sqlDataReader["TotalUsedAP"].ToString();
                            string str22 = sqlDataReader["Quote"].ToString();
                            string str23 = sqlDataReader["IsConnected"].ToString();
                            string str24 = sqlDataReader["CreateTime"].ToString();
                            string str25 = sqlDataReader["TodayPlayTime"].ToString();
                            string str26 = sqlDataReader["Channel"].ToString();
                            string str27 = !(str23 == "True") ? "Offline" : "Online";
                            sqlConnection1.Close();
                            return "```Character Info: \nID: " + str4 + "\nUID: " + str5 + "\nCharacterSN: " + str6 + "\nName: " + str7 + "\nClass: " + str8 + "\nLevel: " + str9 + "\nExp: " + str10 + "\nSTR: " + str11 + "\nDEX: " + str12 + "\nINT: " + str13 + "\nWILL: " + str14 + "\nLUCK: " + str15 + "\nHP: " + str16 + "\nSTAMINA: " + str17 + "\nPlayTime: " + str18 + "\nSelectedTitle: " + str19 + "\nAP: " + str20 + "\nTotalUsedAP: " + str21 + "\nQuote: " + str22 + "\nStatus: " + str27 + "\nCreateTime: " + str24 + "\nTodayPlayTime: " + str25 + "\nChannel: " + str26 + "\n```";
                        }
                    }
                    catch (SqlException ex)
                    {
                        return ex.Message;
                    }
                }
                return "error";
            }
        }

        public static string setTrans(string character, string transtype)
        {
            string charnum = getCharacterIdFromName(character);
            if (string.IsNullOrEmpty(charnum))
            {
                return "Invalid character name!";
            }
            int num;
            string ret = character + " is now a level 40 Paladin";
            if (transtype.ToUpper() == "PALADIN")
            {
                num = 0;
            }
            else if (transtype.ToUpper() == "DARK KNIGHT")
            {
                num = 1;
                ret = character + " is now a level 40 Dark Knight";
            }
            else
            {
                return "Invalid Trans type given!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO [heroes].[dbo].[Vocation] ([CID] ,[VocationClass] ,[VocationLevel] ,[VocationExp] ,[LastTransform]) VALUES (@charNum,@transtype,40,0,GETDATE())";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@charNum", charnum);
                    oCmd.Parameters.AddWithValue("@transtype", num);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            return ret;
        }


        public static string getCharacterIdFromName(string characterName)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from CharacterInfo where Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["ID"].ToString()))
                            {
                                userid = oReader["ID"].ToString();
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
            return userid;
        }

        public static string getUserIdFromCharacterName(string characterName)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from CharacterInfo where Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["UID"].ToString()))
                            {
                                userid = oReader["UID"].ToString();
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
            return userid;
        }


        public static string restoreDeletedCharacter(string charactername)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Status=@fStatus, DeleteTime=@fDeleteTime WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fStatus", 0);
                    oCmd.Parameters.AddWithValue("@fDeleteTime", DBNull.Value);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "The character " + charactername + " has been restored. Please go back to the home screen to see it!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string setCharLevel(string charactername, int newlevel)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Level=@fLevel WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fLevel", newlevel);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = charactername + "'s level has been set to " + newlevel.ToString() + ". Please relog to see your new level!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }


        public static string giveApToChar(string charactername, int amount)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET AP=AP + @fNewAP WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fNewAP", amount);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Successfully gave " + amount.ToString() + " ap to " + charactername + "! They will need to relog to see the AP changes.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string deleteCharacter(string charactername)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Status=1, DeleteTime=CURRENT_TIMESTAMP WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fStatus", 0);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "The character " + charactername + " has been deleted.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string changeUserName(string currentname, string newname)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(currentname);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            string nameExists = getUserIdFromCharacterName(newname);
            if (!string.IsNullOrEmpty(nameExists))
            {
                return "Character name is already taken!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Name=@fNewName WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", currentname);
                    oCmd.Parameters.AddWithValue("@fNewName", newname);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Character Name \"" + currentname + "\" has been changed to '" + newname + "'!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public static string checkActivity(string userName)
        {
            string characterId = getCharacterIdFromName(userName);
            if (string.IsNullOrEmpty(characterId))
            {
                return "Invalid Character Specified";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT convert(varchar(max),convert(varbinary(max),LastUpdate)) FROM CharacterInfo WHERE ID=@fname";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterId);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["LastUpdate"].ToString()))
                            {
                                return oReader["LastUpdate"].ToString();
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
            return null;
        }

        public static string getAccountNameFromID(string accountID)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT Name FROM [User] WHERE ID=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountID);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["Name"].ToString()))
                            {
                                userid = oReader["Name"].ToString();
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
            return userid;
        }
    }
}
