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

    [DataContract]
    public class Api
    {
        [DataMember]
        public string Kind { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string DiscoveryRestUrl { get; set; }
        [DataMember]
        public string DiscoveryLink { get; set; }
        [DataMember]
        public Icons Icons { get; set; }
        [DataMember]
        public string DocumentationLink { get; set; }
        [DataMember]
        public bool Preferred { get; set; }
        [DataMember]
        public List<string> Labels { get; set; }
    }

    [DataContract]
    public class Icons
    {
        [DataMember]
        public string X16 { get; set; }
        [DataMember]
        public string X32 { get; set; }
    }

}
