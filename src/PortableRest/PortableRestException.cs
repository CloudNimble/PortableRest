using System;

namespace PortableRest
{

    public class PortableRestException : Exception
    {

        public string XmlReaderContents { get; set; }

        public PortableRestException()
        {
            
        }

        public PortableRestException(string message) : base(message)
        {
            
        }

        public PortableRestException(string message, string xmlReaderContents) : base(message)
        {
            XmlReaderContents = xmlReaderContents;
        }

        public PortableRestException(string message, Exception inner) : base(message, inner)
        {
            
        }

        public PortableRestException(string message, string xmlReaderContents, Exception inner)
            : base(message, inner)
        {
            XmlReaderContents = xmlReaderContents;
        }

    }

}
