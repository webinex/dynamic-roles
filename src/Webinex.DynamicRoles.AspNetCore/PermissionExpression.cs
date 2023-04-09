using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.DynamicRoles.AspNetCore
{
    internal class PermissionExpression
    {
        public PermissionExpression(Operator @operator, IEnumerable<string> values)
        {
            values = values?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(values));
            if (!values.Any()) throw new ArgumentException("Might contain at least one value", nameof(values));

            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Values = values.ToArray();

            Lexical = string.Join($" {@operator.Lexical} ", Values);
        }
        
        public Operator Operator { get; }
        
        public string[] Values { get; }
        
        public string Lexical { get; }

        public Func<string[], bool> Predicate
        {
            get
            {
                if (Operator.Kind == Operator.Any.Kind)
                    return permissions => Values.Any(permissions.Contains);

                if (Operator.Kind == Operator.All.Kind)
                    return permissions => Values.All(permissions.Contains);

                throw new InvalidOperationException($"Unknown operator {Operator.Kind}");
            }
        }

        public static PermissionExpression Parse(string value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Might contain at least permission", nameof(value));

            if (!value.Contains(" "))
                return new PermissionExpression(Operator.Any, new[] { value });

            foreach (var @operator in Operator.Values)
            {
                var join = $" {@operator.Lexical} ";
                
                if (!value.Contains(join))
                    continue;

                var values = value.Split(join, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (!values.Any()) throw new InvalidOperationException($"Malformed lexical permission value \"{value}\"");

                return new PermissionExpression(@operator, values);
            }
            
            throw new InvalidOperationException($"Malformed lexical permission value \"{value}\"");
        }
    }
}