using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal interface IRolePermissionModel<TRolePermission>
    {
        TRolePermission New(string roleId, string permission);
        string Permission(TRolePermission rolePermission);
        string RoleId(TRolePermission rolePermission);
        Expression<Func<TRolePermission, bool>> ByRoleId(object roleId);
        Expression<Func<TRolePermission, bool>> RoleIdIn(IEnumerable<object> roleIds);
    }

    internal class RolePermissionModel<TRole, TRolePermission, TRoleUser> : IRolePermissionModel<TRolePermission>
    {
        private readonly IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> _definition;

        public RolePermissionModel(IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> definition)
        {
            _definition = definition;
        }

        public TRolePermission New(string roleId, string permission)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
            permission = permission ?? throw new ArgumentNullException(nameof(permission));
            return _definition.NewRolePermission(roleId, permission)
                   ?? throw new InvalidOperationException(
                       $"{nameof(_definition.NewRolePermission)} might not return null.");
        }

        public string Permission(TRolePermission rolePermission)
        {
            rolePermission = rolePermission ?? throw new ArgumentNullException(nameof(rolePermission));
            return _definition.RolePermissionPermission.Compile().Invoke(rolePermission);
        }

        public Expression<Func<TRolePermission, bool>> ByRoleId(object roleId)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));

            var typedRoleId = TypedRoleId(roleId);
            return Expressions.Equals(_definition.RolePermissionRoleId, typedRoleId);
        }

        public Expression<Func<TRolePermission, bool>> RoleIdIn(IEnumerable<object> roleIds)
        {
            roleIds = roleIds ?? throw new ArgumentNullException(nameof(roleIds));

            var typedRoleIds = roleIds.Select(TypedRoleId);
            return Expressions.Contains(_definition.RolePermissionRoleId, typedRoleIds);
        }

        public string RoleId(TRolePermission rolePermission)
        {
            rolePermission = rolePermission ?? throw new ArgumentNullException(nameof(rolePermission));

            return _definition.RolePermissionRoleId.Compile().Invoke(rolePermission).ToString();
        }

        private object TypedRoleId(object roleId)
        {
            var roleIdType = Expressions.ReturnType(_definition.RolePermissionRoleId);
            return TypeDescriptor.GetConverter(roleIdType).ConvertFromInvariantString(roleId.ToString());
        }
    }
}