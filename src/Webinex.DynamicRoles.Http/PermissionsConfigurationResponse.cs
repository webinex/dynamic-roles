using System.Linq;

namespace Webinex.DynamicRoles.Http
{
    internal class PermissionConfigurationResponse
    {
        public string Kind { get; set; }

        public string[] Includes { get; set; }

        public PermissionConfiguration ToModel()
        {
            return new PermissionConfiguration(
                Kind)
            {
                Includes = Includes,
            };
        }
    }

    internal class PermissionsConfigurationResponse
    {
        public PermissionConfigurationResponse[] Permissions { get; set; }

        public PermissionsConfiguration ToModel()
        {
            return new PermissionsConfiguration(Permissions.Select(p => p.ToModel()));
        }
    }
}