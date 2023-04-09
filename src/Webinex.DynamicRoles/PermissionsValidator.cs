using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles
{
    internal interface IPermissionsValidator
    {
        Task ValidateAndThrowAsync(IEnumerable<string> permissions);
    }
    
    internal class PermissionsValidator : IPermissionsValidator
    {
        private readonly IPermissionsConfigurationStore _permissionsConfigurationStore;

        public PermissionsValidator(IPermissionsConfigurationStore permissionsConfigurationStore)
        {
            _permissionsConfigurationStore = permissionsConfigurationStore;
        }

        public async Task ValidateAndThrowAsync(IEnumerable<string> permissions)
        {
            permissions = permissions?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(permissions));
            if (!permissions.Any()) return;

            var permissionsSet = permissions.ToHashSet();
            var configuration = await _permissionsConfigurationStore.GetAsync();
            
            foreach (var permission in permissions)
            {
                if (!configuration.Has(permission))
                    throw new InvalidOperationException($"Unknown permissions {permission}");

                var includes = configuration.ByKind(permission).Includes;
                var notFoundIncludes = includes.Where(kind => !permissionsSet.Contains(kind)).ToArray();
                if (notFoundIncludes.Any())
                    throw new InvalidOperationException(
                        $"{string.Join(", ", notFoundIncludes)} included in permission {permission}, but doesn't exist in permissions argument");
            }
        }
    }
}