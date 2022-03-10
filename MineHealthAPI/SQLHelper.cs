using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineHealthAPI
{
    public class SQLHelper 
    {
        private const string ServerIP = "211.104.146.87";
        private const string Port = "53383";
        private const string DataBase = "MineHealth";
        private const string Uid = "minehealthsql";
        private const string Pwd = "minehealthsql";
        private const string connStr = "Server=" + ServerIP + ";Port=" + Port + ";Database=" + DataBase + ";Uid=" + Uid + ";Pwd=" + Pwd;
       
        //"SELECT Phone, TestID, TestDate, CAST(ULT.TestDate AS DATETIME) AS DT FROM UserLogTbl AS ULT WHERE Phone = '01077777777' ORDER BY ABS(TIMESTAMPDIFF(SECOND, TestDate, '2022-02-09 10:19:25'));"
        public static string[] GetScore(ScoreCategory category, string phone, string TestDate)
        {
            string[] result = new string[3];
            string columnname = "";
            switch (category)
            {
                case ScoreCategory.FrontSpine:
                    columnname = "ScoreFA";
                    break;
                case ScoreCategory.SideNeck:
                    columnname = "ScoreSN";
                    break;
                case ScoreCategory.SideSpine:
                    columnname = "ScoreSA";
                    break;
            }


            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    //string query = $"SELECT {columnname}, TestDate FROM ResultScoreTbl WHERE TestID = (" +
                    //    $"SELECT TestID FROM UserLogTbl WHERE Phone = '{phone}' " +
                    //    $"ORDER BY ABS(TIMESTAMPDIFF(SECOND, CAST(TestDate AS DATETIME), '{TestDate}')) LIMIT 1);";

                    string query = $"SELECT RST.{columnname}, ULT.Phone, ULT.TestDate FROM ResultScoreTbl AS RST " +
                        $"INNER JOIN UserLogTbl AS ULT ON RST.TestID = ULT.TestID " +
                        $"WHERE RST.TestID = (SELECT TestID FROM UserLogTbl " +
                        $"WHERE Phone = '{phone}' ORDER BY ABS(TIMESTAMPDIFF(SECOND, CAST(TestDate AS DATETIME), '{TestDate}')) LIMIT 1);";


                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result[0] = reader[columnname].ToString();
                                result[1] = reader["Phone"].ToString();
                                result[2] = reader["TestDate"].ToString();
                            }
                            reader.Close();
                        }
                    }
                }
                catch
                {

                }
            }


            return result;
        }

        public enum ScoreCategory
        {
            FrontSpine,
            SideNeck,
            SideSpine
        }
    }
}
