using System;
using System.IO;
using System.Linq;
using PortableRest.Extensions;

namespace PortableRest
{

    /// <summary>
    /// A parameter that can be passed over an HTTP request with a given encoding.
    /// </summary>
    /// <remarks>
    /// This enables parameters to be encoded individually. For example, some parameters in a request may need to be 
    /// unencoded, and some may need to be UrlEncoded.
    /// </remarks>
    internal class EncodedParameter
    {

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        internal string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal ParameterEncoding Encoding { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal EncodedParameter(string key, object value)
        {
            Key = key;
            Value = value;
            Encoding = ParameterEncoding.UriEncoded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        internal EncodedParameter(string key, object value, ParameterEncoding encoding): this(key, value)
        {
            Encoding = encoding;
        }

        #endregion

        #region Internal Methods

       internal object GetEncodedValue()
        {
            //RWM: This will not work. Need to store encoding with parameter and deal with on output, not input.
            object finalValue = null;

            switch (Encoding)
            {
                case ParameterEncoding.Base64:
                    if (!(Value is Stream)) throw new ArgumentException("ByteArray encoded objects must be passed in as a stream.");
                    var bytes1 = (Value as Stream).ToArray();
                    finalValue = Convert.ToBase64String(bytes1);
                    break;
                case ParameterEncoding.ByteArray:
                    if (!(Value is Stream)) throw new ArgumentException("ByteArray encoded objects must be passed in as a stream.");
                    finalValue = (Value as Stream).ToArray();
                    break;
                case ParameterEncoding.Unencoded:
                    finalValue = Value.ToString();
                    break;
                case ParameterEncoding.UriEncoded:
                    finalValue = Uri.EscapeDataString(Value.ToString());
                    break;
            }
            return finalValue;
        }

        #endregion

    }

}