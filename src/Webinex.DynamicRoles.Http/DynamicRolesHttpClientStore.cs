using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pathoschild.Http.Client;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles.Http
{
    internal class DynamicRolesHttpClientStore<TRole> : IDynamicRoleStore<TRole>, IPermissionsConfigurationStore,
        IDisposable
    {
        private readonly Lazy<FluentClient> _fluentClientLazy;

        public DynamicRolesHttpClientStore(IDynamicRolesHttpClientFactory httpClientFactory)
        {
            _fluentClientLazy = new Lazy<FluentClient>(() =>
            {
                var httpClient = httpClientFactory.CreateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                return new FluentClient(httpClient.BaseAddress, httpClient);
            });
        }

        private FluentClient HttpClient => _fluentClientLazy.Value;

        private string Uri(string path)
        {
            var pathPart = path ?? throw new ArgumentNullException(nameof(path));

            if (!pathPart.StartsWith("/"))
                pathPart = "/" + pathPart;

            return new Uri(HttpClient.BaseClient.BaseAddress, "/api/protego/dynamic-roles" + pathPart).ToString();
        }

        public async Task<PermissionsConfiguration> GetAsync()
        {
            var response = await HttpClient
                .GetAsync(Uri("permissions/configuration"))
                .As<PermissionsConfigurationResponse>();

            return response.ToModel();
        }

        public async Task<string[]> CreateAsync(IEnumerable<CreateRoleArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

            return await HttpClient
                .PostAsync(Uri("roles"))
                .WithBody(args)
                .As<string[]>();
        }

        public async Task DeleteAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));

            await HttpClient
                .DeleteAsync(Uri("roles"))
                .WithBody(roleIds);
        }

        public async Task<IDictionary<string, TRole>> RolesAsync()
        {
            return await HttpClient.GetAsync(Uri("roles")).As<IDictionary<string, TRole>>();
        }

        public async Task<IDictionary<string, TRole>> RolesByIdAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            var arguments = roleIds.Select(id => KeyValuePair.Create("roleId", id));

            return await HttpClient
                .GetAsync(Uri("roles/by-id"))
                .WithArguments(arguments)
                .As<IDictionary<string, TRole>>();
        }

        public async Task UpdateRolesAsync(IEnumerable<UpdateRoleArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

            await HttpClient
                .PutAsync(Uri("roles"))
                .WithBody(args);
        }

        public async Task UpdateUsersRolesAsync(IEnumerable<UpdateUserRolesArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

            await HttpClient
                .PutAsync(Uri("users/roles"))
                .WithBody(args);
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUserRolesAsync(IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));
            var arguments = userIds.Select(userId => KeyValuePair.Create("userId", userId));

            return await HttpClient
                .GetAsync(Uri("users/roles"))
                .WithArguments(arguments)
                .As<IDictionary<string, IEnumerable<string>>>();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUserPermissionsAsync(IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));
            var arguments = userIds.Select(userId => KeyValuePair.Create("userId", userId));

            return await HttpClient
                .GetAsync(Uri("users/permissions"))
                .WithArguments(arguments)
                .As<IDictionary<string, IEnumerable<string>>>();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetRolePermissionsAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            var arguments = roleIds.Select(roleId => KeyValuePair.Create("roleId", roleId));

            return await HttpClient
                .GetAsync(Uri("roles/permissions"))
                .WithArguments(arguments)
                .As<IDictionary<string, IEnumerable<string>>>();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUsersByRoleIdsAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            var arguments = roleIds.Select(userId => KeyValuePair.Create("roleId", userId));

            return await HttpClient
                .GetAsync(Uri("roles/users"))
                .WithArguments(arguments)
                .As<IDictionary<string, IEnumerable<string>>>();
        }

        public void Dispose()
        {
            if (_fluentClientLazy.IsValueCreated)
            {
                _fluentClientLazy.Value.Dispose();
            }
        }
    }
}