namespace PortableRest
{

    /// <summary>
    /// Specifies how a body parameter should be encoded before transmission.
    /// </summary>
    public enum ParameterEncoding
    {

        /// <summary>
        /// 
        /// </summary>
        Base64,

        /// <summary>
        /// 
        /// </summary>
        ByteArray,

        /// <summary>
        /// 
        /// </summary>
        UriEncoded,

        /// <summary>
        /// 
        /// </summary>
        Unencoded
    }
}
