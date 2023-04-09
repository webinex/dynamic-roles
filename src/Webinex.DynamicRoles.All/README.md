# Dynamic Roles
____
Provides ability to control access based on dynamic roles model. 
It means that users can create roles and assign permissions to roles.

## Configure
#### 1. Create permissions configuration
```c#
public static class PermissionsSettings
{
    public static PermissionsConfiguration Value => new PermissionsConfiguration(new[]
    {
        new PermissionConfiguration("users.read"),
        new PermissionConfiguration("users.write")
        {
            // When permission includes another, if you add
            // this permission, it might add included permission.
            // If included permission removed, this might be removed as well.
            Includes = new[] { "users.read" },
        }
        // ....
    });
}
```

### 2.1. EF core provider
##### 2.1.1. Create models
```c#
/**
*   Feel free to write your type names, property names and property types (Guid isn't mandatory).
*/

/**
*   You might create tables using your migrations approach
*/

public class Role
{
    public Guid Id { get; set; }
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public string Permission { get; set; }
}

public class RoleUser
{
    public Guid RoleId { get; set; }
    public Guid UserId { get; set; }
}
```

#### 2.1.2. Create DbContext
```c#
public class IdentityDbContext : DbContext, IDynamicRoleDbContext<Role, RoleUser, RolePermission>
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleUser> RoleUsers { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
}
```

#### 2.1.3. Create model definition
```c#
internal class DynamicRolesModelDefinition : IDynamicRoleModelsDefinition<Role, RolePermission, RoleUser>
{
    public Expression<Func<Role, object>> RoleId { get; } = role => role.Id;

    public Expression<Func<RolePermission, object>> RolePermissionRoleId { get; } =
        rolePermission => rolePermission.RoleId;

    public Expression<Func<RolePermission, string>> RolePermissionPermission { get; } =
        rolePermission => rolePermission.Permission;

    public Expression<Func<RoleUser, object>> RoleUserRoleId { get; } = roleUser => roleUser.RoleId;

    public Expression<Func<RoleUser, object>> RoleUserUserId { get; } = roleUser => roleUser.UserId;
    
    public Role NewRole(IDictionary<string, object> values)
    {
        return new Role { Id = Guid.NewGuid() };
    }

    public RolePermission NewRolePermission(string roleId, string permission)
    {
        return new RolePermission { RoleId = Guid.Parse(roleId), Permission = permission };
    }

    public RoleUser NewRoleUser(string roleId, string userId)
    {
        return new RoleUser { RoleId = Guid.Parse(roleId), UserId = Guid.Parse(userId) };
    }

    public void SetRoleValues(Role role, IDictionary<string, object> values)
    {
        // ....
    }
}
```
#### 2.1.4. Register services
```c#
services
    .AddProtego(protego => protego
        .AddDynamicRoles<Role>(dynamicRoles => dynamicRoles
            .AddDbContext<IdentityDbContext, DynamicRolesModelDefinition>()
            .AddSaveChangesDecorator()
            .UsePermissionsConfiguration(PermissionsSettings.Value)));
```

### 2.2. Http Provider
#### 2.2.1. Create Http client factory
```c#
public class DynamicRolesHttpClientFactory : IDynamicRolesHttpClientFactory
{
    public Task<HttpClient> CreateAsync()
    {
        var httpClient = new HttpClient {
            BaseAddress = "http://......",
        };
        // Add your JWT tokens headers here...
        
        return Task.FromResult(httpClient);
    }
}
```

#### 2.2.2. Register services
```c#
services
    .AddDynamicRoles<Role>(options => options
    .AddHttp(http => http.AddHttpFactory<DynamicRolesHttpClientFactory>())
```

#### 2.2.3. In "source" service add dynamic roles controller

```c#
services
    .AddControllersWithViews()
    .AddDynamicRolesController<Role>()
```

## Usages (management)
You can check the methods of IDynamicRoles{TRole}.  

## Usages (check)  

> Warning: if you using attributes checks, you might register attributes!
>
> ```c#
> protego
>    .AddDynamicRoles<Role>(options => options
>        /// .....
>        .AddAspNetAuthorization(auth => auth
>            .AddAuthorizationPoliciesFromPermissionAttributesIn(typeof(Startup).Assembly)))
> ```

#### Attributes
```c#
public class AppController : ControllerBase
{
    // User might have users.read OR roles.write permission
    [Permission.Any("users.read", "roles.write")]
    public async Task<IActionResult> CheckedForAny()
    {
        /// ....
    }
    
    // User might have users.read AND roles.write permissions
    [Permission.All("users.read", "roles.write")]
    public async Task<IActionResult> CheckedForAll()
    {
        /// ....
    }

    // User might have users.read permission
    [Permission("users.read")]
    public async Task<IActionResult> CheckedForOne()
    {
        /// ....
    }
}
```

#### Code
```c#
public class BusinessService
{
    private readonly IDynamicRoles<Role> _dynamicRoles;

    public async Task InvokeAsync()
    {
        if (await _dynamicRoles.HasAnyAsync(userId, new[] { "users.read", "roles.write" })) {
            //... has any permission
        }
        
        if (await _dynamicRoles.HasAllAsync(userId, new[] { "users.read", "roles.write" })) {
            //... has all permissions
        }
    }
}
```

## Extensions

#### How to use custom DTO model in dynamic roles controller?

```c#
// -- RoleDto.cs
// Create your role model
public class RoleDto
{
    // Might be public constructor with single parameter of Role type
    public RoleDto(Role role)
    {
        // .... assign your fields
    }
}

// -- Startup.cs
// Add role DTO to controller registration

services
      .AddControllersWithViews()
      .AddDynamicRolesController<Role, RoleDto>();
```

If you need more complex mapping:
```c#
// -- RoleDtoMapper.cs
// Create your own mapper2

public class RoleDtoMapper : IDynamicRoleDtoMapper<TRole, TRoleDto>
{
    // your impl
}

// -- Startup.cs
// Register it in DI container
services
      .AddControllersWithViews()
      .AddDynamicRolesController<Role, RoleDto>()
      .Services
      .AddScoped<IDynamicRoleDtoMapper<Role, RoleDto>>();
```

#### How to add authorization to DynamicRolesController?
```c#
// -- Startup.cs

services
      .AddControllersWithViews()
      .AddDynamicRolesController<Role, RoleDto>(x => x
        // Specify policy
        .AddPolicy("YOUR_POLICY", JwtBearerDefaults.AuthenticationSchema))
        
services.AddAuthorization(auth => auth
    // Add your policy
    .AddPolicy("YOUR_POLICY", x => x.RequiredAuthenticatedUser()))
```

#### How to add additional properties to Role model?

```c#
// -- Role.cs
// Add properties to model
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

// -- ModelDefinition.cs
// Add values mapping

internal class DynamicRolesModelDefinition : IDynamicRoleModelsDefinition<Role, RolePermission, RoleUser>
{
    public Role NewRole(IDictionary<string, object> values)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = values["Name"],
            Description = values["Description"]
        };
    }

    public void SetRoleValues(Role role, IDictionary<string, object> values)
    {
        role.Name = values["Name"];
        role.Description = values["Description"];
    }
}

// -- BusinessService.cs
// Example of update

var values = new Dictionary<string, object>
{
    ["Name"] = "New Role Name",
    ["Description"] = "New Role Description",
}

await _dynamicRoles.UpdateRoleAsync(new UpdateRoleArgs(roleId, values: values));
```