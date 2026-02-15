using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class Admin : User
    {
        public string AccessLevel { get; set; }
        public List<string> Permissions { get; set; }

        public Admin(string username, string email, string accessLevel) : base(username, email)
        {
            AccessLevel = accessLevel;
            Permissions = new List<string>();
        }
    }
}
