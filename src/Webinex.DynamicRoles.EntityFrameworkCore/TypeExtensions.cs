using System;
using System.Linq;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal static class TypeExtensions
    {
        internal static Type ClosedGenericInterface(this Type @class, Type interfaceOpenType)
        {
            @class = @class ?? throw new ArgumentNullException(nameof(@class));
            interfaceOpenType = interfaceOpenType ?? throw new ArgumentNullException(nameof(interfaceOpenType));

            if (!@class.IsClass || @class.IsAbstract)
                throw new ArgumentException("Might be non-abstract class", nameof(@class));

            if (!interfaceOpenType.IsInterface || interfaceOpenType.IsConstructedGenericType)
                throw new ArgumentException("Might be non-constructed interface", nameof(interfaceOpenType));
            
            return @class
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.IsConstructedGenericType)
                .SingleOrDefault(x => x.GetGenericTypeDefinition() == interfaceOpenType);
        }
    }
}