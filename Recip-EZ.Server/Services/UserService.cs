using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    public class UserService
    {
        private readonly RecipEzDbContext _context;

        public UserService(RecipEzDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUser(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault<User>(u=> u.Username == username && u.Password == password);

            if(user == null)
            {
                throw new Exception($"User with username {username} not found.");
            }
            else
            {
                return (User)user;
            }

        }
    }
}
