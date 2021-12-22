using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace MineHealthDLL_NetFrame
{
    public static class MHAPI
    {
        public const string ServerIP = "127.0.0.1";   // default: 127.0.0.1
        public const int ServerPort = 9090;           // default: 9090

        private static TcpClient client = null;
        private static NetworkStream ns = null;
        private static StreamReader sr = null;
        private static StreamWriter sw = null;


        private static bool _IsConnected = false;
        public static bool IsConnected
        {
            set
            {
                _IsConnected = value;
            }
            get
            {
                return _IsConnected;
            }
        }

        /// <summary>
        /// Server에 연결, Connect(서버IP, 포트)
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="port"></param>
        /// <returns>성공메세지 또는 실패메세지 출력</returns>
        public static string Connect(string serverIP, int port)
        {
            try
            {
                client = new TcpClient(serverIP, port);
                ns = client.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);

                
                IsConnected = true;

                return sr.ReadLine();
            }
            catch (SocketException ex)
            {
                IsConnected = false;
                return ex.Message;
            }
        }

        /// <summary>
        /// 서버 연결 해제, DisConnect()
        /// </summary>
        public static void DisConnect()
        {
            if (IsConnected)
            {
                sr.Close();
                sw.Close();
                ns.Close();
                client.Close();
            }
        }

        /// <summary>
        /// 로그인 API, GetLogIn(핸드폰번호, 비밀번호)
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Password"></param>
        /// <returns> 시스템에러: -1, <para/>로그인 성공: 0, <para/>비밀번호가 다름: 1, <para/>존재하지 않는 번호: 2 </returns>
        public static int GetLogIn(string Phone, string Password)
        {
            int result = -1;
            if (IsConnected)
            {
                string strRecvMsg = null;
                //sr.ReadToEnd();
                sw.WriteLine("LOGIN" + Phone + "," + Password);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "OK":
                        result = 0;
                        break;
                    case "Failed, 1":
                        result = 1;
                        break;
                    case "Failed, 2":
                        result = 2;
                        break;
                    default:
                        result = -1;
                        break;
                }
                sw.Flush();
            }

            return result;
        }

        /// <summary>
        /// 회원가입 API, GetSignIn(핸드폰번호, 비밀번호, 생일(yyyy-MM-dd), 이름, 성별(M or F))
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Password"></param>
        /// <param name="Birthday"></param>
        /// <param name="Nickname"></param>
        /// <param name="Gender"></param>
        /// <returns>시스템에러: -1, <para/>회원가입 성공: 0, <para/>비밀번호가 잘못됨: 1, <para/>이미 존재하는 번호: 2, <para/>정보가 부족함: 3</returns>
        public static int GetSignIn(string Phone, string Password, string Birthday, string Nickname, Gender gender)
        {
            int result = -1;
            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("SIGNIN" + Phone + "," 
                    + Password + "," 
                    + Birthday + ","
                    + Nickname + ","
                    + gender.ToString());
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "OK":
                        result = 0;
                        break;
                    case "Failed, 1":
                        result = 1;
                        break;
                    case "Failed, 2":
                        result = 2;
                        break;
                    case "Failed, 3":
                        result = 3;
                        break;
                    default:
                        result = -1;
                        break;
                }

                Console.WriteLine(strRecvMsg);
                sw.Flush();
            }

            return result;
        }

        /// <summary>
        /// 유저 정보를 조회 API, GetUserInfo(핸드폰번호)
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns>
        /// 시스템에러: -1, <para/>비밀번호가 잘못됨: 1, <para/>정상적으로 조회: 핸드폰번호,이름,생일,성별 순으로 출력
        /// </returns>
        public static string GetUserInfo(string Phone)
        {
            string result = "-1";
            if (IsConnected)
            {
                string strRecvMsg = null;
                //sr.ReadToEnd();
                sw.WriteLine("USERINFO" + Phone);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = "-1";
                        break;
                    case "Failed, 1":
                        result = "1";
                        break;
                    default:
                        result = strRecvMsg;
                        break;
                }
                sw.Flush();
            }

            return result;
        }

        /// <summary>
        /// 번호의 중복여부 체크 API, GetUserDuplication(핸드폰번호)
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns>시스템에러: -1, <para/>중복아님: 0, <para/>중복: 1</returns>
        public static int GetUserDuplication(string Phone)
        {
            int result = -1;
            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("DUPLICATION" + Phone);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = -1;
                        break;
                    case "0":
                        result = 0;
                        break;
                    default:
                        result = 1;
                        break;
                }
                sw.Flush();
            }

            return result;
        }

        /// <summary>
        /// 유저정보 수정 API, SetUserInfoUpdate(핸드폰번호, 비밀번호, 생일, 닉네임, 성별)<para/>
        /// 핸드폰번호와 비밀번호가 일치해야 수정 가능
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Password"></param>
        /// <param name="Birthday"></param>
        /// <param name="Nickname"></param>
        /// <param name="gender"></param>
        /// <returns>시스템에러: -1 <para/> 수정성공: 0 <para/> 수정실패: 1</returns>
        public static int SetUserInfoUpdate(string Phone, string Password, string Birthday, string Nickname, Gender gender)
        {
            int result = -1;

            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("UPDATE" + Phone);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = -1;
                        break;
                    case "0":
                        result = 0;
                        break;
                    default:
                        result = 1;
                        break;
                }
                sw.Flush();
            }



            return result;
        }

        /// <summary>
        /// 유저 테스트일자 호출 API, GetUserTestDate(핸드폰번호) or GetUserTestDate(핸드폰번호, 데이터수)
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="number"></param>
        /// <returns>NULL: 시스템 오류 <para/>Length == 0: 데이터없음 <para/> Length > 0 : 데이터 인덱스별로 저장</returns>
        public static string[] GetUserTestDate(string Phone, int number = 0)
        {
            string[] result = null;

            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("USERTESTDATE" + Phone + "," + number.ToString());
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();
                result = strRecvMsg.Split(',');
                sw.Flush();
            }

            return result;
        }

        /// <summary>
        /// 유저 첫번째 설문조사 점수 조회, GetUserFirstQScore(핸드폰번호, 테스트일자)
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="TestDate"></param>
        /// <returns> 시스템오류: -1 <para/> 데이터없음: -2 <para/> 점수: 0 이상 </returns>
        public static int GetUserFirstQScore(string Phone, string TestDate)
        {
            int result = -1;

            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("QSCORE" + "A," + Phone + "," +TestDate);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();

                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = -1;
                        break;
                    case "":
                        result = -2;
                        break;
                    default:
                        result = Convert.ToInt32(strRecvMsg);
                        break;
                }
                sw.Flush();
            }

            return result;
        }

        public static int GetUserSecondQScore(string Phone, string TestDate)
        {
            int result = -1;

            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("QSCORE" + "B," + Phone + "," + TestDate);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();


                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = -1;
                        break;
                    case "":
                        result = -2;
                        break;
                    default:
                        result = Convert.ToInt32(strRecvMsg);
                        break;
                }
                sw.Flush();
            }


            return result;
        }

        public static int GetUserThirdQScore(string Phone, string TestDate)
        {
            int result = -1;

            if (IsConnected)
            {
                string strRecvMsg = null;
                sw.WriteLine("QSCORE" + "C," + Phone + "," + TestDate);
                sw.Flush();
                Task.Delay(100);
                strRecvMsg = sr.ReadLine();


                switch (strRecvMsg)
                {
                    case "Failed, -1":
                        result = -1;
                        break;
                    case "":
                        result = -2;
                        break;
                    default:
                        result = Convert.ToInt32(strRecvMsg);
                        break;
                }
                sw.Flush();
            }


            return result;
        }

        public enum Gender
        {
            M = 0,
            F = 1
        }

        public static object Command(string cmd)
        {
            string strRecvMsg = null;
            if (IsConnected)
            {
                sw.WriteLine(cmd);
                sw.Flush();
                Task.Delay(500);
                strRecvMsg = sr.ReadLine();
            }
            return strRecvMsg;
        }
    }
}
