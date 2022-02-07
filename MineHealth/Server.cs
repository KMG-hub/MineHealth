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
                    WriteLine("Server 시작! Client 연결 대기중...");

                    client = sockServer.AcceptTcpClient();//Accept
                    WriteLine(string.Format("Client#{0} 접속성공!", ++clientCount));

                    Thread thread = new Thread(initServerDo);
                    thread.Start();
                }

                sockServer.Stop();
            }
            catch (SocketException e)
            {
                WriteLine(e.ToString());
            }


        }

        static void initServerDo()
        {
            ServerDo serverDo = new ServerDo(client, clientCount);
            serverDo.ClientExit += ServerDo_ClientExit;
            serverDo.DoStart();
        }

        private static void ServerDo_ClientExit(object sender, EventArgs e)
        {
            clientCount--;
        }

        private static void WriteLine(string msg)
        {
            Console.WriteLine("{0}, {1}", msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        }
    }

}
