using System;
using System.IO;
using System.Net.Sockets;

namespace MineHealthAPI
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
        public static void Connect(string serverIP, int port)
        {
            try
            {
                client = new TcpClient(serverIP, port);
                ns = client.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);

                IsConnected = true;
            }
            catch(SocketException ex)
            {
                IsConnected = false;
                Console.WriteLine(ex.Message);
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
                string strRecvMsg;
                while (true)
                {
                    sw.WriteLine("LOGIN" + Phone + "," + Password);
                    strRecvMsg = sr.ReadLine();

                    switch(strRecvMsg)
                    {
                        case "OK":
                            result = 0;
                            break;
                        case "FAILE1":
                            result = 1;
                            break;
                        case "FAILE2":
                            result = 2;
                            break;
                        default:
                            result = -1;
                            break;
                    }
                }
            }
            return result;
        }


        
        
    }
}
