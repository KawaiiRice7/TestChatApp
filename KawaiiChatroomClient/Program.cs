using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KawaiiChatroomClient
{
    class Program
    {

        static void Main(string[] args)
        {
            ChatClient client = new ChatClient();
            client.ExecuteClient(6112);

            Console.WriteLine("Sent the message.");
        }

        // ExecuteClient() Method 
        
    }
}
