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
        //private const string Port = "53384";
        private const string DataBase = "MineHealth";
        private const string Uid = "minehealthsql";
        private const string Pwd = "minehealthsql";
        private const string connStr = "Server=" + ServerIP + ";Port=" + Port + ";Database=" + DataBase + ";Uid=" + Uid + ";Pwd=" + Pwd;

        /// <summary>
        /// 회원가입 요청
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Pswd"></param>
        /// <param name="Birth"></param>
        /// <param name="Nickname"></param>
        /// <param name="Gender"></param>
        /// <returns>0:success, -1:fail</returns>
        public static int RequestSignIn(string Phone, string Pswd, string Birth, string Nickname, string Gender)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string qry = "INSERT INTO UserInfoTbl(Phone, Pswd, Birth, Nickname, Gender) VALUES ('" 
                        + Phone + "', '" + Pswd + "', '" + Birth + "', '" + Nickname + "', '" + Gender + ");";

                    MySqlCommand cmd = new MySqlCommand(qry, conn);
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    Console.WriteLine("실패");
                    Console.WriteLine(DateTime.Now.ToString() + " Error: " + ex.ToString());
                }
            }

            return result;
        }
        



        public static int CheckDuplicationPhone(string Phone)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection("Server=" + ServerIP + ";Port=" + Port + ";Database=" + DataBase + ";Uid=" + Uid + ";Pwd=" + Pwd))
            {
                try
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM UserInfoTbl WHERE Phone = '" + Phone + "'";
                    MySqlCommand cmd = new MySqlCommand(qry, conn);

                    result = Convert.ToInt32(cmd.ExecuteScalar());
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
