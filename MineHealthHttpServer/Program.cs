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
            //WebServer server = new WebServer();
            //server.AddBindingAddress("http://minehealth.awesomepia.kr:9092/");

            //server.RootPath = "c:\\wwwroot";
            //server.ActionRequested += server_ActionRequested;

            //server.Start();
            //while (true) ;


            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".

            //if (args == null || args.Length == 0)
            //    throw new ArgumentException("args");

            string read = Console.ReadLine();
            if (read == null || read.Length == 0)
                throw new ArgumentException("args");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            //foreach (string s in read)
            //{
            //    listener.Prefixes.Add(s);
            //}

            listener.Prefixes.Add(read);
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            listener.Stop();
        }

        //private static void server_ActionRequested(object sender, ActionRequestedEventArgs e)
        //{
        //    e.Server.WriteDefaultAction(e.Context);
        //}
    }
}

