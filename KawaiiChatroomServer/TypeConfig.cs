using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


using Common;
using SqlDAL;
using CoreDomain;

namespace KawaiiChatroomServer
{
    public class TypeConfig
    {
        public IConfiguration Configuration { get; set; }

        public void Start()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("AppSettings.json");
            Configuration = builder.Build();

            bool useProductionServer = false;
            string connectionString = useProductionServer ? Configuration.GetConnectionString("AzureSqlConnectionString") : Configuration.GetConnectionString("SqlConnectionString");

            // Register instances
            ObjectFactory.RegisterInstance<IConnectionHelper>(new HttpHelper());
            ObjectFactory.RegisterInstance<ICredentialAccessor>(useProductionServer ? (ICredentialAccessor)new AzureSqlAccountRepository(connectionString)  
                : (ICredentialAccessor)new SqlAccountRepository(connectionString));
            ObjectFactory.RegisterInstance<IChatAccessor>(useProductionServer ? (IChatAccessor)new AzureSqlMessageRepository(connectionString)
                : (IChatAccessor)new SqlMessageRepository(connectionString));


            var test = Console.Out;
            Console.WriteLine("Started server...");

            var connectionHelper = ObjectFactory.Resolve<IConnectionHelper>();

            connectionHelper.StartListening(6112);
        }
    }
}
