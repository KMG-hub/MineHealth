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

                    Thread thread = new Thread(initServerDo);
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
            serverDo.ClientExit += ServerDo_ClientExit;
            serverDo.DoStart();
        }

        private static void ServerDo_ClientExit(object sender, EventArgs e)
        {
            clientCount--;
        }
    }

}
