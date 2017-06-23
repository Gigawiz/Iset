using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class MiscFunctions
    {
        static SqlConnection conn;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");

        public static string clearSoaps()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesShare; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM [ChannelBuff]";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "Soaps have been cleared!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string onlinePlayers2(bool listAll = false, bool useNew = false)
        {
            string restsr = "Error retrieving online players";
            if (onlinePlayers().Count() == 0)
                return "Players Online: 0 at Time: " + DateTime.Now;
            if (useNew)
            {
                try
                {
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesLog; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                        string oString = "SELECT TOP 1 * FROM UserCountLog ORDER BY [TIMESTAMP] ASC";
                        SqlCommand oCmd = new SqlCommand(oString, conn);
                        conn.Open();
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            while (oReader.Read())
                            {
                                restsr = "Players Online: " + oReader["usercount"].ToString() + " at time " + oReader["TIMESTAMP"].ToString() + Environment.NewLine + "Waiting : " + oReader["Wait"].ToString() + Environment.NewLine + "Quest : " + oReader["Quest"].ToString();
                            }
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                restsr = "Players Online: " + onlinePlayers().Count().ToString() + " at time " + DateTime.Now.ToString();
            }
            if (listAll)
            {
                string userstr = "";
                foreach (string username in onlinePlayers())
                {
                    if (userstr == "")
                    {
                        userstr = username;
                    }
                    else
                    {
                        userstr = userstr + ", " + username;
                    }
                }
                restsr = restsr + Environment.NewLine + userstr;
            }
            return restsr;
        }

        public static List<string> onlinePlayers()
        {
            List<string> onlineNames = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE IsConnected=1 ORDER BY Name ASC";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            onlineNames.Add(oReader["Name"].ToString());
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                onlineNames.Add("Error in query!");
                onlineNames.Add(ex.Message);
            }
            return onlineNames;
        }

        public static string listEnchants()
        {
            return "```ADVENTURER | Explorer's\nBALANCED | Well-balanced\nBITTER | Relentless\nHEAVY_RAIN = Chaos\nCRESENT | Crescent Moonlight's\nCURIOUS | Warlord's\nDELICATE | Subdued\nDURATION | Consistent\nENLIGHTENMENT | Enlightened\nFRESH | Fresh\nINJUSTICE | Immoral\nJUSTICE | Righteous\nLEOPARD | Leopard's\nORNATE | Ornate\nREMEMBER | Memorable\nREPEATSAY | Reinforced\nRESOLUTION | Adjusted\nRICH | Enhanced\nSIGNIFICANT | Significant\nSILENT | Silent\nSPEEDY | Fast\nSTARLIGHT | Starlight\nSTEADY | Enduring\nSTOUT | Strong\nSTRONG_BODY | Resilient\nTHRILLING | Tricky\nTIME | Temporal\nTUTELARY | Tutelary\nTWINKLE | Twinkling\nUNDEAD | The Dead\nUNDERDONE | Gallant\nAMBITION | Maelstrom\nANNOUNCEMENT | Declarative\nCHANCE | Chance\nCOAT | Dominance\nCONVICT | Spirited\nDIAMOND | Diamond\nECHO | Echoing\nEXPEDITION | Expeditionary\nFALLENLEAF | Sentinel\nFATAL | Deadly\nFERVOR | Enthusiastic\nFIGHT | Resistant\nITINERARY | Journeying\nJASMINE | Force\nJUDGMENT | Judgment\nMIND | Berserker\nNOBLE | Incorruptible\nPASSION | Passion\nPETAL | Bloodlust\nPROTECTION | Master\nPUNISH | Divine Punishment\nWISE_MAN | Sage's\nSTIGMA | Stigma\nVALOR | Valor\n```";
        }

        public static string listInfusions()
        {
            return "```Infusions: \nATK \nPVP_ATK \nMATK \nPVP_MATK \nBalance \nCritical \nATK_Speed \nATK_Range \nATK_Absolute \nDEF \nPVP_DEF \nDEF_Absolute \nDEF_Destroyed \nSTR \nDEX \nINT \nWILL \nLUCK \nHP \nSTAMINA \nRes_Critical \nTOWN_SPEED \n```";
        }

        public static string colorConvert(string r, string g = null, string b = null)
        {
            int newr = -1;
            int newg = -1;
            int newb = -1;
            if (String.IsNullOrEmpty(g) && String.IsNullOrEmpty(b))
            {
                //assume it was given with /'s instead of an RGB format
                string[] rgb = r.Split('/');
                if (!int.TryParse(rgb[0], out newr))
                {
                    return "Failed to get R Value from input!";
                }
                if (!int.TryParse(rgb[1], out newg))
                {
                    return "Failed to get G Value from input!";
                }
                if (!int.TryParse(rgb[2], out newb))
                {
                    return "Failed to get B Value from input!";
                }

            }
            else
            {
                if (!int.TryParse(r, out newr))
                {
                    return "Failed to get R Value from input!";
                }
                if (!int.TryParse(g, out newg))
                {
                    return "Failed to get G Value from input!";
                }
                if (!int.TryParse(b, out newb))
                {
                    return "Failed to get B Value from input!";
                }
            }
            if (newr >= 0 && newg >= 0 && newb >= 0)
            {
                string s = newr.ToString("X2") + newg.ToString("X2") + newb.ToString("X2");
                int num = int.Parse(s, NumberStyles.HexNumber);
                return "Hex: #" + s + " Decimal: " + (object)num;
            }
            else
            {
                return "Invalid data given!";
            }
        }
    }
}
