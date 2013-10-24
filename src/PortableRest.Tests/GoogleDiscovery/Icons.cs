using System.Runtime.Serialization;

namespace Google.Discovery
{

    [DataContract]
    public class Icons
    {

        [DataMember]
        public string X16 { get; set; }

        [DataMember]
        public string X32 { get; set; }

    }

}