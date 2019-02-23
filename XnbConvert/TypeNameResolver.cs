using System;
using System.Collections.Generic;

namespace XnbConvert
{
    //  Not all XNB types reside in .NET framework, for example, the Vector type.
    //  This library contains its own implementation of those types.
    //  That's why the AssemblyQualifiedName property cannot be used on every type.
    public class TypeNameResolver
    {
        private static readonly Dictionary<Type, Func<string>> NameResolvers = new Dictionary<Type, Func<string>>
        {
            {typeof(string), () => typeof(string).AssemblyQualifiedName},
            {typeof(int), () => typeof(int).AssemblyQualifiedName}
        };
            
        public static string ResolveAssemblyQualifiedName<TTarget>()
        {
            Type type = typeof(TTarget);
            if (NameResolvers.TryGetValue(type, out Func<string> resolver))
            {
                return resolver();
            }
            else
            {
                throw new NotSupportedException($"{type.Name} is not supported");
            }
        }
    }
}