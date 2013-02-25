using System;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace PortableRestJs
{
    public partial class xmlTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            string body = string.Empty;
            using (StreamReader streamReader = new StreamReader(Request.InputStream))
            {
                body = streamReader.ReadToEnd();
            }

            XDocument xDoc = JsonConvert.DeserializeXNode(body);

            Response.Write(xDoc.ToString(SaveOptions.DisableFormatting));
            Response.ContentType = "application/xml";
            Response.Headers.Add("Content-Type", Response.ContentType);
        }
    }
}