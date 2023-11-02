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

        internal static Dictionary<string, string> partyMembers()
        {
            Dictionary<string, string> partymembers = new Dictionary<string, string>();
            foreach (KeyValuePair<string,string> kvp in microPlayInstances())
            {
                if (partymembers.ContainsKey(kvp.Value))
                {
                    partymembers[kvp.Value] = partymembers[kvp.Value] + ", " + UserFunctions.getCharacterNameFromID(kvp.Key);
                }
                else
                {
                    partymembers.Add(kvp.Value, UserFunctions.getCharacterNameFromID(kvp.Key));
                }
            }
            return partymembers;
        }

        public static Dictionary<string, string> microPlayInstances()
        {
            Dictionary<string, string> MicroPlayInstances = new Dictionary<string, string>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                string oString = "SELECT * FROM MicroPlayEntity where convert(varchar(10), UpdateTime, 102) = convert(varchar(10), getdate(), 102)";
                SqlCommand oCmd = new SqlCommand(oString, conn);
                conn.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        MicroPlayInstances.Add(oReader["CID"].ToString(), oReader["PartyID"].ToString());
                    }
                    conn.Close();
                }
            }
            return MicroPlayInstances;
        }

        public static string onlinePlayers2(string listcase = null)
        {
            string restsr = "Error retrieving online players";
            if (onlinePlayers().Count() == 0)
                return "Players Online: 0 at Time: " + DateTime.Now;
            Dictionary<string, string> microplayinstances = microPlayInstances();
            int dungeonCount = 0;
            int totalCount = onlinePlayers().Count();
            string userstr = "";
            if (listcase == "all" || listcase == "list")
            {
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
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroesLog; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT TOP 1 * FROM UserCountLog ORDER BY [TIMESTAMP] DESC ";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            int.TryParse(oReader["Quest"].ToString(), out dungeonCount);
                            int boatCount = dungeonCount - microplayinstances.Count();
                            if (boatCount < 0) boatCount = 0;
                            if (listcase == "all")
                            {
                                restsr = Templates.OnlineTemplate(totalCount.ToString(), (totalCount - dungeonCount).ToString(), microplayinstances.Count().ToString(), microplayinstances.Values.Distinct().Count().ToString(), boatCount.ToString(), oReader["Fish"].ToString(), oReader["PVP_FMatch"].ToString(), oReader["PVP_Arena"].ToString(), oReader["PVP_MMatch"].ToString(), oReader["PVP_PMatch"].ToString(), userstr, partyMembers());
                            }
                            else if (listcase == "list")
                            {
                                restsr = Templates.OnlineTemplate(totalCount.ToString(), (totalCount - dungeonCount).ToString(), microplayinstances.Count().ToString(), microplayinstances.Values.Distinct().Count().ToString(), boatCount.ToString(), oReader["Fish"].ToString(), oReader["PVP_FMatch"].ToString(), oReader["PVP_Arena"].ToString(), oReader["PVP_MMatch"].ToString(), oReader["PVP_PMatch"].ToString(), userstr);
                            }
                            else if (listcase == "parties")
                            {
                                restsr = Templates.OnlineTemplate(totalCount.ToString(), (totalCount - dungeonCount).ToString(), microplayinstances.Count().ToString(), microplayinstances.Values.Distinct().Count().ToString(), boatCount.ToString(), oReader["Fish"].ToString(), oReader["PVP_FMatch"].ToString(), oReader["PVP_Arena"].ToString(), oReader["PVP_MMatch"].ToString(), oReader["PVP_PMatch"].ToString(), null, partyMembers());
                            }
                            else
                            {
                                restsr = Templates.OnlineTemplate(totalCount.ToString(), (totalCount - dungeonCount).ToString(), microplayinstances.Count().ToString(), microplayinstances.Values.Distinct().Count().ToString(), boatCount.ToString(), oReader["Fish"].ToString(), oReader["PVP_FMatch"].ToString(), oReader["PVP_Arena"].ToString(), oReader["PVP_MMatch"].ToString(), oReader["PVP_PMatch"].ToString());
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
