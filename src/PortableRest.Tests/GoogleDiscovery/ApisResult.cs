using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Google.Discovery
{

    [DataContract]
    public class ApisResult
    {

        [DataMember]
        public string Kind { get; set; }

        [DataMember(Name = "discoveryVersion")]
        public string Version { get; set; }

        [DataMember(Name = "items")]
        public List<Api> Apis { get; set; }

    }

}
