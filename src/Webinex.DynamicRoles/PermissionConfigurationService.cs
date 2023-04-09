using System.Threading.Tasks;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles
{
    internal interface IPermissionConfigurationService
    {
        Task<PermissionsConfiguration> GetAsync();
    }
    
    internal class PermissionConfigurationService : IPermissionConfigurationService
    {
        private readonly IPermissionsConfigurationStore _store;

        public PermissionConfigurationService(IPermissionsConfigurationStore store)
        {
            _store = store;
        }

        public Task<PermissionsConfiguration> GetAsync()
        {
            return _store.GetAsync();
        }
    }
}