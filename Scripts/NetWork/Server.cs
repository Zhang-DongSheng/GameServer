using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Convert = Game.Network.Convert;

namespace Game.Network
{
    class Server
    {
        private readonly Socket socket;

        private readonly byte[] buffer = new byte[1024];

        private readonly Dictionary<string, NetworkOfficer> officers = new Dictionary<string, NetworkOfficer>();

        public Server(string ipString, int port)
        {
            IPAddress address = IPAddress.Parse(ipString);

            IPEndPoint point = new IPEndPoint(address, port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(point);

            socket.Listen(20);

            Thread thread = new Thread(Listener);

            thread.Start();
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    Socket client = socket.Accept();

                    string key = client.RemoteEndPoint.ToString();

                    Register(key);

                    officers[key].receiver = new Thread(Receive)
                    {
                        IsBackground = true,
                    };
                    officers[key].receiver.Start(client);

                    officers[key].sender = new Thread(Send)
                    {
                        IsBackground = true,
                    };
                    officers[key].sender.Start(client);

                    Console.WriteLine(string.Format("客户端{0}已连接", key));
                }
                catch
                {
                    break;
                }
            }
        }

        private void Register(string key)
        {
            if (officers.ContainsKey(key)) { }
            else
            {
                officers.Add(key, new NetworkOfficer());
            }
        }

        private void Unregister(string key)
        {
            if (officers.ContainsKey(key))
            {
                officers[key].Dispose();
                officers.Remove(key);
            }
        }

        private void Receive(object socket)
        {
            Socket client = (Socket)socket;

            string key = client.RemoteEndPoint.ToString();

            while (true)
            {
                try
                {
                    int length = client.Receive(buffer);

                    if (length > 0)
                    {
                        string value = Convert.ToString(buffer, 0, length);

                        client.Send(Convert.ToBytes(value));

                        Console.WriteLine(value);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("客户端{0}已离线", key));
                        Unregister(key);
                        break;
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Unregister(key);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    break;
                }
            }
        }

        private void Send(object socket)
        {
            Socket client = (Socket)socket;

            string key = client.RemoteEndPoint.ToString();

            while (true)
            {
                try
                {
                    if (officers.ContainsKey(key) && officers[key].status)
                    {
                        officers[key].status = false;

                        byte[] buffer = Convert.ToBytes(officers[key].value);

                        client.Send(buffer);
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Unregister(key);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    break;
                }
            }
        }

        public void Send(string value, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                foreach (var k in officers.Keys)
                {
                    officers[k].value = value;
                    officers[k].status = true;
                }
            }
            else
            {
                if (officers.ContainsKey(key))
                {
                    officers[key].value = value;
                    officers[key].status = true;
                }
            }
        }

        public void Close()
        {
            if (socket != null && socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            foreach (var officer in officers.Values)
            {
                officer.Dispose();
            }
            officers.Clear();
        }
    }

    public class NetworkOfficer : IDisposable
    {
        public Thread sender;

        public Thread receiver;

        public string value;

        public bool status;

        public void Reset()
        {
            if (sender != null)
            {
                sender.Abort();
                sender = null;
            }
            if (receiver != null)
            {
                receiver.Abort();
                receiver = null;
            }
        }

        public void Dispose()
        {
            Reset();
        }
    }
}