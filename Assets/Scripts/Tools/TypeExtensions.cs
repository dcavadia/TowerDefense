using System;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    public static Type[] GetAllDerivedTypes(this Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToArray();
    }
}
