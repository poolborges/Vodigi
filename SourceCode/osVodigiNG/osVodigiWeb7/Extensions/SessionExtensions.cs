using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace osVodigiWeb7.Extensions
{
    /// <summary>
    /// Provides extension methods to the <see cref="ISession"/> class.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Creates an instance with arguments.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <remarks>
        /// <para>Throws an exception if the factory failed to get an instance of the specified type.</para>
        /// <para>The arguments are used as dependencies by the factory.</para>
        /// </remarks>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Gets an obejct stored session.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <remarks>Throws an exception failed to get an instance of the specified type.</remarks>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
