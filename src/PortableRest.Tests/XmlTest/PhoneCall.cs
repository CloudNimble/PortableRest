using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PortableRest.Tests.XmlTest
{

    [DataContract(Namespace = "")]
    public class PhoneCall
    {

        [DataMember]
        [XmlAttribute]
        public string ID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

    }
}
