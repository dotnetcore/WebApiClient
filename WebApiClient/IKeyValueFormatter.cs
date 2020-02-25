using System.Collections.Generic;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// Defines the behavior of converting objects to key-value pairs
    /// </summary>
    public interface IKeyValueFormatter
    {
        /// <summary>
        /// Serialized objects are key-value pairs
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="obj">Object instance</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(string name, object obj, FormatOptions options);

        /// <summary>
        /// Serialization parameters are key-value pairs
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options);
    }
}
