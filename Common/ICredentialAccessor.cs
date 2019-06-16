using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreDomain;

namespace Common
{
    public interface ICredentialAccessor
    {
        Task<string> VerifyCredential(string username, string password);
        Task<bool> CreateAccount(string username, string password);
        Task<User> GetUser(string userId);
    }
}
