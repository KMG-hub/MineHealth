using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineHealthHttpServer
{
    class Program
    {
        public static HttpListener listener;
        public static string url = "http://127.0.0.1:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }


        //static int clientCount = 0;
        //static TcpClient client = null;

        //static bool IsServer = true;
        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        TcpListener sockServer = new TcpListener(IPAddress.Any, 9092); //IP, Port
        //        while (IsServer)
        //        {
        //            sockServer.Start();
        //            WriteLine("Server 시작! Client 연결 대기중...");

        //            client = sockServer.AcceptTcpClient();//Accept
        //            WriteLine(string.Format("Client#{0} 접속성공!", ++clientCount));

        //            Thread thread = new Thread(initServerDo);
        //            thread.Start();
        //        }

        //        sockServer.Stop();
        //    }
        //    catch (SocketException e)
        //    {
        //        WriteLine(e.ToString());
        //    }
        //}

        //static void initServerDo()
        //{
        //    NetworkStream ns = client.GetStream();
        //    StreamReader sr = new StreamReader(ns);
        //    StreamWriter sw = new StreamWriter(ns);

        //    while (true)
        //    {
        //        var strMsg = sr.ReadLine();
        //        //WriteLine(strMsg);
        //        Debug.WriteLine(strMsg);

        //    }
        //    //ServerDo serverDo = new ServerDo(client, clientCount);
        //    //serverDo.ClientExit += ServerDo_ClientExit;
        //    //serverDo.DoStart();
        //}
        //private static void WriteLine(string msg)
        //{
        //    Console.WriteLine("{0}, {1}", msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        //}
        //static void Main(string[] args)
        //{
        //    //WebServer server = new WebServer();
        //    //server.AddBindingAddress("http://minehealth.awesomepia.kr:9092/");

        //    //server.RootPath = "c:\\wwwroot";
        //    //server.ActionRequested += server_ActionRequested;

        //    //server.Start();
        //    //while (true) ;


        //    if (!HttpListener.IsSupported)
        //    {
        //        Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
        //        return;
        //    }
        //    // URI prefixes are required,
        //    // for example "http://contoso.com:8080/index/".

        //    //if (args == null || args.Length == 0)
        //    //    throw new ArgumentException("args");

        //    string read = Console.ReadLine();
        //    if (read == null || read.Length == 0)
        //        throw new ArgumentException("args");

        //    // Create a listener.
        //    HttpListener listener = new HttpListener();
        //    // Add the prefixes.
        //    //foreach (string s in read)
        //    //{
        //    //    listener.Prefixes.Add(s);
        //    //}

        //    listener.Prefixes.Add(read);
        //    listener.Start();
        //    Console.WriteLine("Listening...");
        //    // Note: The GetContext method blocks while waiting for a request.
        //    HttpListenerContext context = listener.GetContext();
        //    HttpListenerRequest request = context.Request;
        //    // Obtain a response object.
        //    HttpListenerResponse response = context.Response;
        //    // Construct a response.
        //    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
        //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        //    // Get a response stream and write the response to it.
        //    response.ContentLength64 = buffer.Length;
        //    System.IO.Stream output = response.OutputStream;
        //    output.Write(buffer, 0, buffer.Length);
        //    // You must close the output stream.
        //    output.Close();
        //    listener.Stop();
        //}

        ////private static void server_ActionRequested(object sender, ActionRequestedEventArgs e)
        ////{
        ////    e.Server.WriteDefaultAction(e.Context);
        ////}
        ///



    }
}

