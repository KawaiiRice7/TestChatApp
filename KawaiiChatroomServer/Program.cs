using System;

namespace KawaiiChatroomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start app
            TypeConfig config = new TypeConfig();

            config.Start();

            while (true) ;
        }
    }
}
