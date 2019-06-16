using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreDomain;

namespace Common
{
    public interface IChatAccessor
    {
        Task<List<Message>> GetMessages(int count); // If 0, get last 50.
        Task<bool> PostMessage(Message content);
    }
}
