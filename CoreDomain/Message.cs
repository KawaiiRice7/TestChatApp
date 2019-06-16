using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDomain
{
    public class Message
    {
        public long Id { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public Message() { }
        //public Message() { }
    }
}
