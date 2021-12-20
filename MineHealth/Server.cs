using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utility;

namespace MineHealth
{
    class Server
    {
        static void Main(string[] args)
        {
            try
            {
                string strMsg;

                TcpListener sockServer = new TcpListener(IPAddress.Any, 9090); //IP, Port
                sockServer.Start();
                Console.WriteLine("Server 시작! Client 연결 대기중...");

                TcpClient client = sockServer.AcceptTcpClient();//Accept
                Console.WriteLine("Client 접속성공!");

                NetworkStream ns = client.GetStream();
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);

                string welcome = "Server Connnect Success!";
                sw.WriteLine(welcome);
                sw.Flush();
                while (true)
                {
                    strMsg = sr.ReadLine();
                    if (strMsg == "exit")  //exit 메시지 수신시 종료하기
                        break;

                    if (strMsg.Contains("USERINFO"))
                    {
                        strMsg = strMsg.Replace("USERINFO", "");
                        strMsg = SQLHelper.RequestUserInfo(strMsg).ToString();
                    }


                    if (strMsg.Contains("SIGNIN"))
                    {
                        strMsg = strMsg.Replace("SIGNIN", "");
                        var splitStr = strMsg.Split(',');
                        strMsg = SQLHelper.RequestSignIn(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]).ToString();
                    }

                    if (strMsg.Contains("UPDATE"))
                    {
                        strMsg = strMsg.Replace("UPDATE", "");
                        var splitStr = strMsg.Split(',');
                        strMsg = SQLHelper.RequestUpdateUserInfo(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]).ToString();
                    }

                    if (strMsg == "DUPLICATION")
                    {
                        strMsg.Replace("DUPLICATION", "");
                        strMsg = SQLHelper.CheckDuplicationPhone(strMsg).ToString();
                    }


                    Console.WriteLine(strMsg);
                    sw.WriteLine(strMsg);
                    sw.Flush();
                }

                sw.Close();
                sr.Close();
                ns.Close();
                sockServer.Stop();

                Console.WriteLine("Client 연결 종료!");
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            //Console.Read();
        }
    }
}
