using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    public static class DynamicRolesConfigurationExtensions
    {
        private const string DB_CONTEXT_KEY = "Webinex.DynamicRoles.EntityFrameworkCore.DB_CONTEXT";

        /// <summary>
        ///     Adds DbContext based services as Dynamic Roles store
        /// </summary>
        /// <param name="configuration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <typeparam name="TDbContext">Type of DbContext</typeparam>
        /// <typeparam name="TModelDefinition">Entity types models definitions</typeparam>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        /// <exception cref="InvalidOperationException">When TDbContext doesn't implement required interfaces correctly</exception>
        public static IDynamicRolesConfiguration AddDbContext<TDbContext, TModelDefinition>(
            [NotNull] this IDynamicRolesConfiguration configuration)
            where TDbContext : DbContext
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            configuration.Values[DB_CONTEXT_KEY] = typeof(TDbContext);

            var serviceTypes = GetServiceTypes<TDbContext>();

            configuration.Services.AddScoped(serviceTypes.StoreInterfaceType, serviceTypes.StoreImplType);
            configuration.Services.AddSingleton(serviceTypes.ModelDefinitionType, typeof(TModelDefinition));
            configuration.Services.AddSingleton(serviceTypes.RoleModelType, serviceTypes.RoleModelImplType);
            configuration.Services.AddSingleton(serviceTypes.RoleUserModelType, serviceTypes.RoleUserModelImplType);
            configuration.Services.AddSingleton(serviceTypes.RolePermissionModelType, serviceTypes.RolePermissionModelImplType);

            return configuration;
        }

        private static ServiceTypes<TDbContext> GetServiceTypes<TDbContext>()
        {
            var dbContextInterface = typeof(TDbContext).ClosedGenericInterface(typeof(IDynamicRoleDbContext<,,>));
            if (dbContextInterface == null)
                throw new InvalidOperationException($"{typeof(TDbContext).Name} might implement IDynamicRoleDbContext");

            var genericArguments = dbContextInterface.GetGenericArguments();
            return new ServiceTypes<TDbContext>(genericArguments[0], genericArguments[1], genericArguments[2]);
        }

        /// <summary>
        ///     Adds DbContext.SaveChangesAsync() calls after mutable methods in <see cref="IDynamicRoles{TRole}"/>
        /// </summary>
        /// <param name="configuration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        /// <exception cref="InvalidOperationException">Thrown when this method called before <see cref="AddDbContext{TDbContext,TModelDefinition}"/></exception>
        public static IDynamicRolesConfiguration AddSaveChangesDecorator(
            [NotNull] this IDynamicRolesConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (!configuration.Values.ContainsKey(DB_CONTEXT_KEY))
                throw new InvalidOperationException(
                    $"You might call {nameof(AddDbContext)} before calling {nameof(AddSaveChangesDecorator)}");

            var dbContextType = (Type)configuration.Values[DB_CONTEXT_KEY];
            configuration.Services.Decorate(
                typeof(IDynamicRoles<>).MakeGenericType(configuration.RoleType),
                typeof(SaveChangesDecorator<,>).MakeGenericType(configuration.RoleType, dbContextType));

            return configuration;
        }

        private class ServiceTypes<TDbContext>
        {
            private readonly Type _roleType;
            private readonly Type _roleUserType;
            private readonly Type _rolePermissionType;

            public ServiceTypes(Type roleType, Type roleUserType, Type rolePermissionType)
            {
                _roleType = roleType;
                _roleUserType = roleUserType;
                _rolePermissionType = rolePermissionType;
            }

            public Type StoreImplType => typeof(EfCoreDynamicRolesStore<,,,>).MakeGenericType(
                typeof(TDbContext),
                _roleType,
                _roleUserType,
                _rolePermissionType);

            public Type StoreInterfaceType => typeof(IDynamicRoleStore<>).MakeGenericType(_roleType);

            public Type ModelDefinitionType =>
                typeof(IDynamicRoleModelsDefinition<,,>).MakeGenericType(_roleType, _rolePermissionType, _roleUserType);

            public Type RoleModelType => typeof(IRoleModel<>).MakeGenericType(_roleType);

            public Type RoleModelImplType =>
                typeof(RoleModel<,,>).MakeGenericType(_roleType, _rolePermissionType, _roleUserType);

            public Type RoleUserModelType => typeof(IRoleUserModel<>).MakeGenericType(_roleUserType);

            public Type RoleUserModelImplType =>
                typeof(RoleUserModel<,,>).MakeGenericType(_roleType, _rolePermissionType, _roleUserType);

            public Type RolePermissionModelType => typeof(IRolePermissionModel<>).MakeGenericType(_rolePermissionType);

            public Type RolePermissionModelImplType =>
                typeof(RolePermissionModel<,,>).MakeGenericType(_roleType, _rolePermissionType, _roleUserType);
        }
    }
}