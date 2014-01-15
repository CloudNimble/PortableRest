using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace PortableRest.Extensions
{
    /// <summary>
    /// Extension methods for reading response content.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Gets the content as the specified type synchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static T GetContentAs<T>(this HttpResponseMessage message)
        {
            var task = message.Content.ReadAsStringAsync();
            task.Wait();
            return JsonConvert.DeserializeObject<T>(task.Result);
        }

        /// <summary>
        /// Gets the content as a string synchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string ReadAsString(this HttpResponseMessage message)
        {
            var task = message.Content.ReadAsStringAsync();
            task.Wait();
            return task.Result;
        }
    }
}
