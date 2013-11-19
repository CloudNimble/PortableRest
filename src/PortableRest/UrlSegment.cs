using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableRest
{
    internal class UrlSegment
    {

        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsQueryString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public UrlSegment(string key, string value)
        {
            Key = key;
            Value = value;
            IsQueryString = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isQueryString"></param>
        public UrlSegment(string key, string value, bool isQueryString)
        {
            Key = key;
            Value = value;
            IsQueryString = isQueryString;
        }

    }
}
