using System;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles.Stores
{
    internal class InMemoryPermissionConfigurationStore : IPermissionsConfigurationStore
    {
        private readonly PermissionsConfiguration _value;

        public InMemoryPermissionConfigurationStore(PermissionsConfiguration value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Task<PermissionsConfiguration> GetAsync()
        {
            return Task.FromResult(_value);
        }
    }
}