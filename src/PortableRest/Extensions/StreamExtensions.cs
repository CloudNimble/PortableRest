﻿using System;
using System.IO;

namespace PortableRest.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream input)
        {
            if (input == null) return new byte[0];
            var buffer = new byte[16 * 1024];
            using var ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }

    }
}
