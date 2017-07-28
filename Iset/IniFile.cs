using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Iset
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    internal class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);


        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(5000);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            5000, this.path);
            return temp.ToString();

        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        private List<string> GetKeys(string iniFile, string category)
        {

            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(category, buffer, 2048, iniFile);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                result.Add(entry.Substring(0, entry.IndexOf("=")));
            }

            return result;
        }
    }
}