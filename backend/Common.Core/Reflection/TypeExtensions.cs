
using System.Reflection;

namespace ActivityGameBackend.Common.Core.Reflection;
public static class TypeExtensions
{
    public static Dictionary<string, T?> GetFieldValues<T>(this Type type) where T : class
    {
        ArgumentNullException.ThrowIfNull(type);

        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(T))
            .ToDictionary(f => f.Name, f => f.GetValue(null) as T);
    }
}