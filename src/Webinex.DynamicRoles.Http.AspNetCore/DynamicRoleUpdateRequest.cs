using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    public class DynamicRoleUpdateRequest
    {
        [Required] public string Id { get; set; }
        public string[] UserIds { get; set; }
        public string[] Permissions { get; set; }
        public IDictionary<string, object> Values { get; set; }
    }
}