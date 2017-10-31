using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static List<Client> chatClientsList = new List<Client>();
        TcpListener server;
        public static ConcurrentQueue<Message> msgQueue;
        public Server()
        {
            server = new TcpListener(IPAddress.Any, 9999); 
            msgQueue = new ConcurrentQueue<Message>();
            server.Start();
        }
        public void Run()
        {
            Console.WriteLine("Now accepting clients on port 9999");
            Parallel.Invoke(AcceptClient, Respond);
        }
        protected void AcceptClient() 
        {
            while (true)
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                Console.WriteLine("Connected");
                NetworkStream stream = clientSocket.GetStream();
                Client clientConnection = new Client(stream, clientSocket);
                chatClientsList.Add(clientConnection); 
                GetUser(clientConnection);
                Task.Run(() => clientConnection.Recieve());
            }
        }
        
        private void GetUser(Client clientConnection)
        {
            Task<string> userName = Task.Run(() => clientConnection.RequestNewUser());
            string name = userName.Result.Trim('\0');
            NewChatUserAlert(name, clientConnection);
        }

        private void NewChatUserAlert(string userName, Client client)
        {
            string msg = $"{userName} has joined the Chat.";
            Message message = new Message(client, msg);
            Console.WriteLine(msg);
            msgQueue.Enqueue(message);
        }

        private void LogQueue(string message)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                Logger.Log(message, sw);
            }
        }
        
        private void Respond()
        {
            while (true)
            {
                Message msg = new Message(null, "");
                if (msgQueue.TryDequeue(out msg))
                {
                    foreach (Client chatUser in chatClientsList)
                    {
                        if (chatUser != msg.sender)
                        {
                            chatUser.Send(msg.Body);
                            LogQueue(msg.Body);
                        }
                    }
                }
            }
        }
    }
}
