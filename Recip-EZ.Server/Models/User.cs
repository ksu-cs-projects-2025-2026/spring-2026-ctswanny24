namespace Recip_EZ.Server.Models
{
    public class User
    {
        public int UserId { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public DateTime CreatedOn { get; init; }

    }
}
