using System;
using Griffin.WebServer;

namespace MineHealthHttpServer
{
    class Program
    {
        class User
        {
            public int id;
            public string userName;
            public User(int id, string userName)
            {
                this.id = id;
                this.userName = userName;
            }
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
