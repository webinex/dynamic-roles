using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.DynamicRoles.AspNetCore
{
    /// <summary>
    ///     Permission check operator
    /// </summary>
    public class Operator
    {
        private Operator(string kind, string lexical)
        {
            Kind = kind ?? throw new ArgumentNullException(nameof(kind));
            Lexical = lexical ?? throw new ArgumentNullException(nameof(lexical));
        }

        /// <summary>
        ///     Any operator works like logical OR
        /// </summary>
        public static Operator Any { get; } = new Operator("Any", "or");

        /// <summary>
        ///     All operator works like logical AND
        /// </summary>
        public static Operator All { get; } = new Operator("All", "and");
        
        /// <summary>
        ///     All known operators
        /// </summary>
        public static ImmutableArray<Operator> Values { get; } = new[] { Any, All }.ToImmutableArray();
        
        /// <summary>
        ///     Operator kind
        /// </summary>
        [NotNull]
        public string Kind { get; }
        
        /// <summary>
        ///     Lexical representation of operator
        /// </summary>
        [NotNull]
        public string Lexical { get; }
    }
}