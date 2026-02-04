using OnlineStore.Domain.Common;
//User este o entitate separată, responsabilă doar de datele utilizatorului, respectând SRP
namespace OnlineStore.Domain.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}
