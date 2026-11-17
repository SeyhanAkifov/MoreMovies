namespace MoreMovies.Models
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Manager = "Manager";

        /// <summary>Identify roles used by application authorization.</summary>
        public static bool IsBuiltIn(string name) => name is Admin or User or Manager;
    }
}
