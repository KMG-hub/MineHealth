using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MineHealthHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer();
            server.AddBindingAddress("http://localhost:9092/");

            server.RootPath = "c:\\wwwroot";
            server.ActionRequested += server_ActionRequested;

            server.Start();
            while (true) ;
        }

        private static void server_ActionRequested(object sender, ActionRequestedEventArgs e)
        {
            e.Server.WriteDefaultAction(e.Context);
        }
    }
}

