using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Common;
using CoreDomain;

namespace SqlDAL
{
    public class SqlMessageRepository : IChatAccessor
    {
        private string connectionString = string.Empty;
        private Func<ICredentialAccessor> credentialAccessor;

        public SqlMessageRepository(string _connectionString)
        {
            this.connectionString = _connectionString;
            this.credentialAccessor = () => ObjectFactory.Resolve<ICredentialAccessor>();
        }


        public async Task<List<Message>> GetMessages(int msgCount)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Message> messages = new List<Message>();

                string sql = $"SELECT * FROM ( SELECT TOP {msgCount} * FROM ChatLogs ORDER BY PostDateTime DESC ) sq ORDER BY PostDateTime ASC";
                var query = await db.QueryAsync<MessageDataModel>(sql);

                messages.AddRange(from msg in query select msg.ToMessage());

                foreach (var msg in messages)
                {
                    msg.Author = await credentialAccessor().GetUser(msg.Author.Id);
                }

                return messages;
            }
        }

        public async Task<bool> PostMessage(Message message)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"INSERT INTO ChatLogs VALUES ('1', @UserId, @Content, @Date)";
                    var query = await db.QueryAsync<MessageDataModel>(sql, new { UserId = message.Author.Id, Content = message.Content, Date = message.Date });
                    return true;
                }
                catch (Exception ex)
                {
                    // need a logger, log with a guid
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }

    public class AzureSqlMessageRepository : IChatAccessor
    {
        private string connectionString = string.Empty;
        private Func<ICredentialAccessor> credentialAccessor;

        public AzureSqlMessageRepository(string _connectionString)
        {
            this.connectionString = _connectionString;
            this.credentialAccessor = () => ObjectFactory.Resolve<ICredentialAccessor>();

        }

        public async Task<List<Message>> GetMessages(int msgCount)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Message> messages = new List<Message>();

                string sql = $"SELECT * FROM ( SELECT TOP {msgCount} * FROM ChatLogs ORDER BY PostDateTime DESC ) sq ORDER BY PostDateTime ASC";
                var query = await db.QueryAsync<MessageDataModel>(sql);

                messages.AddRange(from msg in query select msg.ToMessage());

                foreach (var msg in messages)
                {
                    msg.Author = await credentialAccessor().GetUser(msg.Author.Id);
                }

                return messages;
            }
        }

        public async Task<bool> PostMessage(Message message)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"INSERT INTO ChatLogs VALUES ('1', @UserId, @Content, @Date)";
                    var query = await db.QueryAsync<MessageDataModel>(sql, new { UserId = message.Author.Id, Content = message.Content, Date = message.Date });
                    return true;
                }
                catch (Exception ex)
                {
                    // need a logger, log with a guid
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
