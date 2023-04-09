using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    [Route("/api/protego/dynamic-roles")]
    public class DynamicRolesStoreController<TRole, TRoleDto> : Controller
    {
        private readonly IDynamicRoles<TRole> _dynamicRoles;
        private readonly IDynamicRolesStoreControllerSettings _settings;
        private readonly IDynamicRoleDtoMapper<TRole, TRoleDto> _mapper;

        public DynamicRolesStoreController(
            IDynamicRoles<TRole> dynamicRoles,
            IDynamicRolesStoreControllerSettings settings,
            IDynamicRoleDtoMapper<TRole, TRoleDto> mapper)
        {
            _dynamicRoles = dynamicRoles;
            _settings = settings;
            _mapper = mapper;
        }

        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        private IAuthenticationService AuthenticationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        [HttpGet("permissions/configuration")]
        public virtual async Task<IActionResult> GetPermissionsConfigurationAsync()
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var result = await _dynamicRoles.PermissionsConfigurationAsync();
            return Ok(result);
        }

        [HttpGet("users/permissions")]
        public virtual async Task<IActionResult> GetUserPermissionsAsync(string[] userId)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (userId == null)
                return BadRequest($"{nameof(userId)} might not be null");

            userId = userId.ToArray();

            if (userId.Any(x => x == null))
                return BadRequest($"{nameof(userId)} might not contain nulls");

            var permissions = await _dynamicRoles.PermissionsByUserIdAsync(userId);
            return Ok(permissions);
        }

        [HttpGet("users/roles")]
        public virtual async Task<IActionResult> GetUserRolesAsync(string[] userId)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (userId == null)
                return BadRequest($"{nameof(userId)} might not be null");

            if (userId.Any(x => x == null))
                return BadRequest($"{nameof(userId)} might not contain nulls");

            var roles = await _dynamicRoles.RolesByUserIdAsync(userId);
            return Ok(roles);
        }

        [HttpGet("roles/permissions")]
        public virtual async Task<IActionResult> GetRolePermissionsAsync(string[] roleId)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (roleId == null)
                return BadRequest($"{nameof(roleId)} might not be null");

            if (roleId.Any(x => x == null))
                return BadRequest($"{nameof(roleId)} might not contain nulls");

            var permissions = await _dynamicRoles.PermissionsByRoleIdAsync(roleId);
            return Ok(permissions);
        }

        [HttpGet("roles/by-id")]
        public virtual async Task<IActionResult> RolesByIdAsync(string[] roleId)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (roleId == null)
                return BadRequest($"{nameof(roleId)} might not be null");

            if (roleId.Any(x => x == null))
                return BadRequest($"{nameof(roleId)} might not contain nulls");

            var roles = await _dynamicRoles.RolesByIdAsync(roleId);
            var result = await _mapper.MapAsync(roles);
            return Ok(result);
        }
        
        [HttpGet("roles/users")]
        public virtual async Task<IActionResult> UsersByRoleIdAsync(string[] roleId)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (roleId == null)
                return BadRequest($"{nameof(roleId)} might not be null");

            if (roleId.Any(x => x == null))
                return BadRequest($"{nameof(roleId)} might not contain nulls");
            
            var usersByRoleId = await _dynamicRoles.UsersByRoleIdAsync(roleId);
            return Ok(usersByRoleId);
        }

        [HttpGet("roles")]
        public virtual async Task<IActionResult> RolesAsync()
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var roles = await _dynamicRoles.RolesAsync();
            var result = await _mapper.MapAsync(roles);
            return Ok(result);
        }

        [HttpPost("roles")]
        public virtual async Task<IActionResult> CreateRolesAsync([FromBody] DynamicRolesCreateRequest[] models)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (models == null)
                return BadRequest($"{nameof(models)} might not be null");

            if (models.Any(x => x == null))
                return BadRequest($"{nameof(models)} might not contain nulls");

            var args = models.Select(
                    x => new CreateRoleArgs(x.UserIds, x.Permissions, x.Values))
                .ToArray();

            var roleIds = await _dynamicRoles.CreateRolesAsync(args);
            return Ok(roleIds);
        }

        [HttpPut("roles")]
        public virtual async Task<IActionResult> UpdateRolesAsync([FromBody] DynamicRoleUpdateRequest[] models)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var args = models.Select(model => new UpdateRoleArgs(model.Id, model.UserIds, model.Permissions, model.Values)).ToArray();
            await _dynamicRoles.UpdateRolesAsync(args);
            return Ok();
        }

        [HttpDelete("roles")]
        public virtual async Task<IActionResult> UpdateRolesAsync([FromBody] string[] roleIds)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            if (roleIds == null)
                return BadRequest($"{nameof(roleIds)} might not be null");

            if (roleIds.Any(x => x == null))
                return BadRequest($"{nameof(roleIds)} might not contain nulls");

            await _dynamicRoles.DeleteRolesAsync(roleIds);

            return Ok();
        }

        [HttpPut("users/roles")]
        public virtual async Task<IActionResult> UpdateUsersRolesAsync([FromBody] DynamicRolesUserRolesUpdateRequest[] models)
        {
            if (!await AuthorizeAsync())
                return Forbid();

            var args = models.Select(model => new UpdateUserRolesArgs(model.UserId, model.RoleIds)).ToArray();
            await _dynamicRoles.UpdateUsersRolesAsync(args);
            return Ok();
        }

        private async Task<bool> AuthorizeAsync()
        {
            if (_settings.Policy == null)
                return true;

            var authenticationResult = await AuthenticationService.AuthenticateAsync(HttpContext, _settings.Schema);
            if (!authenticationResult.Succeeded) 
                return false;
            
            var authorizationResult = await AuthorizationService.AuthorizeAsync(authenticationResult.Principal!, _settings.Policy);
            return authorizationResult.Succeeded;
        }
    }
}