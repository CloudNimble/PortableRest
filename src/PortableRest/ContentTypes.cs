using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableRest
{

    /// <summary>
    /// The types of content supported in PortableRest requests and responses.
    /// </summary>
    public enum ContentTypes
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

		MultiPartFormData
    }
}
