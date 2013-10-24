using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PortableRest.Tests.XmlTest
{

    [DataContract(Namespace = "")]
    public class PhoneNumber
    {

        [DataMember]
        [XmlAttribute]
        public string ID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public List<PhoneCall> Calls { get; set; }

        public PhoneNumber()
        {
            Calls = new List<PhoneCall>();
        }

        [DataMember]
        public PhoneCall Call { get; set; }

        public PhoneNumber(string id, string number) : this()
        {
            ID = id;
            Number = number;
        }

    }
}
