using System;
using System.Collections.Generic;
using System.Text;
using CoreDomain;

namespace SqlDAL
{
    public class MessageDataModel
    {
        public long Id { get; set; }
        public long Server { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime PostDateTime { get; set; }

        public Message ToMessage()
        {
            Message message = new Message();

            message.Id = Id;
            message.Author = new User(UserId, Username);
            message.Content = Content;
            message.Date = PostDateTime;

            return message;
        }
    }
}
