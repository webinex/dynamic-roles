using System.Net.Http;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles.Http
{
    /// <summary>
    ///     Gives ability to configure DynamicRoles http client. Such as: authorization, custom headers and etc.
    /// </summary>
    public interface IDynamicRolesHttpClientFactory
    {
        /// <summary>
        ///     Creates new instance of HttpClient to use in DynamicRoles http store
        /// </summary>
        /// <returns>New instance of HttpClient</returns>
        Task<HttpClient> CreateAsync();
    }
}