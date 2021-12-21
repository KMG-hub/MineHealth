using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utility;

namespace MineHealth
{
    class Server
    {

        static int clientCount = 0;
        static TcpClient client = null;

        static bool IsServer = true;
        static void Main(string[] args)
        {
            try
            {
                TcpListener sockServer = new TcpListener(IPAddress.Any, 9090); //IP, Port
                while (IsServer)
                {
                    sockServer.Start();
                    Console.WriteLine("Server 시작! Client 연결 대기중...");

                    client = sockServer.AcceptTcpClient();//Accept
                    Console.WriteLine("Client#{0} 접속성공!", ++clientCount);

                    Thread thread = new(initServerDo);
                    thread.Start();
                }

                sockServer.Stop();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }


        }

        static void initServerDo()
        {
            ServerDo serverDo = new ServerDo(client, clientCount);
            serverDo.ShutDown += ServerDo_ShutDown;
            serverDo.ClientExit += ServerDo_ClientExit;
        }

        private static void ServerDo_ClientExit(object sender, EventArgs e)
        {
            clientCount--;
        }

        private static void ServerDo_ShutDown(object sender, EventArgs e)
        {
            IsServer = false;
        }
    }


    public class ServerDo
    {
        private TcpClient client;
        private int clientNo;
        string strMsg;

        public ServerDo(TcpClient client, int clientno)
        {
            this.client = client;
            this.clientNo = clientno;

            Running();
        }

        public event EventHandler ShutDown;
        public event EventHandler ClientExit;
        private void Running()
        {

            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            string welcome = "Server Connnect Success!";
            sw.WriteLine(welcome);
            sw.Flush();
            while (true)
            {
                strMsg = sr.ReadLine();
                Console.WriteLine("me: {0}", strMsg);
                if (strMsg == "exit")  //exit 메시지 수신시 종료하기
                {
                    ClientExit?.Invoke(client, new EventArgs());
                    break;
                }
                if (strMsg == "shutdown")
                {
                    ShutDown?.Invoke(client, new EventArgs());
                }

                // 로그인 여부, LOGIN[핸드폰번호],[비밀번호]
                if (strMsg.Contains("LOGIN"))
                {
                    strMsg = strMsg.Replace("LOGIN", "");
                    var splitStr = strMsg.Split(',');
                    if (splitStr.Length == 2)
                    {
                        strMsg = SQLHelper.RequestUserPswd(splitStr[0]);
                        if (string.IsNullOrEmpty(strMsg))
                        {
                            strMsg = "Failed, 2";
                        }
                        else if (strMsg == splitStr[1])
                        {
                            strMsg = "OK";
                        }
                        else
                        {
                            strMsg = "Failed, 1";
                        }
                    }
                    else
                        strMsg = "Failed, -1";
                }

                // 회원가입: SIGNIN[핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                if (strMsg.Contains("SIGNIN"))
                {
                    strMsg = strMsg.Replace("SIGNIN", "");
                    var splitStr = strMsg.Split(',');
                    if (SQLHelper.CheckDuplicationPhone(splitStr[0]) > 0)
                    {
                        strMsg = "Failed, 2";
                    }
                    else if (splitStr.Length == 5)
                    {
                        var temp = SQLHelper.RequestSignIn(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);
                        if (temp == -1)
                            strMsg = "Failed, -1";
                        else
                            strMsg = "OK";
                    }
                    else
                    {
                        strMsg = "Failed, 3";
                    }
                }

                // 유저정보 조회, USERINFO[핸드폰번호]
                if (strMsg.Contains("USERINFO"))
                {
                    strMsg = strMsg.Replace("USERINFO", "");
                    var strTemp = SQLHelper.RequestUserInfo(strMsg);
                    if (strTemp is null)
                        strMsg = "Failed, -1";
                    else if (strTemp == string.Empty)
                        strMsg = "Failed, 1";
                    else
                        strMsg = strTemp;
                }
             
                // 회원정보수정: UPDATE[핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                if (strMsg.Contains("UPDATE"))
                {
                    strMsg = strMsg.Replace("UPDATE", "");
                    var splitStr = strMsg.Split(',');

                    if (splitStr.Length == 5)
                    {
                        var temp = SQLHelper.RequestUpdateUserInfo(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);

                        if (temp == -1)
                            strMsg = "UPDATE Failed";
                        else
                            strMsg = "UPDATE Success.";
                    }
                    else
                    {
                        strMsg = "This is Incorrect Information.";
                    }
                }
                // 등록된 핸드폰번호 중복검사: DUPLICATION[핸드폰번호]
                if (strMsg.Contains("DUPLICATION"))
                {
                    strMsg = strMsg.Replace("DUPLICATION", "");
                    strMsg = "Count: " + SQLHelper.CheckDuplicationPhone(strMsg).ToString();
                }
                // 테스트 일자 호출: USERTESTDATE[핸드폰번호] 또는 USERTESTDATE[핸드폰번호],[조회 수]
                if (strMsg.Contains("USERTESTDATE"))
                {
                    strMsg = strMsg.Replace("USERTESTDATE", "");
                    var splitStr = strMsg.Split(',');
                    if (splitStr.Length == 2)
                        strMsg = SQLHelper.RequestTestDateTime(splitStr[0], Convert.ToInt32(splitStr[1])).ToString();
                    else if (splitStr.Length == 1)
                        strMsg = SQLHelper.RequestTestDateTime(splitStr[0]).ToString();
                    else
                        strMsg = "This is Incorrect Information.";
                }

                SendMessage(strMsg);
                sw.WriteLine(strMsg);
                sw.Flush();
            }

            sw.Close();
            sr.Close();
            ns.Close();

            SendMessage("Client 연결 종료!");
        }


        public void SendMessage(string msg)
        {
            Console.WriteLine("client#{0}: {1}", clientNo, msg);
        }
    }
}
