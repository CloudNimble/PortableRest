using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableRest
{
    internal class UrlSegment
    {

        #region Properties

        /// <summary>
        /// The variable name part of the Segment
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value to pass back to the service.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Whether or not the Segment is part of the main URL or the QueryString.
        /// </summary>
        public bool IsQueryString { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor for UrlSegments.
        /// </summary>
        /// <param name="key">The segment name.</param>
        /// <param name="value">The segment value.</param>
        public UrlSegment(string key, string value)
        {
            Key = key;
            Value = value;
            IsQueryString = false;
        }

        /// <summary>
        /// The default constructor for QueryStrings.
        /// </summary>
        /// <param name="key">The QueryString name.</param>
        /// <param name="value">The QueryString value.</param>
        /// <param name="isQueryString"></param>
        public UrlSegment(string key, string value, bool isQueryString)
        {
            Key = key;
            Value = value;
            IsQueryString = isQueryString;
        }

        #endregion

    }
}
