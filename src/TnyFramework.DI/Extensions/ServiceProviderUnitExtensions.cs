// // Copyright (c) 2020 Tunaiyi
// // Tny Framework For CSharp is licensed under Mulan PSL v2.
// // You can use this software according to the terms and conditions of the Mulan PSL v2.
// // You may obtain a copy of Mulan PSL v2 at:
// //          http://license.coscl.org.cn/MulanPSL2
// // THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// // See the Mulan PSL v2 for more details.
//
// using System;
// using Microsoft.Extensions.DependencyInjection;
// using TnyFramework.DI.Container;
//
// namespace TnyFramework.DI.Extensions;
//
// public static class ServiceProviderUnitExtensions
// {
//     public static T? GetService<T>(this IServiceProvider provider)
//     {
//         var unit =
//         return (T) provider.GetService(typeof(IUnit<T>));
//     }
//
//     public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
//     {
//         ThrowHelper.ThrowIfNull((object) provider, nameof(provider));
//         ThrowHelper.ThrowIfNull((object) serviceType, nameof(serviceType));
//         if (provider is ISupportRequiredService supportRequiredService)
//             return supportRequiredService.GetRequiredService(serviceType);
//         return provider.GetService(serviceType) ?? throw new InvalidOperationException(SR.Format(SR.NoServiceRegistered, (object) serviceType));
//     }
//
//     /// <summary>
//     /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
//     /// </summary>
//     /// <typeparam name="T">The type of service object to get.</typeparam>
//     /// <param name="provider">The <see cref="T:System.IServiceProvider" /> to retrieve the service object from.</param>
//     /// <returns>A service object of type <typeparamref name="T" />.</returns>
//     /// <exception cref="T:System.InvalidOperationException">There is no service of type <typeparamref name="T" />.</exception>
//     public static T GetRequiredService<T>(this IServiceProvider provider) where T : notnull
//     {
//         ThrowHelper.ThrowIfNull((object) provider, nameof(provider));
//         return (T) provider.GetRequiredService(typeof(T));
//     }
//
//     /// <summary>
//     /// Get an enumeration of services of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
//     /// </summary>
//     /// <typeparam name="T">The type of service object to get.</typeparam>
//     /// <param name="provider">The <see cref="T:System.IServiceProvider" /> to retrieve the services from.</param>
//     /// <returns>An enumeration of services of type <typeparamref name="T" />.</returns>
//     public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
//     {
//         ThrowHelper.ThrowIfNull((object) provider, nameof(provider));
//         return provider.GetRequiredService<IEnumerable<T>>();
//     }
//
//     public static IEnumerable<object?> GetServices(this IServiceProvider provider, Type serviceType)
//     {
//         ThrowHelper.ThrowIfNull((object) provider, nameof(provider));
//         ThrowHelper.ThrowIfNull((object) serviceType, nameof(serviceType));
//         Type serviceType1 = typeof(IEnumerable<>).MakeGenericType(serviceType);
//         return (IEnumerable<object>) provider.GetRequiredService(serviceType1);
//     }
//
//     public static IServiceCollection getUnit(this IServiceProvider services, string name, Type instanceType)
//     {
//         services.GetService<>()
//         var serviceInstance = new ScopedServiceInstance(new IServiceProvider(instanceType));
//         return services.RegisterScopedUnit(name, serviceInstance, instanceType);
//     }
// }
