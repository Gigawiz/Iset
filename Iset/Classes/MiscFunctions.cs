using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public static string colorConvert(string R, string G, string B)
        {
            int resultType = 0;
            int decimalColor = 0;
            string baseInput = "";

            if (R != "" && G != "" && B != "")
            {
                // This might be an RGB color separated by spaces (Pre-validation)
                int r, g, b;
                bool flag1, flag2, flag3;
                flag1 = int.TryParse(R, out r);
                flag2 = int.TryParse(G, out g);
                flag3 = int.TryParse(B, out b);
                if (flag1 && flag2 && flag3)
                {
                    // This might be an RGB color separated by spaces (Pre-range validation)
                    if (r > -1 && r < 256 && g > -1 && g < 256 && b > -1 && b < 256)
                    {
                        // This is an RGB color separated by spaces (Post-validation)
                        baseInput = r + "/" + g + "/" + b;
                        String R1 = Convert.ToString(r, 2);
                        String G1 = Convert.ToString(g, 2);
                        String B1 = Convert.ToString(b, 2);
                        // Concatenating together all three binary values, and converting back to int to get the decimal value
                        decimalColor = Convert.ToInt32(R1 + G1 + B1, 2);
                    }
                    else
                    {
                        // Invalid color input syntax
                        resultType = 2;
                    }
                }
                else
                {
                    // Invalid color input syntax
                    resultType = 2;
                }
            }
            else
            {
                string arg1 = R;
                if (arg1.Split('/').Length == 3)
                {
                    // This is an RGB color separated by forward slashes
                    baseInput = R;
                    // Splitting the hex into R, G, and B values
                    string[] values = arg1.Split('/');
                    // Then subsequently converting each into binary string versions
                    string R2 = Convert.ToString(Convert.ToInt32(values[0]), 2);
                    string G2 = Convert.ToString(Convert.ToInt32(values[1]), 2);
                    string B2 = Convert.ToString(Convert.ToInt32(values[2]), 2);
                    // Concatenating together all three binary values, and converting back to int to get the decimal value
                    decimalColor = Convert.ToInt32(R2 + G2 + B2, 2);
                }
                else if (arg1.IndexOf("#") != -1)
                {
                    // This is a hexadecimal color
                    baseInput = R;
                    string colorIn = arg1.Split('#')[1];
                    if (colorIn.Length != 6)
                    {
                        // Invalid color input syntax
                        resultType = 2;
                    }
                    else
                    {
                        // Splitting the hex into R, G, and B values
                        // Then subsequently converting into binary string versions
                        string R1 = Convert.ToString(int.Parse(colorIn.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), 2);
                        string G1 = Convert.ToString(int.Parse(colorIn.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), 2);
                        string B1 = Convert.ToString(int.Parse(colorIn.Substring(4, 2), System.Globalization.NumberStyles.HexNumber), 2);
                        // Concatenating together all three binary values, and converting back to int to get the decimal value
                        decimalColor = Convert.ToInt32(R1 + G1 + B1, 2);
                    }
                }
                else
                {
                    // This is an invalid color format
                    resultType = 2;
                }
            }
            // Returning a result
            switch (resultType)
            {
                case 0:
                    // The function is completing successfully
                    return $"[Color] The color {baseInput} is {decimalColor} in decimal.";
                case 1:
                    // No argument was provided
                    return "[Error] You must supply a color value to convert!";
                case 2:
                    // Invalid color input syntax
                    return "[Error] The color that you provided is not a valid format! This function supports the following:"
                        + "\nRGB: 255 255 255\nRGB: 255/255/255\nHexadecimal: #FFFFFF\nPlease correct your input and try again.";
                default:
                    return "[Error] An unknown error has occurred.";
            }

        }
    }
}
