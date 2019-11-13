using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableRest
{

    /// <summary>
    /// The types of content supported in PortableRest requests and responses.
    /// </summary>
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum ContentTypes
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
    {
        /// <summary>
        /// 
        /// </summary>
        ByteArray,

        /// <summary>
        /// 
        /// </summary>
        FormUrlEncoded,

        /// <summary>
        /// 
        /// </summary>
        Json,

        /// <summary>
        /// 
        /// </summary>
        Xml,

        /// <summary>
        /// 
        /// </summary>
		MultiPartFormData
    }
}
