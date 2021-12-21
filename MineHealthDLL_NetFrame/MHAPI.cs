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
        /// Server에 연결
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="port"></param>
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
        /// <returns> 시스템에러:-1, 로그인 성공:0, 비밀번호가 다름:1, 존재하지 않는 번호:2 </returns>
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
                    case "1":
                        result = 1;
                        break;
                    case "2":
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
        /// <returns>시스템에러:-1, 회원가입 성공:0, 비밀번호가 잘못됨:1, 이미 존재하는 번호:2, 정보가 부족함:3</returns>
        public static int GetSignIn(string Phone, string Password, string Birthday, string Nickname, Gender gender)
        {
            int result = -1;
            if (IsConnected)
            {
                string strRecvMsg = null;
                //sr.ReadToEnd();
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
        /// 유저 정보를 조회, USERINFO핸드폰번호
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns>
        /// 시스템에러:-1, 비밀번호가 잘못됨:1, 정상적으로 조회:
        /// 핸드폰번호,이름,생일,성별 순으로 출력
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


        public enum Gender
        {
            M = 0,
            F = 1
        }
    }
}
