

namespace BuildingBlocks.Secrets.Extensions;

public static class Extensions
{
    public static Dictionary<string, object> ToDictionary(this object obj)
    {
        return obj.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
    }

    public static T ToObject<T>(this IDictionary<string, object> dict) where T : new()
    {
        // Create a new instance of the object
        var obj = new T();
        // Get the properties of type T
        var objType = typeof(T);

        // Iterate over dictionary entries
        foreach (var kvp in dict)
        {
            // Find a property in T that matches the dictionary key name
            var prop = objType.GetProperty(kvp.Key,
                BindingFlags.Public | BindingFlags.Instance);

            // Check if the property exists and is writable
            if (prop != null && prop.CanWrite)
            {
                // Convert the dictionary value to the property 
                var value = Convert.ChangeType(kvp.Value, prop.PropertyType);
                prop.SetValue(obj, value);
            }
        }

        return obj;
    }
}