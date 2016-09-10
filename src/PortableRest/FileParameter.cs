using System.IO;

namespace PortableRest
{

    /// <summary>
    /// 
    /// </summary>
	internal class FileParameter : EncodedParameter
	{

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        internal string Filename { get; set; }

        #endregion

        #region Constructors

        internal FileParameter(string key, Stream fileStream) : base(key, fileStream)
        {
        }

		internal FileParameter(string key, Stream fileStream, string filename) : base(key, fileStream)
		{
			this.Filename = filename;
		}

        #endregion

    }

}