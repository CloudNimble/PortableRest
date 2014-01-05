using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Google.Discovery
{

    /// <summary>
    /// A container for the request to get the list of APIs from Google.
    /// </summary>
    /// <remarks>
    /// Because the "GetApis" request returns metadata in addition to the list of APIs, we need a container object.
    /// </remarks>
    [DataContract]
    public class ApisResult
    {

        [DataMember]
        public string Kind { get; set; }

        [DataMember(Name = "discoveryVersion")]
        public string Version { get; set; }

        /// <summary>
        /// The list of available APIs.
        /// </summary>
        /// <remarks>
        /// This demonstrates how you can have one object name in the payload, but a better name in your API.
        /// Don't let some dev's bad decisions force you to write crappy-looking code.
        /// </remarks>
        [DataMember(Name = "items")]
        public List<Api> Apis { get; set; }

    }

}
