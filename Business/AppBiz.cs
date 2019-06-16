using System;
using System.Threading.Tasks;
using Unity;

using Common;
using CoreDomain;

namespace Business
{
    public class AppBiz
    {
        private Func<IChatAccessor> ChatAccessor { get; set; }
        private Func<ICredentialAccessor> CredentialAccessor { get; set; }

        public AppBiz()
        {
            ChatAccessor = () => ObjectFactory.Resolve<IChatAccessor>();
            CredentialAccessor = () => ObjectFactory.Resolve<ICredentialAccessor>();
        }

        // Who calls this function?
        // How does my HttpClient relate to this login?
        public async Task<bool> Login()
        {
            //User user = HttpClient.Response();

            //// Validation logic
            //if (blahblah)
            //    return false;

            //User db = await CredentialAccessor().GetUser(user.Username, user.Password);

            //return userCredentials == db;
            return true;
        }

        public async Task<bool> ReceiveMessage()
        {


            return true;
        }

        public async Task<bool> UpdateClientChat()
        {

            return true;
        }
    }
}
