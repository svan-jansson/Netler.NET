using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Netler.Reflection
{
    internal static class Extensions
    {
        internal static IDictionary<string, object> ToAnonymous(this object data)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (data != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(data))
                {
                    object obj = propertyDescriptor.GetValue(data);
                    dict.Add(propertyDescriptor.Name, obj);
                }
            }

            return dict;
        }

        internal static T FromAnonymous<T>(this IDictionary<string, object> anonymousData)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var pair in anonymousData)
            {
                type.GetProperty(pair.Key).SetValue(obj, pair.Value);
            }
            return (T)obj;
        }
    }
}
