using System;

namespace PortableRest
{

    /// <summary>
    /// 
    /// </summary>
    public class PortableRestException : Exception
    {

        #region Properties

        /// <summary>
        /// The contents of the XmlReader at the time of the exception.
        /// </summary>
        /// <remarks>This can help you figure out what node is causing the problem with Serialization,
        /// as DataCOntractSerializer does not provide this information by default.</remarks>
        public string XmlReaderContents { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// A clean instance of the PortableRestException.
        /// </summary>
        /// <remarks>You should probably pass more information than this constructor allows.</remarks>
        public PortableRestException()
        {
        }

        /// <summary>
        /// An instance of PortableRestException that passes along a human-readable message.
        /// </summary>
        /// <param name="message">A human-readable message to pass back to the developer.</param>
        public PortableRestException(string message) : base(message)
        {         
        }

        /// <summary>
        /// An instance of PortableRestException that passes along a human-readable message, 
        /// along with the node causing the problem.
        /// </summary>
        /// <param name="message">A human-readable message to pass back to the developer.</param>
        /// <param name="xmlReaderContents">The contents of the XmlReader at the time of the exception.</param>
        public PortableRestException(string message, string xmlReaderContents) : base(message)
        {
            XmlReaderContents = xmlReaderContents;
        }

        /// <summary>
        /// An instance of PortableRestException that passes along a human-readable message, 
        /// along with the original Exception thrown by the Serializer.
        /// </summary>
        /// <param name="message">A human-readable message to pass back to the developer.</param>
        /// <param name="inner">The Exception thrown by the Serializer.</param>
        public PortableRestException(string message, Exception inner) : base(message, inner)
        {       
        }

        /// <summary>
        /// An instance of PortableRestException that passes along a human-readable message, 
        /// along with the node causing the problem, and the original Exception thrown by the Serializer.
        /// </summary>
        /// <param name="message">A human-readable message to pass back to the developer.</param>
        /// <param name="xmlReaderContents">The contents of the XmlReader at the time of the exception.</param>
        /// <param name="inner">The Exception thrown by the Serializer.</param>
        /// <remarks>This is the instance that will typically be thrown on a serialization error.</remarks>
        public PortableRestException(string message, string xmlReaderContents, Exception inner) : base(message, inner)
        {
            XmlReaderContents = xmlReaderContents;
        }

        #endregion

    }

}