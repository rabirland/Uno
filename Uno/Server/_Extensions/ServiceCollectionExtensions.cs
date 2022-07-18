using System.Reflection;
using Uno.Server.Annotation;

namespace Uno.Server
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds every class to the service collection that is marked with <see cref="ServiceAttribute"/>,
        /// by their first interface. For classes without interfaces, they will be added by their own type.
        /// </summary>
        /// <param name="collection">The service collection.</param>
        public static void AddMarkedServices(this IServiceCollection collection)
        {
            var services = typeof(Program)
                .Assembly
                .GetTypes()
                .Select(t => (Type: t, Attribute: t.GetCustomAttribute<ServiceAttribute>()))
                .Where(t => t.Attribute != null);

            foreach (var service in services)
            {
                var isSingleton = service.Attribute!.AsSingleton;
                var interfaces = service.Type.GetInterfaces();

                if (interfaces.Length == 0)
                {
                    AddType(collection, isSingleton, service.Type);
                }
                else
                {
                    AddTypeInterface(collection, isSingleton, interfaces[0], service.Type);
                }
            }
        }

        private static void AddType(IServiceCollection collection, bool asSingleton, Type type)
        {
            if (asSingleton)
            {
                collection.AddSingleton(type);
            }
            else
            {
                collection.AddScoped(type);
            }
        }

        private static void AddTypeInterface(IServiceCollection collection, bool asSingleton, Type interfaceType, Type type)
        {
            if (asSingleton)
            {
                collection.AddSingleton(interfaceType, type);
            }
            else
            {
                collection.AddScoped(interfaceType, type);
            }
        }
    }
}
