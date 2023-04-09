using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.DynamicRoles
{
    /// <summary>
    ///     Permission configuration
    /// </summary>
    public class PermissionConfiguration
    {
        private string[] _includes = Array.Empty<string>();

        /// <summary>
        ///     Creates new instance of permission configuration
        /// </summary>
        /// <param name="kind">Permission kind, might be unique within configurations</param>
        public PermissionConfiguration([NotNull] string kind)
        {
            Kind = kind ?? throw new ArgumentNullException(nameof(kind));
        }

        /// <summary>
        ///     Permission kind (identifier)
        /// </summary>
        public string Kind { get; }

        /// <summary>
        ///     What permissions included to current permission.
        ///     If current permission set, it might set included permissions.
        ///     If included removed, it might remove current permission.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public string[] Includes
        {
            get => _includes;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Any(x => x == null)) throw new ArgumentException("Might not contain nulls.", nameof(value));

                _includes = value;
            }
        }
    }
}