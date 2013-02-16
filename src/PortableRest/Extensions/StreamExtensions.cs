using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class StreamExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="toWrite"></param>
        public static void WriteString(this Stream stream, string toWrite)
        {
            var bytes = Encoding.UTF8.GetBytes(toWrite);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
