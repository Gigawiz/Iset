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
    class ValidationFunctions
    {
        static SqlConnection conn;
        static SQLiteConnection m_dbConnection;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");

        public static string confirmValidation(string charactername, string validationCode)
        {
            return null;
        }

        public static bool codeExists(string validationCode)
        {
            bool ret = false;
            try
            {
                string sql = "select count(validationKey) from validations WHERE validationKey = '" + validationCode + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                m_dbConnection.Open();
                int rowCount = Convert.ToInt32(command.ExecuteScalar());
                Logging.OldLogItem(rowCount.ToString());
                m_dbConnection.Close();
            }
            catch (SqlException ex)
            {
                Logging.OldLogItem(ex.Message);
            }
            return ret;
        }

        public static string genValidationCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        public static string storeValidationCode(string charactername)
        {
            string validationCode = genValidationCode();
            //bool isUsed = codeExists(validationCode);
            /*while (isUsed == true)
            {
                validationCode = genValidationCode();
            }*/
            return validationCode;
        }

        public static void createInitialTables()
        {
            if (!System.IO.File.Exists("iset.db3"))
            {
                Console.WriteLine("Account Validations DB does not exist! Creating!");
                try
                {
                    SQLiteConnection.CreateFile("iset.db3");
                    using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                    {
                        string sql = "CREATE TABLE validations (discordName VARCHAR(50), characterName VARCHAR(50), validationKey VARCHAR(50), status INT)";
                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                        m_dbConnection.Open();
                        command.ExecuteNonQuery();
                        m_dbConnection.Close();
                    }
                    using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                    {
                        string sql = "CREATE TABLE item_restores (discordStaff VARCHAR(50), accountID VARCHAR(50), accountName VARCHAR(50), characterName VARCHAR(50), itemCode VARCHAR(500), dateRestored TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                        m_dbConnection.Open();
                        command.ExecuteNonQuery();
                        m_dbConnection.Close();
                    }
                    using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                    {
                        string sql = "CREATE TABLE item_spawns (discordStaffName VARCHAR(50), characterName VARCHAR(50), itemCode VARCHAR(500), qty INT(100), dateSpawned TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                        m_dbConnection.Open();
                        command.ExecuteNonQuery();
                        m_dbConnection.Close();
                    }
                    using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
                    {
                        string sql = "CREATE TABLE command_logs (discordStaffName VARCHAR(50), command VARCHAR(500), variables VARCHAR(500), dateRun TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
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
        }

        public static string checkValidationStatus(string charactername, string discordname)
        {
            using (m_dbConnection = new SQLiteConnection("Data Source=iset.db3;Version=3;"))
            {
                m_dbConnection.Open();
            }
            return null;
        }

        public static string sendValidation(string charactername, string mailSender)
        {
            string characterId = UserFunctions.getCharacterIdFromName(charactername);
            try
            {
                string mailText = mailSender + " has requested this character to be linked to their discord account. If this was not done by you, please report this to staff immidately. If this was done by you, please enter the following text in discord: !validate ASFA";
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 0, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@CID", characterId);
                    oCmd.Parameters.AddWithValue("@ItemClassEx", "cooking_egg");
                    oCmd.Parameters.AddWithValue("@Count", 1);
                    oCmd.Parameters.AddWithValue("@MailContent", mailText);
                    oCmd.Parameters.AddWithValue("@MailTitle", "Iset Account Validation");
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
