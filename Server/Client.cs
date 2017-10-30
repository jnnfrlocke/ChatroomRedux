using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        NetworkStream stream;
        TcpClient client;
        public string UserId;
        public Client(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            UserId = Guid.NewGuid().ToString();
        }

        public string RequestNewUser()
        {
            Send("Welcome to the chatroom. Please enter a user name: ");
            byte[] receivedMessage = new byte[256];
            stream.Read(receivedMessage, 0, receivedMessage.Length);
            string userName = Encoding.ASCII.GetString(receivedMessage).Trim(new char[] { '\0' });
            //Recieve(); //gets stuck here because of the while true
            CreateNewUser(userName);
            return userName;
        }

        private void CreateNewUser(string userName)
        {
            User newUser = new User();
            newUser.GenerateNewUser(userName);
        }

        public void Send(string Message)
        {
            byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
        }
        public string Recieve()
        {
            while (true)
            {
                try
                {
                    byte[] receivedMessage = new byte[256];
                    stream.Read(receivedMessage, 0, receivedMessage.Length);
                    string receivedMessageString = Encoding.ASCII.GetString(receivedMessage).Trim(new char[] { '\0' });
                    Message message = new Message(this, receivedMessageString);
                    Server.msgQueue.Enqueue(message);
                    Console.WriteLine(receivedMessageString);
                }
                catch (Exception)
                {
                    Server.chatClientsList.Remove(this);
                    Console.WriteLine("Chat User has disconnected their session.");
                }
            }
        }




        //byte[] recievedMessage = new byte[256];
        //    stream.Read(recievedMessage, 0, recievedMessage.Length);
        //    string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
        //    Console.WriteLine(recievedMessageString);
        //    return recievedMessageString;
        //}

    }
}
