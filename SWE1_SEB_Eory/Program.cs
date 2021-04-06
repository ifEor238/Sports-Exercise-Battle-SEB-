using SWE1_SEB_Eory;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWE1_SEB_Eory
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 10001);
            listener.Start(5);
            var tasks = new List<Task>();

            while (true)
            {
                try
                {
                    tasks.Add(Task.Run(() => AddConnection(listener)));
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }
        private static void AddConnection(TcpListener listener)
        {
            List<string> reqList = new List<string>();
            var socket = listener.AcceptTcpClient();
            var message = new HTTPmessage(socket, reqList);
            socket.Close();
        }
    }
}
