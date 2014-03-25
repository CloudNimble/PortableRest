using System.Xml.Linq;

namespace PortableRest.Tests
{
	internal static class StringExtensions
	{
		public static string AsXmlSanitized(this string source)
		{
			return
				XElement
				.Parse(source)
				.ToString(SaveOptions.DisableFormatting);
		}
	}
}