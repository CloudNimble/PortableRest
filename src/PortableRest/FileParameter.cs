namespace PortableRest
{
	/// <summary>
	/// 
	/// </summary>
	public class FileParameter
	{
		/// <summary>
		/// 
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public byte[] Data { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="bytes"></param>
		/// <param name="fileName"></param>
		public FileParameter(string name, byte[] bytes, string fileName)
		{
			FileName = fileName;
			Data = bytes;
			Name = name;
		}
	}
}
