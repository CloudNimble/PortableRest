using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PortableRest
{
	internal class FileParameter : EncodedParameter
	{
		internal FileParameter(string key, Stream fileStream) : base(key, fileStream) { }

		internal FileParameter(string key, Stream fileStream, string filename) : base(key, fileStream)
		{
			this.Filename = filename;
		}

		internal string Filename { get; set; }
	}
}
