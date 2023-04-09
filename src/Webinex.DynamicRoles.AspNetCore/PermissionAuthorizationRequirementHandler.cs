using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Webinex.DynamicRoles.AspNetCore
{
    internal class PermissionAuthorizationRequirementHandler<TRole> : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly IDynamicRoles<TRole> _dynamicRoles;
        private readonly IDynamicRolesAspNetCoreAuthorizationSettings _settings;
        private readonly ILogger<PermissionAuthorizationRequirementHandler<TRole>> _logger;

        public PermissionAuthorizationRequirementHandler(IDynamicRoles<TRole> dynamicRoles, IDynamicRolesAspNetCoreAuthorizationSettings settings, ILogger<PermissionAuthorizationRequirementHandler<TRole>> logger)
        {
            _dynamicRoles = dynamicRoles;
            _settings = settings;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionAuthorizationRequirement requirement)
        {
            var userId = _settings.UserIdClaimValue(context.User);
            if (userId == null)
            {
                _logger.LogInformation("User id claim not found");
                context.Fail();
                return;
            }

            var permissions = await _dynamicRoles.GetUserPermissionsAsync(userId);
            if (!requirement.Condition(permissions))
            {
                _logger.LogInformation("Condition verification failed");
                context.Fail();
                return;
            }
            
            context.Succeed(requirement);
        }
    }
}