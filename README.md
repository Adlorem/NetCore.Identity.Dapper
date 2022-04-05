Under development. This library aims to replace Identity Entity Framework database handling with Dapper. Example project uses Net Core, but works fine with Blazor.

In your web application create default model for identity user. For example:

```csharp
   public class ApplicationUser : IdentityUser, IDapperIdentityUser<ApplicationUser>
    {
        // Add custom properties here
    }
```
Make sure your database table [AspNetUsers] can handle custom properties. You can use Entity Framework to update your database.

In your web application Program.cs replace/add following default identity to use dapper istead of Entity Framework.

```csharp
using NetCore.Identity.Dapper.Interfaces;

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<ApplicationRole>()
    .AddDapperIdentityStores<ApplicationUser>(options => options.ConnectionString = connectionString);
```

You are ready to go. Keep in mind to replace IdentityUser with in this case example ApplicationUser when using stores.

TODO: Unit testing.
