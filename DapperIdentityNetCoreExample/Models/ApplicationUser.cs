using NetCore.Identity.Dapper.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DapperIdentityNetCoreExample.Models
{
    public class ApplicationUser : IdentityUser , IDapperIdentityUser<ApplicationUser>
    {
        // Custom properties....
 
    }
}
