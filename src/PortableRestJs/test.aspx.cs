using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PortableRestJs
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            string body = string.Empty;
            using (StreamReader streamReader = new StreamReader(Request.InputStream))
            {
                body = streamReader.ReadToEnd();
            }
            JObject jObject = null;
            switch(Request.ContentType)
            {
                case "application/json":
                    jObject = JsonConvert.DeserializeObject<JObject>(body);
                    break;
                case "application/x-www-form-urlencoded":
                    jObject = new JObject();
                    foreach (string postField in Request.Form.AllKeys)
                    {
                        jObject.AddFirst(new JProperty(postField, Request.Form[postField]));
                    }
                    break;
            }
            Response.Write(JsonConvert.SerializeObject(jObject));
            Response.ContentType = "application/json";
        }
    }
}