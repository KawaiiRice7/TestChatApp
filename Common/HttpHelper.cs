using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;
using CoreDomain;
using Newtonsoft.Json;
using System.Threading;

namespace Common
{
    public class HttpHelper : IConnectionHelper
    {
        private Func<IChatAccessor> chatAccessor;
        private Func<ICredentialAccessor> credentialAccessor;
        public User clientUser;
        public List<(string, Socket)> socketPool = new List<(string, Socket)>();
        public ManualResetEvent allDone = new ManualResetEvent(false);

        public HttpHelper()
        {
            chatAccessor = () => ObjectFactory.Resolve<IChatAccessor>();
            credentialAccessor = () => ObjectFactory.Resolve<ICredentialAccessor>();
        }

        // Async example from MS website

        public void StartListening(int portNum)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], portNum);

            Console.WriteLine($"Local address and port : {localEP.ToString()}");

            Socket listener = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEP);
                listener.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Closing the listener...");
        }

        // Called when we receive the client's message
        public async void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            bool authenticated = false;

            // Signal the main thread to continue.  
            allDone.Set();

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;

            try
            {
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(AuthenticateUser), state);
                SendNewMessages(handler);
            }
            catch
            {
                Console.WriteLine("Failed to login, cya...");
                handler.Close();
            }

            Console.WriteLine("Login successful, welcome to the server.");
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        // Reads data and finally responds to the client
        public async void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.  
            try
            {
                int read = handler.EndReceive(ar);

                // Data was read from the client socket.  
                if (read > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);

                    string content = state.sb.ToString().Replace("<EOF>", "");

                    var incomingMessage = JsonConvert.DeserializeObject<Message>(content);
                    incomingMessage.Author = state.user;

                    Console.WriteLine($"[{incomingMessage.Date}] {incomingMessage.Author.Username}: {incomingMessage.Content}");

                    // Post new message to db
                    var addMessage = chatAccessor().PostMessage(incomingMessage);
                    state.sb.Clear();

                    SendNewMessages(handler);
                }
            }
            catch
            {
                
                if (socketPool.Where(item => item.Item1 == state.user.Id).Count() > 0)
                {
                    socketPool.Remove((state.user.Id, handler));
                    Console.WriteLine($"{state.user.Username} has disconnected from the server.");
                }

                handler.Close();
            }
        }

        public async void AuthenticateUser(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                while (true)
                {
                    int read = handler.EndReceive(ar);

                    // Data was read from the client socket.  
                    if (read > 0)
                    {
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);

                        string content = state.sb.ToString().Replace("<EOF>", "");

                        // Add a UserId to it after verifying in db.
                        var userCredentials = JsonConvert.DeserializeObject<User>(content);

                        userCredentials.Id = await credentialAccessor().VerifyCredential(userCredentials.Username, userCredentials.Password);

                        if (!string.IsNullOrEmpty(userCredentials.Id))
                        {
                            Console.WriteLine($"[{DateTime.Now}]: {userCredentials.Username} has logged in.");
                            socketPool.Add((userCredentials.Id, handler));
                            state.user = userCredentials;
                            state.sb.Clear();

                            handler.Send(Encoding.ASCII.GetBytes("true"));

                            return;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"{handler.Handle} failed to login.");
                handler.Close();
            }

            return;
        }

        public async void SendNewMessages(Socket clientSocket)
        {
            var chatLog = await chatAccessor().GetMessages(10);
            var msgs = JsonConvert.SerializeObject(chatLog);

            byte[] messages = Encoding.ASCII.GetBytes(msgs);

            System.Threading.Thread.Sleep(2000);

            // Fire an event to send updated chat to all connected sockets
            foreach (var socket in socketPool)
            {
                socket.Item2.Send(messages);
            }
        }
    }
}
