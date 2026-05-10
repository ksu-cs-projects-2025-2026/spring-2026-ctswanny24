using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    /// <summary>
    /// Class for all User Login and Authentication related services. 
    /// This is where the logic for authenticating users and managing user data will be implemented.
    /// </summary>
    public class UserService
    {
        #region Fields

        private readonly RecipEzDbContext _context;

        #endregion
        
        #region Constructor(s)

        /// <summary>
        /// Constructor that allows the use of the dbcontext in the methods of this service. 
        /// This is where the connection to the database is established for this service layer.
        /// </summary>
        /// <param name="context">The database context to be used by the service</param>
        public UserService(RecipEzDbContext context)
        {
            _context = context;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Test method for development purposes. 
        /// This method retrieves all users from the database and returns them as a list.
        /// </summary>
        /// <returns>List of all users in the database</returns>
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        /// <summary>
        /// Attempts to retrieve a user from the db based on the provided username and password.
        /// </summary>
        /// <param name="username">The username of the user to retrieve</param>
        /// <param name="password">The password of the user to retrieve</param>
        /// <returns>The matched user, or null when credentials are invalid.</returns>
        public User? GetUser(string username, string password)
        {
            return _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        /// <summary>
        /// Attempts to create a new user in the database with the provided username and password.
        /// </summary>
        /// <param name="username">The username for the new user</param>
        /// <param name="password">The password for the new user</param>
        /// <returns>The newly created user, or null if the registration fails</returns>
        public User? RegisterUser(string username, string password)
        {
            //Potential to get this finished before 1.0, but will likely be a goal for 1.1 or later.
            return null;
        }

        #endregion
    }
}
