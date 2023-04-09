using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    internal class DefaultDynamicRoleDtoMapper<TRole, TRoleDto> : IDynamicRoleDtoMapper<TRole, TRoleDto>
    {
        private readonly ILogger _logger;

        public DefaultDynamicRoleDtoMapper(ILogger<DefaultDynamicRoleDtoMapper<TRole, TRoleDto>> logger)
        {
            _logger = logger;
        }

        public Task<IDictionary<string, TRoleDto>> MapAsync(IDictionary<string, TRole> roles)
        {
            roles = roles ?? throw new ArgumentNullException(nameof(roles));
            var result = roles.ToDictionary(x => x.Key, x => MapOne(x.Value));
            return Task.FromResult<IDictionary<string, TRoleDto>>(result);
        }

        private TRoleDto MapOne(TRole role)
        {
            if (!TrySameType(role, out var result)
                && !TryConstructor(role, out result))
            {
                throw new InvalidOperationException(
                    $"Unable to convert {typeof(TRole).Name} to {typeof(TRoleDto).Name}:" +
                    $"you might use same type or {typeof(TRoleDto).Name} might have public constructor with 1 parameter of type {typeof(TRole).Name}. " +
                    $"Or you can create your own implementation of {nameof(IDynamicRoleDtoMapper<TRole, TRoleDto>)}.");
            }

            return result;
        }

        private bool TrySameType(TRole role, out TRoleDto result)
        {
            result = default;

            if (typeof(TRole) != typeof(TRoleDto))
            {
                _logger.LogInformation(
                    $"Types of {typeof(TRole).Name} and {typeof(TRoleDto).Name} different, continue mapping.");
                return false;
            }

            _logger.LogInformation(
                $"Types of {typeof(TRole).Name} and {typeof(TRoleDto).Name} the same, return same value.");
            result = (TRoleDto)(object)role;
            return true;
        }

        private bool TryConstructor(TRole role, out TRoleDto result)
        {
            result = default;

            var constructors = typeof(TRoleDto).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var constructor = constructors.SingleOrDefault(IsMatchingConstructor);
            if (constructor == null)
            {
                _logger.LogInformation(
                    $"Unable to find public constructor with 1 parameter of type {typeof(TRole).Name} on type {typeof(TRoleDto).Name}");
                return false;
            }

            result = (TRoleDto) constructor.Invoke(new object[] { role });
            return true;
        }

        private bool IsMatchingConstructor(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            return parameters.Length == 1 && parameters.Single().ParameterType == typeof(TRole);
        }
    }
}