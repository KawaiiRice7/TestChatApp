using System;
using System.Threading.Tasks;
using Common;

namespace AzureSqlDAL
{
    public class AzureAccountRepository : ICredentialAccessor
    {
        public async Task<bool> VerifyCredential(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
