//#define LocalDB
#define ExternalDB

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
#if (LocalDB)
        private const string ServerIP = "localhost";
        private const string Port = "3306";
#else
        private const string ServerIP = "211.104.146.87";
        private const string Port = "53383";
#endif
        private const string DataBase = "MineHealth";
        private const string Uid = "minehealthsql";
        private const string Pwd = "minehealthsql";
        private const string connStr = "Server=" + ServerIP + ";Port=" + Port + ";Database=" + DataBase + ";Uid=" + Uid + ";Pwd=" + Pwd;

        /// <summary>
        /// 비밀번호 조회, RequestUserPswd(핸드폰번호)
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns>비밀번호, 존재하지 않으면 null </returns>
        public static string RequestUserPswd(string Phone)
        {
            string result = null;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "SELECT Pswd FROM UserInfoTbl WHERE Phone = '" + Phone + "';";
                    Console.WriteLine("Query: " + qry);

                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["Pswd"].ToString();
                            }
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
        /// 유저 정보 조회
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static string RequestUserInfo(string Phone, string Pswd)
        {
            string result = null;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "SELECT Phone, Nickname, Birth, Gender FROM UserInfoTbl WHERE Phone = '" + Phone + "' AND Pswd = '" + Pswd + "';";

                    Console.WriteLine("Query: " + qry);

                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            if (reader.Read())
                            {
                                result = reader["Phone"].ToString() + "," + reader["Birth"].ToString() + "," + reader["Nickname"].ToString() + "," + reader["Gender"].ToString();
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = null;
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
                    if (!string.IsNullOrEmpty(Birth)) qry += " Birth = '" + Birth + "', ";
                    if (!string.IsNullOrEmpty(Nickname)) qry += " Nickname = '" + Nickname + "', ";
                    if (!string.IsNullOrEmpty(Gender)) qry += " Gender = '" + Gender + "'";
                    qry += " WHERE Phone = '" + Phone + "' AND Pswd = '" + Pswd + "';";

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
        /// 비밀번호 변경 요청
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Pswd"></param>
        /// <param name="newPswd"></param>
        /// <returns></returns>
        public static int RequestChangePassword(string Phone, string Pswd, string newPswd)
        {
            int result = -1;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "UPDATE UserInfoTbl SET";
                    qry += " Pswd = '" + newPswd + "' ";
                    qry += " WHERE Phone = '" + Phone + "' AND Pswd = '" + Pswd + "';";

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = Convert.ToInt32(cmd.ExecuteNonQuery());
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

        /// <summary>
        /// 유저 테스트 일자 조회
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string RequestTestDateTime(string Phone, string number = "0")
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {

                try
                {
                    int tempnum = 0;
                    int.TryParse(number, out tempnum);
                    string qry = "SELECT TestDate FROM UserLogTbl WHERE Phone = '" + Phone + "' ORDER BY TestDate DESC ";
                    if (tempnum != 0) qry += "LIMIT " + tempnum.ToString();
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
                            if (result.Length > 0)
                                result = result.Remove(result.Length - 1);
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

        /// <summary>
        /// 유저 테스트 ID 조회
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="TestDate"></param>
        /// <returns></returns>
        public static string RequestTestId(string Phone, string TestDate)
        {
            string result = null;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "SELECT TestId From UserLogTbl WHERE Phone = '" + Phone + "' AND TestDate = '" + TestDate + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            if (reader.Read())
                            {
                                result = reader[0].ToString();
                            }
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

        /// <summary>
        /// 테스트 점수 조회, RequestScoreQuestion(테스트항목, TestId)
        /// </summary>
        /// <param name="Question"></param>
        /// <param name="Phone"></param>
        /// <param name="TestDate"></param>
        /// <returns>점수 반환</returns>
        public static string RequestScore(string Category, string TestId)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string strtemp = "";
                    switch (Category)
                    {
                        case "QA":
                            strtemp = "QuestionATbl";
                            break;
                        case "QB":
                            strtemp = "QuestionBTbl";
                            break;
                        case "QC":
                            strtemp = "QuestionCTbl";
                            break;
                        case "PA":
                            strtemp = "PoseATbl";
                            break;
                        case "PB":
                            strtemp = "PoseBTbl";
                            break;
                        default:
                            return "-1";
                    }


                    string qry = "SELECT Score FROM " + strtemp + " WHERE TestID = '" + TestId + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            if (reader.Read())
                            {
                                result = reader[0].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return "-1";
                }
            }

            return result;
        }

        /// <summary>
        /// 테스트 레벨 조회, RequestLevel(테스트항목, TestId)
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="TestId"></param>
        /// <returns></returns>
        public static string RequestLevel(string Category, string TestId)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string strtemp = "";
                    switch (Category)
                    {
                        case "QA":
                            strtemp = "QuestionATbl";
                            break;
                        case "QB":
                            strtemp = "QuestionBTbl";
                            break;
                        case "QC":
                            strtemp = "QuestionCTbl";
                            break;
                        case "PA":
                            strtemp = "PoseATbl";
                            break;
                        case "PB":
                            strtemp = "PoseBTbl";
                            break;
                        default:
                            return "-1";
                    }

                    string qry = "SELECT Level FROM " + strtemp + " WHERE TestID = '" + TestId + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            if (reader.Read())
                            {
                                result = reader[0].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return "-1";
                }
            }

            return result;
        }

        /// <summary>
        /// 테스트 코멘트 조회, RequestComment(테스트항목, TestId)
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="TestId"></param>
        /// <returns></returns>
        public static string RequestComment(string Category, string TestId)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string strtemp = "";
                    switch (Category)
                    {
                        case "QA":
                            strtemp = "QuestionATbl";
                            break;
                        case "QB":
                            strtemp = "QuestionBTbl";
                            break;
                        case "QC":
                            strtemp = "QuestionCTbl";
                            break;
                        case "PA":
                            strtemp = "PoseATbl";
                            break;
                        case "PB":
                            strtemp = "PoseBTbl";
                            break;
                        default:
                            return "-1";
                    }
                    string qry = "SELECT CommentTbl.Comment FROM CommentTbl JOIN " + strtemp + " AS tbl ON CommentTbl.CommentID = tbl.CommentID WHERE TestID = '" + TestId + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            if (reader.Read())
                            {
                                result = reader[0].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return "-1";
                }
            }

            return result;
        }

        /// <summary>
        /// 유저별 맞춤 유튜브 링크 조회, RequestPersonalLink(테스트항목, TestId)
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="TestId"></param>
        /// <returns></returns>
        public static string RequestPersonalLink(string Category, string TestId)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "";
                    if (Category == "MINE")
                    {
                        qry = "SELECT LINK01 FROM CommentTbl RIGHT OUTER JOIN QuestionATbl ON CommentTbl.CommentID = QuestionATbl.CommentID " +
                            "WHERE QuestionATbl.TestID = '" + TestId + "' ";
                        qry += "UNION ";
                        qry += "SELECT LINK01 FROM CommentTbl RIGHT OUTER JOIN QuestionBTbl ON CommentTbl.CommentID = QuestionBTbl.CommentID " +
                            "WHERE QuestionBTbl.TestID = '" + TestId + "' ";
                        qry += "UNION ";
                        qry += "SELECT LINK01 FROM CommentTbl RIGHT OUTER JOIN QuestionCTbl ON CommentTbl.CommentID = QuestionCTbl.CommentID " +
                            "WHERE QuestionCTbl.TestID = '" + TestId + "' ";
                    }
                    else if (Category == "HEALTH")
                    {
                        qry = "SELECT LINK01 FROM CommentTbl RIGHT OUTER JOIN PoseATbl ON CommentTbl.CommentID = PoseATbl.CommentID " +
                           "WHERE PoseATbl.TestID = '" + TestId + "' ";
                        qry += "UNION ";
                        qry += "SELECT LINK01 FROM CommentTbl RIGHT OUTER JOIN PoseBTbl ON CommentTbl.CommentID = PoseBTbl.CommentID " +
                            "WHERE PoseBTbl.TestID = '" + TestId + "' ";
                    }

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            while (reader.Read())
                            {
                                result += reader[0]?.ToString() + ",";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return "-1";
                }
            }

            return result;
        }


        /// <summary>
        /// 항목별 모든 유튜브 링크 조회, RequestAllLink(테스트항목)
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="TestId"></param>
        /// <returns></returns>
        public static string RequestAllLink(string Category)
        {

            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "";
                    if (Category == "MINE")
                    {
                        qry = "SELECT Link01 FROM CommentTbl WHERE CommentID LIKE 'Q%';";
                    }
                    else
                    {
                        qry = "SELECT Link01 FROM CommentTbl WHERE CommentID LIKE 'P%';";
                    }

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = "";
                            while (reader.Read())
                            {
                                result += reader[0]?.ToString() + ",";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return "-1";
                }
            }

            return result;

        }
        public static int InsertUserLog(string Phone, string TestDate, string TestLocation)
        {
            int result = -1;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "INSERT INTO UserLogTbl (Phone, TestDate, Location)" +
                        "VALUES ('" + Phone + "', '" + TestDate + "', '" + TestLocation + "');";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }
            return result;
        }
        public static int InsertQuestion(string Category, string TestId, List<string> Answer, string Score)
        {
            int result = -1;

            string tableName = "";
            switch(Category)
            {
                case "QA":
                    tableName = "QuestionATbl";
                    if (Answer is null)
                        Answer = new List<string>()
                        {
                            "","","","","","","","","",""
                        };
                    break;
                    
                case "QB":
                    tableName = "QuestionBTbl";
                    if (Answer is null)
                        Answer = new List<string>()
                        {
                            "","","","","","",""
                        };
                    break;

                case "QC":
                    tableName = "QuestionCTbl";
                    Answer = new List<string>()
                        {
                            "","","","","","","","","",""
                        };
                    break;

                default:
                    return -1;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "INSERT INTO " + tableName
                        + " (TestID, Score";
                    
                    
                    for (int i = 0; i < Answer.Count; i++)
                    {
                        qry += "," + "Answer" + i.ToString();
                    }

                    qry += ") VALUES ('" + TestId + "', '" + Score + "";
                    foreach (var item in Answer)
                    {
                        qry += "','" + item;
                    }

                    qry += "');";

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }


            return result;
        }
        public static int InsertEmotion(string Category, string TestId, string path)
        {
            int result = -1;

            string tableName = "";
            if (string.IsNullOrEmpty(path))
                path = "";
            switch (Category)
            {
                case "EA":
                    tableName = "EmotionATbl";
                    break;

                case "EB":
                    tableName = "EmotionBTbl";
                    break;

                case "EC":
                    tableName = "EmotionCTbl";
                    break;
                case "ED":
                    tableName = "EmotionDTbl";
                    break;

                default:
                    return -1;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "INSERT INTO " + tableName
                        + " (TestID, SavePath) VALUES ";

                    qry += "('" + TestId + "', '" + path;
                    qry += "');";

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }


            return result;
        }
        public static int InsertPose(string Category, string TestId, List<string> Answer, string Score)
        {
            int result = -1;


            string tableName = "";
            switch (Category)
            {
                case "PA":
                    tableName = "PoseATbl";
                    break;

                case "PB":
                    tableName = "PoseBTbl";
                    break;
                default:
                    return -1;
            }


            if (Answer is null)
            {
                Answer = new List<string>()
                {
                    "<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>",
                    "<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>",
                    "<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>","<0,0>",
                    "<0,0>","<0,0>","<0,0>","<0,0>"
                };
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = $"INSERT INTO {tableName} (" +
                        $"TestID, Score, Level, CommentID, SavePath, ";
                    List<string> jointsname = new List<string>()
                    {
                        "PELVIS", "SPINE_NAVAL", "SPINE_CHEST", "NECK",
                        "CLAVICLE_LEFT", "SHOULDER_LEFT", "ELBOW_LEFT", "WRIST_LEFT", "HAND_LEFT", "HANDTIP_LEFT", "THUMB_LEFT",
                        "CLAVICLE_RIGHT", "SHOULDER_RIGHT", "ELBOW_RIGHT", "WRIST_RIGHT", "HAND_RIGHT", "HANDTIP_RIGHT", "THUMB_RIGHT",
                        "HIP_LEFT", "KNEE_LEFT", "ANKLE_LEFT", "FOOT_LEFT",
                        "HIP_RIGHT", "KNEE_RIGHT", "ANKLE_RIGHT", "FOOT_RIGHT",
                        "HEAD", "NOSE", "EYE_LEFT", "EAR_LEFT", "EYE_RIGHT", "EAR_RIGHT"
                    };
                    foreach (var joint in jointsname)
                    {
                        qry += $"{joint}, ";
                    }
                    qry += $"SACRUM, C7";
                    qry += $") VALUES (";
                    qry += $"'{TestId}', '{Score}', '', '', '', ";
                    Answer.ForEach(x => qry += $"'{x.Replace(" ", "")}', ");
                    qry = qry.Remove(qry.Length - 2, 2);
                    qry += ");";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }
            

            return result;
        }
        public static int UpdateQuestion(string Category, string TestId, List<string> Answer, string Score)
        {
            int result = -1;

            string tableName = "";
            switch (Category)
            {
                case "QA":
                    tableName = "QuestionATbl";
                    if (Answer is null)
                        Answer = new List<string>()
                        {
                            "","","","","","","","","",""
                        };
                    break;

                case "QB":
                    tableName = "QuestionBTbl";
                    if (Answer is null)
                        Answer = new List<string>()
                        {
                            "","","","","","",""
                        };
                    break;

                case "QC":
                    tableName = "QuestionCTbl";
                    if (Answer is null)
                        Answer = new List<string>()
                        {
                            "","","","","","","","","",""
                        };
                    break;

                default:
                    return -1;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "UPDATE " + tableName + " SET ";

                    for (int i = 0; i < Answer.Count; i++)
                    {
                        qry += "Answer" + i.ToString();
                        qry += " = '" + Answer[i] + "', ";
                    }
                    qry += "Score" + " = '" + Score + "', ";

                    string level = "";
                    string CommentID = "";

                    switch (Category)
                    {
                        case "QA":
                            if (Convert.ToDouble(Score) <= 2)
                                level = "A";
                            else
                                level = "B";
                            break;
                        case "QB":
                            if (Convert.ToDouble(Score) <= 4)
                                level = "A";
                            else
                                level = "B";
                            break;
                        case "QC":
                            if (Convert.ToDouble(Score) <= 10)
                                level = "A";
                            else if (Convert.ToDouble(Score) <= 20)
                                level = "B";
                            else if (Convert.ToDouble(Score) <= 30)
                                level = "C";
                            else
                                level = "D";
                            break;
                    }

                    CommentID = Category + "-" + level;

                    qry += "Level" + " = '" + level + "', ";
                    qry += "CommentID" + " = '" + CommentID + "' ";

                    qry += "WHERE TestID = '" + TestId + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }


            return result;
        }
        public static int UpdateEmotion(string Category, string TestId, string path)
        {
            int result = -1;

            string tableName = "";
            if (string.IsNullOrEmpty(path))
                path = "";
            switch (Category)
            {
                case "EA":
                    tableName = "EmotionATbl";
                    break;

                case "EB":
                    tableName = "EmotionBTbl";
                    break;

                case "EC":
                    tableName = "EmotionCTbl";
                    break;
                case "ED":
                    tableName = "EmotionDTbl";
                    break;

                default:
                    return -1;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "UPDATE " + tableName + " SET ";
                    qry += "TestId = '" + TestId + "', ";
                    qry += "SavePath = '" + path + "' ";
                    qry += "WHERE TestID = '" + TestId + "';";
                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }


            return result;
        }
        public static int UpdatePose(string Category, string TestId, List<string> Answer, string path, string Score)
        {
            int result = -1;
            string tableName = "";
            switch (Category)
            {
                case "PA":
                    tableName = "PoseATbl";
                    break;

                case "PB":
                    tableName = "PoseBTbl";
                    break;
                default:
                    return -1;
            }

            List<string> jointsname = new List<string>()
            {
                 "PELVIS", "SPINE_NAVAL", "SPINE_CHEST", "NECK",
                    "CLAVICLE_LEFT", "SHOULDER_LEFT", "ELBOW_LEFT", "WRIST_LEFT", "HAND_LEFT", "HANDTIP_LEFT", "THUMB_LEFT",
                    "CLAVICLE_RIGHT", "SHOULDER_RIGHT", "ELBOW_RIGHT", "WRIST_RIGHT", "HAND_RIGHT", "HANDTIP_RIGHT", "THUMB_RIGHT",
                    "HIP_LEFT", "KNEE_LEFT", "ANKLE_LEFT", "FOOT_LEFT",
                    "HIP_RIGHT", "KNEE_RIGHT", "ANKLE_RIGHT", "FOOT_RIGHT",
                    "HEAD", "NOSE", "EYE_LEFT", "EAR_LEFT", "EYE_RIGHT", "EAR_RIGHT", "SACRUM", "C7"
            };

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string qry = "UPDATE " + tableName + " SET ";

                    for (int i = 0; i < jointsname.Count; i++)
                    {    
                        qry += jointsname[i] + " = '" + Answer[i] +"', ";
                    }

                    qry += "Score = '" + Score + "', ";
                    qry += "SavePath = " + path + " ";
                    qry += "WHERE TestID = '" + TestId + "'";

                    Console.WriteLine("Query: " + qry);
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qry, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }


            return result;
        }

        public enum ScoreCategory
        {
            FrontAngle = 0,
            SideAngle = 1,
            SideNeck = 2
        }
        public static bool SaveScoreDatas(string TestId, ScoreCategory sc, string score)
        {
            bool result = false;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    string valueqry = "";
                    switch (sc)
                    {
                        case ScoreCategory.FrontAngle:
                            valueqry = "ScoreFA";
                            break;
                        case ScoreCategory.SideAngle:
                            valueqry = "ScoreSA";
                            break;
                        case ScoreCategory.SideNeck:
                            valueqry = "ScoreSN";
                            break;
                        default:
                            return false;
                            break;
                    }

                    string Query = $"UPDATE ResultScoreTbl SET {valueqry} = '{score}' WHERE TestID = '{TestId}'";
                    conn.Open();
                    using (MySqlCommand cmd = new(Query, conn))
                    {
                        if (cmd.ExecuteNonQuery() > 0)
                            result = true;
                    }
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}

//PAINSERT
//29706ffc-67bc-11ec-a9a5-6c0b84653419,
//<123,456>, <123,457>, <123,458>, <123,459>, <123,460>, <123,461>, <123,462>, <123,463>, <123,464>, <123,465>, <123,466>, <123,467>, <123,468>, <123,469>, <123,470>, <123,471>, <123,472>, <123,473>, <123,474>, <123,475>, <123,476>, <123,477>, <123,478>, <123,479>, <123,480>, <123,481>, <123,482>, <123,483>, <123,484>, <123,485>, <123,486>, <123,487>,

//10
