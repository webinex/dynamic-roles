using System.ComponentModel.DataAnnotations;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    public class DynamicRolesUserRolesUpdateRequest
    {
        [Required] public string UserId { get; set; }

        [Required] public string[] RoleIds { get; set; }
    }
}