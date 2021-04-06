using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SWE1_SEB_Eory
{
    public class HTTPmessage
    {

        public HTTPmessage(TcpClient socket, List<string> messagesList)
        {
            var stream = socket.GetStream();
            string message = "";
            Thread.Sleep(300);

            while (stream.DataAvailable)
            {
                Byte[] bytes = new Byte[4096];
                int i = stream.Read(bytes, 0, bytes.Length);
                message += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            }
            Console.WriteLine(message);

            var requester = new RequestContext(message, messagesList);
            var responder = new ResponseContext(requester);
            using var writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };
            writer.WriteLine(responder.ExecuteRequest());
        }
    }
}
