using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace KawaiiChatroomClient
{
    public class ChatClient
    {
        public List<Message> receivedMessages = new List<Message>();
        public bool authenticated = false;

        public void ExecuteClient(int portNum)
        {
            //User user = new User("KawaiiRice", "FakePw");
            //Message message = new Message(null, "Test message");
            Message message = new Message();

            // Establish the remote endpoint for the socket. This example uses port 11111 on the local computer. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, portNum);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Connect Socket to the remote endpoint using method Connect() 
                sender.Connect(localEndPoint);

                // We print EndPoint information that we are connected 
                Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

                Authenticate(sender);

                ReceiveMessages(sender);

                while (true)
                {
                    // Creation of message that we will send to Server 
                    Console.WriteLine("Send: ");
                    message.Content = Console.ReadLine();
                    message.Date = DateTime.Now;

                    byte[] messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message) + "<EOF>");

                    int byteSent = sender.Send(messageSent);

                    ReceiveMessages(sender);
                }
            }

            // Manage of Socket's Exceptions 
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }

            catch (SocketException se)
            {

                Console.WriteLine("SocketException : {0}", se.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            
        }

        public void Authenticate(Socket socket)
        {
            while (!authenticated)
            {
                //Console.WriteLine("Please enter your username.");
                //string username = Console.ReadLine();
                //Console.WriteLine("Please enter your password.");
                //string password = Console.ReadLine();

                string username = "KawaiiRice";
                string password = "FakePw";

                Console.WriteLine("Authenticating...");

                socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new User(username, password)) + "<EOF>"));
                byte[] loginResponseBytes = new byte[12000];
                int loginResponseInt = socket.Receive(loginResponseBytes);
                var response = Encoding.ASCII.GetString(loginResponseBytes, 0, loginResponseInt);

                authenticated = Encoding.ASCII.GetString(loginResponseBytes, 0, loginResponseInt) == "true" ? true : false;
            }
        }

        public void ReceiveMessages(Socket socket)
        {
            byte[] messageReceived = new byte[160000];

            int serverResponse = socket.Receive(messageReceived);
            var msgParsed = Encoding.ASCII.GetString(messageReceived, 0, serverResponse);
            var newMessages = JsonConvert.DeserializeObject<List<Message>>(msgParsed.Replace("<EOF>", ""));

            newMessages = newMessages.Where(msg => msg.Date > (receivedMessages.Count == 0 ? DateTime.MinValue : receivedMessages.Last().Date)).OrderBy(msg => msg.Date).ToList();
            receivedMessages.AddRange(newMessages);

            Console.Clear();

            foreach (var msg in receivedMessages)
            {
                Console.WriteLine($"[{msg.Date}] {msg.Author.Username}: {msg.Content}");
            }
        }
    }
}
