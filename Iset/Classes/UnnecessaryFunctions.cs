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
    class UnnecessaryFunctions
    {
        static SqlConnection conn;
        static SQLiteConnection m_dbConnection;
        static IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");

        public static string giveTannaTits(string charactername)
        {

            return null;
        }
    }
}
