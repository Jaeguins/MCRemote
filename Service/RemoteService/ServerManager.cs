using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteService
{
    public class ServerManager
    {
        private TcpListener server;
        private int port;
        private IPEndPoint ep;
        public bool terminated { get; private set; }= false;
        private ServerCallBack callback;
        public void Start()
        {
            ep = new IPEndPoint(IPAddress.Any, this.port);
            server = new TcpListener(ep);
            server.Start(20);
            new Thread(() =>
            {
                while (!terminated)
                {
                    TcpClient client=null;
                    StreamWriter writer=null;
                    StreamReader reader=null;
                    try
                    {
                        client = server.AcceptTcpClient();
                        writer = new StreamWriter(client.GetStream());
                        reader = new StreamReader(client.GetStream());
                        callback(reader, writer);
                        writer.Flush();
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error Occured");
                    }
                    finally
                    {
                        writer?.Close();
                        reader?.Close();
                        client?.Close();
                    }
                }
            }).Start();
        }
        public ServerManager(int port, ServerCallBack callback)
        {
            this.port = port;
            this.callback = callback;
        }

        public void Close()
        {
            terminated = true;
            server.Stop();
        }
    }
}