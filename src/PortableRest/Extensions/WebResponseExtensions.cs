using System.IO;

namespace System.Net
{
    public static class WebResponseExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ReadResponseStream(this WebResponse response)
        {
            var responseStream = response.GetResponseStream();
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }

        }

    }
}
