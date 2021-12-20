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
        /// 유저 정보 조회
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static string RequestUserInfo(string Phone)
        {
            string result = null;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "SELECT Phone, Nickname, Birth, Gender FROM UserInfoTbl WHERE Phone = '" + Phone + "';";
                    Console.WriteLine("Query: " + qry);

                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            result = reader["Phone"].ToString() + "," + reader["Nickname"].ToString() + "," + reader["Birth"].ToString() + "," + reader["Gender"].ToString();
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(DateTime.Now.ToString() + " Error: " + ex.ToString());
                }
            }
            return result;
        }
        /// <summary>
        /// 회원가입 요청
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Pswd"></param>
        /// <param name="Birth"></param>
        /// <param name="Nickname"></param>
        /// <param name="Gender"></param>
        /// <returns> 0:success, -1:fail </returns>
        public static int RequestSignIn(string Phone, string Pswd, string Birth, string Nickname, string Gender)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "INSERT INTO UserInfoTbl(Phone, Pswd, Birth, Nickname, Gender) VALUES ('"
                        + Phone + "', '" + Pswd + "', '" + Birth + "', '" + Nickname + "', '" + Gender + "');";
                    Console.WriteLine("Query: " + qry);

                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
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
        /// <summary>
        /// 회원 정보 수정 요청, 핸드폰번호는 수정 불가.
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Pswd"></param>
        /// <param name="Birth"></param>
        /// <param name="Nickname"></param>
        /// <param name="Gender"></param>
        /// <returns> 0:success, -1:fail </returns>
        public static int RequestUpdateUserInfo(string Phone, string Pswd, string Birth, string Nickname, string Gender)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "UPDATE UserInfoTbl SET";
                    if (!string.IsNullOrEmpty(Pswd)) qry += " Pswd = '" + Pswd + "',";
                    if (!string.IsNullOrEmpty(Birth)) qry += " Birth = '" + Birth + "', ";
                    if (!string.IsNullOrEmpty(Nickname)) qry += " Nickname = '" + Nickname + "', ";
                    if (!string.IsNullOrEmpty(Gender)) qry += " Gender = '" + Gender + "', ";
                    qry.Substring(qry.Length - 2);
                    qry += " WHERE Phone = '" + Phone +"';";

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = Convert.ToInt32(cmd.ExecuteScalar());
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
        /// <summary>
        /// 핸드폰 번호 중복 확인 
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns> 0~:success, -1:fail </returns>
        public static int CheckDuplicationPhone(string Phone)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM UserInfoTbl WHERE Phone = '" + Phone + "';";

                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = Convert.ToInt32(cmd.ExecuteScalar());
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
        /// <summary>
        /// 유저 정보 삭제
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Pswd"></param>
        /// <returns></returns>
        public static int DeleteUserInfo(string Phone, string Pswd)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "DELETE FROM UserInfoTbl WHERE Phone = '" + Phone + "' AND Pswd = '" + Pswd + "';";
                    Console.WriteLine("Query: " + qry);

                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
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


        public static string RequestTestDateTime(string Phone, int number = 0)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "SELECT TestDate FROM UserLogTbl WHERE Phone = '" + Phone + "' ";
                    if (number != 0) qry += "LIMIT " + number.ToString();
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            while (reader.Read())
                            {
                                result += reader[0] + ",";
                            }
                            //if (result.Length > 0)
                            //    result = result.Substring(result.Length - 1);
                        }
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

        public static string RequestScoreQuestion1()
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    //string qry = "SELECT TestDate FROM UserLogTbl WHERE = '" + Phone + "' ";
                    //if (number != 0) qry += "LIMIT " + number.ToString();
                    //Console.WriteLine("Query: " + qry);
                    //conn.Open();
                    //using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    //{
                    //    using (MySqlDataReader reader = cmd.ExecuteReader())
                    //    {
                    //        result = "";
                    //        while (reader.Read())
                    //        {
                    //            result += reader[0] + ",";
                    //        }
                    //        if (result.Length > 0)
                    //            result = result.Substring(result.Length - 1);
                    //    }
                    //}
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
