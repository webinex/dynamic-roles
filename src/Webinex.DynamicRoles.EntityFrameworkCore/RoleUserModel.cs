using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal interface IRoleUserModel<TRoleUser>
    {
        TRoleUser New(object roleId, object userId);
        string UserId(TRoleUser roleUser);
        string RoleId(TRoleUser roleUser);
        Expression<Func<TRoleUser, bool>> ByRoleId(object roleId);
        Expression<Func<TRoleUser, bool>> ByRoleIds(IEnumerable<object> roleIds);
        Expression<Func<TRoleUser, bool>> ByUserId(object userId);
        Expression<Func<TRoleUser, bool>> ByUserIds(IEnumerable<object> userIds);
    }

    internal class RoleUserModel<TRole, TRolePermission, TRoleUser> : IRoleUserModel<TRoleUser>
    {
        private readonly IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> _definition;

        public RoleUserModel(IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser> definition)
        {
            _definition = definition;
        }
        
        public TRoleUser New(object roleId, object userId)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
            userId = userId ?? throw new ArgumentNullException(nameof(userId));

            return _definition.NewRoleUser(roleId.ToString(), userId.ToString())
                ?? throw new InvalidOperationException($"{nameof(_definition.NewRoleUser)} might not return null.");
        }
        
        public string UserId(TRoleUser roleUser)
        {
            roleUser = roleUser ?? throw new ArgumentNullException(nameof(roleUser));

            return _definition.RoleUserUserId.Compile().Invoke(roleUser).ToString();
        }
        
        public Expression<Func<TRoleUser, bool>> ByRoleId(object roleId)
        {
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));

            var typedRoleId = TypedRoleId(roleId);
            return Expressions.Equals(_definition.RoleUserRoleId, typedRoleId);
        }

        public Expression<Func<TRoleUser, bool>> ByRoleIds(IEnumerable<object> roleIds)
        {
            roleIds = roleIds ?? throw new ArgumentNullException(nameof(roleIds));

            var typedRoleIds = roleIds.Select(TypedRoleId).ToArray();
            return Expressions.Contains(_definition.RoleUserRoleId, typedRoleIds);
        }

        public string RoleId(TRoleUser roleUser)
        {
            roleUser = roleUser ?? throw new ArgumentNullException(nameof(roleUser));

            return _definition.RoleUserRoleId.Compile().Invoke(roleUser).ToString();
        }

        public Expression<Func<TRoleUser, bool>> ByUserId(object userId)
        {
            userId = userId ?? throw new ArgumentNullException(nameof(userId));

            var typedUserId = TypedUserId(userId);
            return Expressions.Equals(_definition.RoleUserUserId, typedUserId);
        }

        public Expression<Func<TRoleUser, bool>> ByUserIds(IEnumerable<object> userIds)
        {
            userIds = userIds ?? throw new ArgumentNullException(nameof(userIds));

            var typedUserIds = userIds.Select(TypedUserId);
            return Expressions.Contains(_definition.RoleUserUserId, typedUserIds);
        }

        private object TypedRoleId(object roleId)
        {
            var roleIdType = Expressions.ReturnType(_definition.RoleUserRoleId);
            return TypeDescriptor.GetConverter(roleIdType).ConvertFromInvariantString(roleId.ToString());
        }

        private object TypedUserId(object userId)
        {
            var userIdType = Expressions.ReturnType(_definition.RoleUserUserId);
            return TypeDescriptor.GetConverter(userIdType).ConvertFromInvariantString(userId.ToString());
        }
    }
}