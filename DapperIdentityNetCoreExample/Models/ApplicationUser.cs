using NetCore.Identity.Dapper.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DapperIdentityNetCoreExample.Models
{
    public class ApplicationUser : IdentityUser , IApplicationUser<ApplicationUser>
    {
        // Custom properties....
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
