using System.Threading.Tasks;

namespace Webinex.DynamicRoles.Stores
{
    /// <summary>
    ///     Permissions configuration storage
    /// </summary>
    public interface IPermissionsConfigurationStore
    {
        /// <summary>
        ///     Returns permissions configuration
        /// </summary>
        /// <returns>Permissions configuration</returns>
        Task<PermissionsConfiguration> GetAsync();
    }
}