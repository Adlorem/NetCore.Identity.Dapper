namespace NetCore.Identity.Dapper.Interfaces
{
    /// <summary>
    /// Interface for user identity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IApplicationUser<T> : IUser where T : class, IUser<string>
    {
        string Email { get; set; }
        bool EmailConfirmed { get; set; }
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }
        bool TwoFactorEnabled { get; set; }
        DateTimeOffset? LockoutEnd { get; set; }
        bool LockoutEnabled { get; set; }
        int AccessFailedCount { get; set; }
        string NormalizedUserName { get; set; }
        string NormalizedEmail { get; set; }
        string ConcurrencyStamp { get; set; }
    }
}