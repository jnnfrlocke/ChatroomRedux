﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("192.168.0.116", 9999);
            Console.WriteLine("Press enter to begin chatting.");
            client.Send();
            client.Recieve();
            //Console.WriteLine("Press enter to begin chatting.");
            Console.ReadLine();
        }
    }
}
