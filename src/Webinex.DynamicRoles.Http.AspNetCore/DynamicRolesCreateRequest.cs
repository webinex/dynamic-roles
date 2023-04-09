using System.Collections.Generic;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    public class DynamicRolesCreateRequest
    {
        public string[] UserIds { get; set; }
        public string[] Permissions { get; set; }
        public IDictionary<string, object> Values { get; set; }
    }
}