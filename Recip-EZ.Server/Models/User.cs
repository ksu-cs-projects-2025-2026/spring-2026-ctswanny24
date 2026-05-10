namespace Recip_EZ.Server.Models
{
    public class User
    {
        /// <summary>
        /// Id of the user in the database.
        /// </summary>
        public int UserId { get; init; }

        /// <summary>
        /// Username of user (in the form of an email address). This is used as the unique identifier for the user and is required for login and authentication purposes. 
        /// </summary>
        public string Username { get; init; }

        /// <summary>
        /// Password of user. In production, this should be stored as a hashed value for security purposes, 
        /// but for the sake of simplicity in this project, it is stored as plain text. This is not recommended for real-world applications.
        /// </summary>
        public string Password { get; init; }

        /// <summary>
        /// First name of user
        /// </summary>
        public string FirstName { get; init; }

        /// <summary>
        /// Last name of user
        /// </summary>
        public string LastName { get; init; }

        /// <summary>
        /// Gets the date and time when the object was created.
        /// </summary>
        public DateTime CreatedOn { get; init; }

    }
}
