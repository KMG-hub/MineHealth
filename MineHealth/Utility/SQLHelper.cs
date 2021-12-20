using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Utility
{
    public static class SQLHelper
    {

        const string ServerIP = "localhost";
        //private const string ServerIP = "211.104.146.87";
        private const string Port = "3306";
        private const string DataBase = "MineHealth";
        private const string Uid = "minehealthsql";
        private const string Pwd = "minehealthsql";

        public static bool CheckDuplicationPhone(string Phone)
        {
            bool result = false;
            using (MySqlConnection conn = new MySqlConnection("Server=" + ServerIP + ";Port=" + Port + ";Database=" + DataBase + ";Uid=" + Uid + ";Pwd=" + Pwd))
            {
                try
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM UserInfoTbl Where = '" + Phone + "'";
                    MySqlCommand cmd = new MySqlCommand(qry, conn);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }


    }
}
