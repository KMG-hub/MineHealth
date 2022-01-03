#define original
//#define usingdll
using System;
using System.IO;
using System.Net.Sockets;
using MineHealthAPI;
namespace MineHealthClient
{
    class Client
    {
        static void Main(string[] args)
        {
#if (usingdll)

            //MHAPI.Connect("211.104.146.87", 9090);
            MHAPI.Connect("127.0.0.1", 9090);

            string strRecvMsg;
            while (MHAPI.IsConnected)
            {

            }
#elif (original)
try
            {
                string strRecvMsg;
                string strSendMsg;


                //TcpClient sockClient = new TcpClient("211.104.146.87", 9090); //소켓생성,커넥트
                TcpClient sockClient = new TcpClient("minehealth.awesomeserver.kr", 9090); //소켓생성,커넥트
                //TcpClient sockClient = new TcpClient("127.0.0.1", 9090); //소켓생성,커넥트

                NetworkStream ns = sockClient.GetStream();
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);

                strRecvMsg = sr.ReadLine();         //서버로부터 접속 성공 메시지 수신
                Console.WriteLine(strRecvMsg);

                while (true)
                {
                    strSendMsg = Console.ReadLine();

                    sw.WriteLine(strSendMsg);
                    sw.Flush();
                    if (strSendMsg == "exit")
                    {
                        break;
                    }
                    strRecvMsg = sr.ReadLine();
                    Console.WriteLine(strRecvMsg);
                }
                sr.Close();
                sw.Close();
                ns.Close();
                sockClient.Close();

                Console.WriteLine("접속 종료!");
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
#endif


        }
    }
}
