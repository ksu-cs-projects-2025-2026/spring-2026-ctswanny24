namespace Recip_EZ.Server.Models
{
    public class User
    {
        public string Id { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public DateTime DateOfCreation { get; init; }

        public Dictionary<int, Ingredient> Ingredients { get; init; }

    }
}
