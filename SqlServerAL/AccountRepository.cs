using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using Common;
using Dapper;
using CoreDomain;



namespace SqlDAL
{
    public class SqlAccountRepository : ICredentialAccessor
    {
        private string connectionString = string.Empty;

        public SqlAccountRepository(string _connectionString)
        {
            this.connectionString = _connectionString;
        }

        // How to return a User object in this design without referring to CoreDomain?
        public async Task<string> VerifyCredential(string username, string password)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = $"SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";
                var query = await db.QueryAsync<User>(sql, new { Username = username, Password = password});

                return query.AsList().Count > 0 ? query.AsList<User>()[0].Id : string.Empty;
            }
        }

        public async Task<User> GetUser(string userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = $"SELECT * FROM [User] WHERE Id = @UserId";
                var query = await db.QueryAsync<User>(sql, new { UserId = userId });

                return query.AsList().Count > 0 ? query.AsList<User>()[0] : null;
            }
        }

        public async Task<bool> CreateAccount(string username, string password)
        {
            if (string.IsNullOrEmpty(await VerifyCredential(username, password)))
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO [User] VALUES (@UserId, @Username, @Password, @CurrentDateTime, @CurrentDateTime)";
                    var query = await db.QueryAsync<User>(sql, new { UserId = Guid.NewGuid(), Username = username, Password = password, CurrentDateTime = DateTime.Now });

                    return query != null ? true : false;
                }
            }
            else
            {
                // Log exception
                return false;
            }
        }
    }

    public class AzureSqlAccountRepository : ICredentialAccessor
    {
        private string connectionString = string.Empty;

        public AzureSqlAccountRepository(string _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public async Task<string> VerifyCredential(string username, string password)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = $"SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";
                var query = await db.QueryAsync<User>(sql, new { Username = username, Password = password });

                return query.AsList().Count > 0 ? query.AsList<User>()[0].Id : string.Empty;
            }
        }

        public async Task<User> GetUser(string userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = $"SELECT * FROM [User] WHERE Id = @UserId";
                var query = await db.QueryAsync<User>(sql, new { UserId = userId });

                return query.AsList().Count > 0 ? query.AsList<User>()[0] : null;
            }
        }

        public async Task<bool> CreateAccount(string username, string password)
        {
            if (string.IsNullOrEmpty(await VerifyCredential(username, password)))
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO [User] VALUES (@UserId, @Username, @Password, @CurrentDateTime, @CurrentDateTime)";
                    var query = await db.QueryAsync<User>(sql, new { UserId = Guid.NewGuid(), Username = username, Password = password, CurrentDateTime = DateTime.Now });

                    return query != null ? true : false;
                }
            }
            else
            {
                // Log exception
                return false;
            }
        }
    }
}
