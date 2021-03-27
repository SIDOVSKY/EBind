using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace EBind.PropertyAccessors
{
    internal static class PropertyAccessorCache
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IAccessor> s_cache = new(MemberInfoEqualityComparer.Instance);

        public static IAccessor Find(PropertyInfo propertyInfo, Type targetType)
        {
            if (propertyInfo.ReflectedType != targetType)
            {
                propertyInfo = targetType.GetProperty(propertyInfo.Name);
            }

            return s_cache.GetOrAdd(propertyInfo, CreateAccessor);
        }

        /// <summary>
        /// Requires concrete <see cref="PropertyInfo"/>s because produced delegate may call the method non-virtually.
        /// </summary>
        private static IAccessor CreateAccessor(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.ReflectedType;
            var propertyType = propertyInfo.PropertyType;

            var refTypeTarget = !targetType.IsValueType;
            var refTypeProperty = !propertyType.IsValueType;

            if (refTypeProperty && refTypeTarget)
                return new RefTargetRefPropertyAccessor(propertyInfo);

            var type = refTypeTarget
                ? typeof(RefTargetPropertyAccessor<>).MakeGenericType(propertyType)
                : typeof(PropertyAccessor<,>).MakeGenericType(targetType, propertyType);

            return (IAccessor)Activator.CreateInstance(type, propertyInfo);
        }
    }
}
