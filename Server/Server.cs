using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        //public static Client client;
        TcpListener server;
        public static ConcurrentQueue<Message> msgQueue;
        public Server()
        {
            server = new TcpListener(IPAddress.Any, 9999); //allows for receiving incoming connections across the network as well as on this computer
            msgQueue = new ConcurrentQueue<Message>();
            server.Start();
        }
        public void Run()
        {
            Console.WriteLine("Now accepting clients on port 9999");
            //message = client.Recieve();
            Parallel.Invoke(AcceptClient, Respond);
        }
        protected async void AcceptClient() //need to thread this? - second client cannot connect
        {
            while (true)
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                Console.WriteLine("Connected");
                NetworkStream stream = clientSocket.GetStream();
                Client clientConnection = new Client(stream, clientSocket);
                chatClientsList.Add(clientConnection);
                Task getUser = Task.Run(() => GetUser(clientConnection));
                getUser.Wait();
                Task clientConnectionThread = Task.Run(() => clientConnection.Recieve());

                //clientConnectionThread.Start();
            }
        }
        
        private void GetUser(Client clientConnection)
        {
            Task<string> userName = Task.Run(() => clientConnection.RequestNewUser());
            userName.Wait();
            string name = userName.Result.Trim('\0');
            NewChatUserAlert(name, clientConnection);
        }

        private void NewChatUserAlert(string userName, Client client)
        {
            string msg = $"{userName} has joined the Chat.";
            Task<string> message = Task.Run(() => client.Recieve());
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
                        //Send to everyone except the sender of the message.
                        if (chatUser != msg.sender)
                        {
                            chatUser.Send(msg.Body);
                        }
                    }
                }
            }
        }


        //public string Respond(string body)
        //{
        //    client.Send(body);
        //    return body;
        //}

        Dictionary<string, Client> users = new Dictionary<string, Client>(); 

        Queue<Message> messages = new Queue<Message>();

    }
}
