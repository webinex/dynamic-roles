using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Webinex.DynamicRoles.AspNetCore
{
    public abstract class Permission
    {
        /// <summary>
        ///     Action authorization attribute which requires user to have at least one of permissions
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
        public class AnyAttribute : PermissionAttributeBase
        {
            /// <summary>
            ///     Creates new instance of attribute
            /// </summary>
            /// <param name="permissions">Permissions. User might have at least one.</param>
            public AnyAttribute(params string[] permissions) : base(Operator.Any, permissions ?? throw new ArgumentNullException(nameof(permissions)))
            {
            }
        }

        /// <summary>
        ///     Action authorization attribute which requires user to have all of permissions
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
        public class AllAttribute : PermissionAttributeBase
        {
            /// <summary>
            ///     Creates new instance of attribute
            /// </summary>
            /// <param name="permissions">Permissions. User might have all of provided permissions.</param>
            public AllAttribute(params string[] permissions) :base(Operator.All, permissions ?? throw new ArgumentNullException(nameof(permissions)))
            {
            }
        }
    }

    /// <summary>
    ///     Action authorization attribute which requires user to permission
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : Permission.AnyAttribute
    {
        /// <summary>
        ///     Creates new instance of attribute
        /// </summary>
        /// <param name="permission">Permission. User might have specified permission.</param>
        public PermissionAttribute(string permission)
            : base(permission ?? throw new ArgumentNullException(nameof(permission)))
        {
        }
    }

    /// <summary>
    ///     Base class for permission authorization attributes
    /// </summary>
    public abstract class PermissionAttributeBase : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "dynamic-roles://permission/";
        
        internal PermissionAttributeBase(Operator @operator, string[] permissions)
        {
            permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            if (!permissions.Any())
                throw new ArgumentException("Might contain at least one value", nameof(permissions));

            @operator = @operator ?? throw new ArgumentNullException(nameof(@operator));

            Expression = new PermissionExpression(@operator, permissions);
            Policy = POLICY_PREFIX + Expression.Lexical;
        }

        internal PermissionExpression Expression { get; }
    }
}