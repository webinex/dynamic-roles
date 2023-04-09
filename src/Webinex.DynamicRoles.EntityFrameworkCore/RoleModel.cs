using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal interface IRoleModel<TRole>
    {
        string RoleId(TRole role);
        Expression<Func<TRole, bool>> RoleIdIn(IEnumerable<object> roleIds);
        object TypedRoleId(object roleId);
        object TypedRoleId(string roleId);
        TRole NewRole(IDictionary<string, object> values);
        void SetRoleValues(TRole role, IDictionary<string, object> values);
    }

    internal class RoleModel<TRole, TRolePermission, TRoleUser> : IRoleModel<TRole>
    {
        private readonly IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> _definition;

        public RoleModel(IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> definition)
        {
            _definition = definition;
        }

        public string RoleId(TRole role)
        {
            role = role ?? throw new ArgumentNullException(nameof(role));

            return _definition.RoleId.Compile().Invoke(role).ToString();
        }

        public Expression<Func<TRole, bool>> RoleIdIn(IEnumerable<object> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));

            var typedRoleIds = roleIds.Select(TypedRoleId);
            return Expressions.Contains(_definition.RoleId, typedRoleIds);
        }

        public object TypedRoleId(object roleId)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));

            var roleIdType = Expressions.ReturnType(_definition.RoleId);
            return TypeDescriptor.GetConverter(roleIdType).ConvertFromInvariantString(roleId.ToString());
        }

        public object TypedRoleId(string roleId)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
            return TypedRoleId((object)roleId);
        }

        public TRole NewRole(IDictionary<string, object> values)
        {
            values = values ?? throw new ArgumentNullException(nameof(values));
            return _definition.NewRole(values) ??
                   throw new InvalidOperationException($"{nameof(_definition.NewRole)} might not return null.");
        }

        public void SetRoleValues(TRole role, IDictionary<string, object> values)
        {
            role = role ?? throw new ArgumentNullException(nameof(role));
            values = values ?? throw new ArgumentNullException(nameof(values));
            _definition.SetRoleValues(role, values);
        }
    }
}