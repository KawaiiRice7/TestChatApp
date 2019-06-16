using System;

namespace KawaiiChatroomClient
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        // Could store previous ips and calculate the distance to notify weird login history
        // public List<string> PreviousIpLocations { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastLoginDateTime { get; set; }

        public User() { }

        public User(string username, string password)
        {
            Id = string.Empty;
            Username = username;
            Password = password;
        }
    }
}
